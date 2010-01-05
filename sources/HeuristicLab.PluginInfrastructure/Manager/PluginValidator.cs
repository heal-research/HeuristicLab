#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Security;


namespace HeuristicLab.PluginInfrastructure.Manager {
  /// <summary>
  /// Discovers all installed plugins in the plugin directory. Checks correctness of plugin meta-data and if
  /// all plugin files are available and checks plugin dependencies. 
  /// </summary>
  internal sealed class PluginValidator : MarshalByRefObject {
    internal event EventHandler<PluginInfrastructureEventArgs> PluginLoaded;

    private Dictionary<PluginDescription, List<string>> pluginDependencies;

    private List<ApplicationDescription> applications;
    internal IEnumerable<ApplicationDescription> Applications {
      get {
        if (string.IsNullOrEmpty(PluginDir)) throw new InvalidOperationException("PluginDir is not set.");
        if (applications == null) DiscoverAndCheckPlugins();
        return applications;
      }
    }

    private IEnumerable<PluginDescription> plugins;
    internal IEnumerable<PluginDescription> Plugins {
      get {
        if (string.IsNullOrEmpty(PluginDir)) throw new InvalidOperationException("PluginDir is not set.");
        if (plugins == null) DiscoverAndCheckPlugins();
        return plugins;
      }
    }

    internal string PluginDir { get; set; }

    internal PluginValidator() {
      this.pluginDependencies = new Dictionary<PluginDescription, List<string>>();

      // ReflectionOnlyAssemblyResolveEvent must be handled because we load assemblies from the plugin path 
      // (which is not listed in the default assembly lookup locations)
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolveEventHandler;
    }

    private Assembly ReflectionOnlyAssemblyResolveEventHandler(object sender, ResolveEventArgs args) {
      return Assembly.ReflectionOnlyLoad(args.Name);
    }


    /// <summary>
    /// Init first clears all internal datastructures (including plugin lists)
    /// 1. All assemblies in the plugins directory are loaded into the reflection only context.
    /// 2. The validator checks if all necessary files for each plugin are available.
    /// 3. The validator checks if all declared plugin assemblies can be loaded.
    /// 4. The validator builds the tree of plugin descriptions (dependencies)
    /// 5. The validator checks if there are any cycles in the plugin dependency graph and disables plugin with circular dependencies
    /// 6. The validator checks for each plugin if any dependency is disabled.
    /// 7. All plugins that are not disabled are loaded into the execution context.
    /// 8. Each loaded plugin (all assemblies) is searched for a types that implement IPlugin
    ///    then one instance of each IPlugin type is activated and the OnLoad hook is called.
    /// 9. All types implementing IApplication are discovered
    /// </summary>
    internal void DiscoverAndCheckPlugins() {
      pluginDependencies.Clear();

      IEnumerable<Assembly> reflectionOnlyAssemblies = ReflectionOnlyLoadDlls(PluginDir);
      IEnumerable<PluginDescription> pluginDescriptions = GatherPluginDescriptions(reflectionOnlyAssemblies);
      CheckPluginFiles(pluginDescriptions);

      // check if all plugin assemblies can be loaded
      CheckPluginAssemblies(pluginDescriptions);

      // a full list of plugin descriptions is available now we can build the dependency tree
      BuildDependencyTree(pluginDescriptions);

      // check for dependency cycles
      CheckPluginDependencyCycles(pluginDescriptions);

      // recursively check if all necessary plugins are available and not disabled
      // disable plugins with missing or disabled dependencies
      CheckPluginDependencies(pluginDescriptions);

      // mark all plugins as enabled that were not disabled in CheckPluginFiles, CheckPluginAssemblies, 
      // CheckCircularDependencies, or CheckPluginDependencies
      foreach (var desc in pluginDescriptions)
        if (desc.PluginState != PluginState.Disabled)
          desc.Enable();

      // test full loading (in contrast to reflection only loading) of plugins
      // disables plugins that are not loaded correctly
      LoadPlugins(pluginDescriptions);

      plugins = pluginDescriptions;
      DiscoverApplications();
    }

    private void DiscoverApplications() {
      applications = new List<ApplicationDescription>();

      foreach (IApplication application in GetApplications()) {
        Type appType = application.GetType();
        ApplicationAttribute attr = (from x in appType.GetCustomAttributes(typeof(ApplicationAttribute), false)
                                     select (ApplicationAttribute)x).Single();
        ApplicationDescription info = new ApplicationDescription();
        info.Name = application.Name;
        info.Version = appType.Assembly.GetName().Version;
        info.Description = application.Description;
        info.AutoRestart = attr.RestartOnErrors;
        info.DeclaringAssemblyName = appType.Assembly.GetName().Name;
        info.DeclaringTypeName = appType.Namespace + "." + application.GetType().Name;

        applications.Add(info);
      }
    }

    private static IEnumerable<IApplication> GetApplications() {
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             from t in asm.GetTypes()
             where typeof(IApplication).IsAssignableFrom(t) &&
               !t.IsAbstract && !t.IsInterface && !t.HasElementType
             select (IApplication)Activator.CreateInstance(t);
    }

    private static IEnumerable<Assembly> ReflectionOnlyLoadDlls(string baseDir) {
      List<Assembly> assemblies = new List<Assembly>();
      // recursively load .dll files in subdirectories
      foreach (string dirName in Directory.GetDirectories(baseDir)) {
        assemblies.AddRange(ReflectionOnlyLoadDlls(dirName));
      }
      // try to load each .dll file in the plugin directory into the reflection only context
      foreach (string filename in Directory.GetFiles(baseDir, "*.dll")) {
        try {
          assemblies.Add(Assembly.ReflectionOnlyLoadFrom(filename));
        }
        catch (BadImageFormatException) { } // just ignore the case that the .dll file is not a CLR assembly (e.g. a native dll)
        catch (FileLoadException) { }
        catch (SecurityException) { }
      }
      return assemblies;
    }

    /// <summary>
    /// Checks if all plugin assemblies can be loaded. If an assembly can't be loaded the plugin is disabled.
    /// </summary>
    /// <param name="pluginDescriptions"></param>
    private void CheckPluginAssemblies(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (var desc in pluginDescriptions.Where(x => x.PluginState != PluginState.Disabled)) {
        try {
          foreach (var asm in desc.Assemblies) {
            Assembly.ReflectionOnlyLoadFrom(asm);
          }
        }
        catch (BadImageFormatException) {
          // disable the plugin
          desc.Disable();
        }
        catch (FileNotFoundException) {
          // disable the plugin
          desc.Disable();
        }
        catch (FileLoadException) {
          // disable the plugin
          desc.Disable();
        }
        catch (ArgumentException) {
          // disable the plugin
          desc.Disable();
        }
        catch (SecurityException) {
          // disable the plugin
          desc.Disable();
        }
      }
    }


    // find all types implementing IPlugin in the reflectionOnlyAssemblies and create a list of plugin descriptions
    // the dependencies in the plugin descriptions are not yet set correctly because we need to create
    // the full list of all plugin descriptions first
    private IEnumerable<PluginDescription> GatherPluginDescriptions(IEnumerable<Assembly> assemblies) {
      List<PluginDescription> pluginDescriptions = new List<PluginDescription>();
      foreach (Assembly assembly in assemblies) {
        // GetExportedTypes throws FileNotFoundException when a referenced assembly
        // of the current assembly is missing.
        try {
          // if there is a type that implements IPlugin
          // use AssemblyQualifiedName to compare the types because we can't directly 
          // compare ReflectionOnly types and execution types
          var assemblyPluginDescriptions = from t in assembly.GetExportedTypes()
                                           where !t.IsAbstract && t.GetInterfaces().Any(x => x.AssemblyQualifiedName == typeof(IPlugin).AssemblyQualifiedName)
                                           select GetPluginDescription(t);
          pluginDescriptions.AddRange(assemblyPluginDescriptions);
        }
        // ignore exceptions. Just don't yield a plugin description when an exception is thrown
        catch (FileNotFoundException) {
        }
        catch (FileLoadException) {
        }
        catch (InvalidPluginException) {
        }
      }
      return pluginDescriptions;
    }

    /// <summary>
    /// Extracts plugin information for this type.
    /// Reads plugin name, list and type of files and dependencies of the plugin. This information is necessary for
    /// plugin dependency checking before plugin activation.
    /// </summary>
    /// <param name="t"></param>
    private PluginDescription GetPluginDescription(Type pluginType) {
      // get all attributes of that type
      IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(pluginType);
      List<string> pluginAssemblies = new List<string>();
      List<string> pluginDependencies = new List<string>();
      List<string> pluginFiles = new List<string>();
      string pluginName = null;
      string pluginDescription = null;
      // iterate through all custom attributes and search for attributed that we are interested in 
      foreach (CustomAttributeData attributeData in attributes) {
        if (IsAttributeDataForType(attributeData, typeof(PluginAttribute))) {
          pluginName = (string)attributeData.ConstructorArguments[0].Value;
          if (attributeData.ConstructorArguments.Count() == 2) {
            pluginDescription = (string)attributeData.ConstructorArguments[1].Value;
          } else pluginDescription = pluginName;
        } else if (IsAttributeDataForType(attributeData, typeof(PluginDependencyAttribute))) {
          pluginDependencies.Add((string)attributeData.ConstructorArguments[0].Value);
        } else if (IsAttributeDataForType(attributeData, typeof(PluginFileAttribute))) {
          string pluginFileName = (string)attributeData.ConstructorArguments[0].Value;
          PluginFileType fileType = (PluginFileType)attributeData.ConstructorArguments[1].Value;
          pluginFiles.Add(Path.GetFullPath(Path.Combine(PluginDir, pluginFileName)));
          if (fileType == PluginFileType.Assembly) {
            pluginAssemblies.Add(Path.GetFullPath(Path.Combine(PluginDir, pluginFileName)));
          }
        }
      }

      var buildDates = from attr in CustomAttributeData.GetCustomAttributes(pluginType.Assembly)
                       where IsAttributeDataForType(attr, typeof(AssemblyBuildDateAttribute))
                       select (string)attr.ConstructorArguments[0].Value;

      // minimal sanity check of the attribute values
      if (!string.IsNullOrEmpty(pluginName) &&
          pluginFiles.Count > 0 &&
          pluginAssemblies.Count > 0 &&
          buildDates.Count() == 1) {
        // create a temporary PluginDescription that contains the attribute values
        PluginDescription info = new PluginDescription();
        info.Name = pluginName;
        info.Description = pluginDescription;
        info.Version = pluginType.Assembly.GetName().Version;
        info.BuildDate = DateTime.Parse(buildDates.Single(), System.Globalization.CultureInfo.InvariantCulture);
        info.AddAssemblies(pluginAssemblies);
        info.AddFiles(pluginFiles);

        this.pluginDependencies[info] = pluginDependencies;
        return info;
      } else {
        throw new InvalidPluginException("Invalid metadata in plugin " + pluginType.ToString());
      }
    }

    private static bool IsAttributeDataForType(CustomAttributeData attributeData, Type attributeType) {
      return attributeData.Constructor.DeclaringType.AssemblyQualifiedName == attributeType.AssemblyQualifiedName;
    }

    // builds a dependency tree of all plugin descriptions
    // searches matching plugin descriptions based on the list of dependency names for each plugin
    // and sets the dependencies in the plugin descriptions
    private void BuildDependencyTree(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (var desc in pluginDescriptions) {
        foreach (string pluginName in pluginDependencies[desc]) {
          var matchingDescriptions = pluginDescriptions.Where(x => x.Name == pluginName);
          if (matchingDescriptions.Count() > 0) {
            desc.AddDependency(matchingDescriptions.Single());
          } else {
            // no plugin description that matches the dependency name is available => plugin is disabled
            desc.Disable();
          }
        }
      }
    }

    private void CheckPluginDependencyCycles(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (var plugin in pluginDescriptions) {
        // if the plugin is not disabled anyway check if there are cycles
        if (plugin.PluginState != PluginState.Disabled && HasCycleInDependencies(plugin, plugin.Dependencies)) {
          plugin.Disable();
        }
      }
    }

    private bool HasCycleInDependencies(PluginDescription plugin, IEnumerable<PluginDescription> pluginDependencies) {
      foreach (var dep in pluginDependencies) {
        // if one of the dependencies is the original plugin we found a cycle and can return
        // if the dependency is already disabled we can ignore the cycle detection because we will disable the plugin anyway
        // if following one of the dependencies recursively leads to a cycle then we also return
        if (dep == plugin || dep.PluginState == PluginState.Disabled || HasCycleInDependencies(plugin, dep.Dependencies)) return true;
      }
      // no cycle found and none of the direct and indirect dependencies is disabled
      return false;
    }

    private void CheckPluginDependencies(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (PluginDescription pluginDescription in pluginDescriptions.Where(x => x.PluginState != PluginState.Disabled)) {
        if (IsAnyDependencyDisabled(pluginDescription)) {
          pluginDescription.Disable();
        }
      }
    }


    private bool IsAnyDependencyDisabled(PluginDescription descr) {
      if (descr.PluginState == PluginState.Disabled) return true;
      foreach (PluginDescription dependency in descr.Dependencies) {
        if (IsAnyDependencyDisabled(dependency)) return true;
      }
      return false;
    }

    private void LoadPlugins(IEnumerable<PluginDescription> pluginDescriptions) {
      // load all loadable plugins (all dependencies available) into the execution context
      foreach (var desc in PluginDescriptionIterator.IterateDependenciesBottomUp(pluginDescriptions
                                                                                .Where(x => x.PluginState != PluginState.Disabled))) {
        List<Type> types = new List<Type>();
        foreach (string assembly in desc.Assemblies) {
          var asm = Assembly.LoadFrom(assembly);
          foreach (Type t in asm.GetTypes()) {
            if (typeof(IPlugin).IsAssignableFrom(t)) {
              types.Add(t);
            }
          }
        }

        foreach (Type pluginType in types) {
          if (!pluginType.IsAbstract && !pluginType.IsInterface && !pluginType.HasElementType) {
            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
            plugin.OnLoad();
            OnPluginLoaded(new PluginInfrastructureEventArgs("Plugin loaded", plugin.Name));
          }
        }
        desc.Load();
      }
    }

    // checks if all declared plugin files are actually available and disables plugins with missing files
    private void CheckPluginFiles(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (PluginDescription desc in pluginDescriptions) {
        if (!CheckPluginFiles(desc)) {
          desc.Disable();
        }
      }
    }

    private bool CheckPluginFiles(PluginDescription pluginDescription) {
      foreach (string filename in pluginDescription.Files) {
        if (!FileLiesInDirectory(PluginDir, filename) ||
          !File.Exists(filename)) {
          return false;
        }
      }
      return true;
    }

    private static bool FileLiesInDirectory(string dir, string fileName) {
      var basePath = Path.GetFullPath(dir);
      return Path.GetFullPath(fileName).StartsWith(basePath);
    }

    internal void OnPluginLoaded(PluginInfrastructureEventArgs e) {
      if (PluginLoaded != null)
        PluginLoaded(this, e);
    }

    /// <summary>
    /// Initializes the life time service with an infinite lease time.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    public override object InitializeLifetimeService() {
      return null;
    }
  }
}
