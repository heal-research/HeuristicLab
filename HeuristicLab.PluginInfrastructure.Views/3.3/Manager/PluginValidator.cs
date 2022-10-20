#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;


namespace HeuristicLab.PluginInfrastructure.Manager {
  /// <summary>
  /// Discovers all installed plugins in the plugin directory. Checks correctness of plugin meta-data and if
  /// all plugin files are available and checks plugin dependencies. 
  /// </summary>
  internal sealed class PluginValidator : MarshalByRefObject {
    // private class to store plugin dependency declarations while reflecting over plugins
    private class PluginDependency {
      public string Name { get; private set; }
      public Version Version { get; private set; }

      public PluginDependency(string name, Version version) {
        this.Name = name;
        this.Version = version;
      }
    }


    internal event EventHandler<PluginInfrastructureEventArgs> PluginLoaded;

    private Dictionary<PluginDescription, IEnumerable<PluginDependency>> pluginDependencies;

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
      this.pluginDependencies = new Dictionary<PluginDescription, IEnumerable<PluginDependency>>();

      // ReflectionOnlyAssemblyResolveEvent must be handled because we load assemblies from the plugin path 
      // (which is not listed in the default assembly lookup locations)
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolveEventHandler;
    }

    private Dictionary<string, Assembly> reflectionOnlyAssemblies = new Dictionary<string, Assembly>();
    private Assembly ReflectionOnlyAssemblyResolveEventHandler(object sender, ResolveEventArgs args) {
      if (reflectionOnlyAssemblies.ContainsKey(args.Name))
        return reflectionOnlyAssemblies[args.Name];
      else
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

      // 1st time recursively check if all necessary plugins are available and not disabled
      // disable plugins with missing or disabled dependencies 
      // to prevent that plugins with missing dependencies are loaded into the execution context
      // in the next step
      CheckPluginDependencies(pluginDescriptions);

      // test full loading (in contrast to reflection only loading) of plugins
      // disables plugins that are not loaded correctly
      CheckExecutionContextLoad(pluginDescriptions);

      // 2nd time recursively check if all necessary plugins have been loaded successfully and not disabled
      // disable plugins with for which dependencies could not be loaded successfully
      CheckPluginDependencies(pluginDescriptions);

      // mark all plugins as enabled that were not disabled in CheckPluginFiles, CheckPluginAssemblies, 
      // CheckCircularDependencies, CheckPluginDependencies and CheckExecutionContextLoad
      foreach (var desc in pluginDescriptions)
        if (desc.PluginState != PluginState.Disabled)
          desc.Enable();

      // load the enabled plugins
      LoadPlugins(pluginDescriptions);

      plugins = pluginDescriptions;
      DiscoverApplications(pluginDescriptions);
    }

    private void DiscoverApplications(IEnumerable<PluginDescription> pluginDescriptions) {
      applications = new List<ApplicationDescription>();
      foreach (IApplication application in GetApplications(pluginDescriptions)) {
        Type appType = application.GetType();
        ApplicationAttribute attr = (from x in appType.GetCustomAttributes(typeof(ApplicationAttribute), false)
                                     select (ApplicationAttribute)x).Single();
        ApplicationDescription info = new ApplicationDescription();
        PluginDescription declaringPlugin = GetDeclaringPlugin(appType, pluginDescriptions);
        info.Name = application.Name;
        info.Version = declaringPlugin.Version;
        info.Description = application.Description;
        info.AutoRestart = attr.RestartOnErrors;
        info.DeclaringAssemblyName = appType.Assembly.GetName().Name;
        info.DeclaringTypeName = appType.Namespace + "." + application.GetType().Name;

        applications.Add(info);
      }
    }

    private static IEnumerable<IApplication> GetApplications(IEnumerable<PluginDescription> pluginDescriptions) {
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             from t in asm.GetTypes()
             where typeof(IApplication).IsAssignableFrom(t) &&
               !t.IsAbstract && !t.IsInterface && !t.HasElementType
             where GetDeclaringPlugin(t, pluginDescriptions).PluginState != PluginState.Disabled
             select (IApplication)Activator.CreateInstance(t);
    }

    private IEnumerable<Assembly> ReflectionOnlyLoadDlls(string baseDir) {
      List<Assembly> assemblies = new List<Assembly>();
      // recursively load .dll files in subdirectories
      foreach (string dirName in Directory.GetDirectories(baseDir)) {
        assemblies.AddRange(ReflectionOnlyLoadDlls(dirName));
      }
      // try to load each .dll file in the plugin directory into the reflection only context
      foreach (string filename in Directory.GetFiles(baseDir, "*.dll").Union(Directory.GetFiles(baseDir, "*.exe"))) {
        try {
          Assembly asm = Assembly.ReflectionOnlyLoadFrom(filename);
          RegisterLoadedAssembly(asm);
          assemblies.Add(asm);
        }
        catch (BadImageFormatException) { } // just ignore the case that the .dll file is not a CLR assembly (e.g. a native dll)
        catch (FileLoadException) { }
        catch (SecurityException) { }
        catch (ReflectionTypeLoadException) { } // referenced assemblies are missing
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
          var missingAssemblies = new List<string>();
          foreach (var asmLocation in desc.AssemblyLocations) {
            // the assembly must have been loaded in ReflectionOnlyDlls
            // so we simply determine the name of the assembly and try to find it in the cache of loaded assemblies
            var asmName = AssemblyName.GetAssemblyName(asmLocation);
            if (!reflectionOnlyAssemblies.ContainsKey(asmName.FullName)) {
              missingAssemblies.Add(asmName.FullName);
            }
          }
          if (missingAssemblies.Count > 0) {
            StringBuilder errorStrBuiler = new StringBuilder();
            errorStrBuiler.AppendLine("Missing assemblies:");
            foreach (string missingAsm in missingAssemblies) {
              errorStrBuiler.AppendLine(missingAsm);
            }
            desc.Disable(errorStrBuiler.ToString());
          }
        }
        catch (BadImageFormatException ex) {
          // disable the plugin
          desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "BadImageFormatException: " + ex.Message);
        }
        catch (FileNotFoundException ex) {
          // disable the plugin
          desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "FileNotFoundException: " + ex.Message);
        }
        catch (FileLoadException ex) {
          // disable the plugin
          desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "FileLoadException: " + ex.Message);
        }
        catch (ArgumentException ex) {
          // disable the plugin
          desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "ArgumentException: " + ex.Message);
        }
        catch (SecurityException ex) {
          // disable the plugin
          desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "SecurityException: " + ex.Message);
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
        catch (TypeLoadException) {
        }
        catch (MissingMemberException) {
        }
      }
      return pluginDescriptions;
    }

    /// <summary>
    /// Extracts plugin information for this type.
    /// Reads plugin name, list and type of files and dependencies of the plugin. This information is necessary for
    /// plugin dependency checking before plugin activation.
    /// </summary>
    /// <param name="pluginType"></param>
    private PluginDescription GetPluginDescription(Type pluginType) {

      string pluginName, pluginDescription, pluginVersion;
      string contactName, contactAddress;
      GetPluginMetaData(pluginType, out pluginName, out pluginDescription, out pluginVersion);
      GetPluginContactMetaData(pluginType, out contactName, out contactAddress);
      var pluginFiles = GetPluginFilesMetaData(pluginType);
      var pluginDependencies = GetPluginDependencyMetaData(pluginType);

      // minimal sanity check of the attribute values
      if (!string.IsNullOrEmpty(pluginName) &&
          pluginFiles.Count() > 0 &&                                 // at least one file
          pluginFiles.Any(f => f.Type == PluginFileType.Assembly)) { // at least one assembly
        // create a temporary PluginDescription that contains the attribute values
        PluginDescription info = new PluginDescription();
        info.Name = pluginName;
        info.Description = pluginDescription;
        info.Version = new Version(pluginVersion);
        info.ContactName = contactName;
        info.ContactEmail = contactAddress;
        info.LicenseText = ReadLicenseFiles(pluginFiles);
        info.AddFiles(pluginFiles);

        this.pluginDependencies[info] = pluginDependencies;
        return info;
      } else {
        throw new InvalidPluginException("Invalid metadata in plugin " + pluginType.ToString());
      }
    }

    private static string ReadLicenseFiles(IEnumerable<PluginFile> pluginFiles) {
      // combine the contents of all plugin files 
      var licenseFiles = from file in pluginFiles
                         where file.Type == PluginFileType.License
                         select file;
      if (licenseFiles.Count() == 0) return string.Empty;
      StringBuilder licenseTextBuilder = new StringBuilder();
      licenseTextBuilder.AppendLine(File.ReadAllText(licenseFiles.First().Name));
      foreach (var licenseFile in licenseFiles.Skip(1)) {
        licenseTextBuilder.AppendLine().AppendLine(); // leave some empty space between multiple license files
        licenseTextBuilder.AppendLine(File.ReadAllText(licenseFile.Name));
      }
      return licenseTextBuilder.ToString();
    }

    private static IEnumerable<PluginDependency> GetPluginDependencyMetaData(Type pluginType) {
      // get all attributes of type PluginDependency
      var dependencyAttributes = from attr in CustomAttributeData.GetCustomAttributes(pluginType)
                                 where IsAttributeDataForType(attr, typeof(PluginDependencyAttribute))
                                 select attr;

      foreach (var dependencyAttr in dependencyAttributes) {
        string name = (string)dependencyAttr.ConstructorArguments[0].Value;
        Version version = new Version("0.0.0.0"); // default version
        // check if version is given for now
        // later when the constructor of PluginDependencyAttribute with only one argument has been removed
        // this conditional can be removed as well
        if (dependencyAttr.ConstructorArguments.Count > 1) {
          try {
            version = new Version((string)dependencyAttr.ConstructorArguments[1].Value); // might throw FormatException
          }
          catch (FormatException ex) {
            throw new InvalidPluginException("Invalid version format of dependency " + name + " in plugin " + pluginType.ToString(), ex);
          }
        }
        yield return new PluginDependency(name, version);
      }
    }

    private static void GetPluginContactMetaData(Type pluginType, out string contactName, out string contactAddress) {
      // get attribute of type ContactInformation if there is any
      var contactInfoAttribute = (from attr in CustomAttributeData.GetCustomAttributes(pluginType)
                                  where IsAttributeDataForType(attr, typeof(ContactInformationAttribute))
                                  select attr).SingleOrDefault();

      if (contactInfoAttribute != null) {
        contactName = (string)contactInfoAttribute.ConstructorArguments[0].Value;
        contactAddress = (string)contactInfoAttribute.ConstructorArguments[1].Value;
      } else {
        contactName = string.Empty;
        contactAddress = string.Empty;
      }
    }

    // not static because we need the PluginDir property
    private IEnumerable<PluginFile> GetPluginFilesMetaData(Type pluginType) {
      // get all attributes of type PluginFileAttribute
      var pluginFileAttributes = from attr in CustomAttributeData.GetCustomAttributes(pluginType)
                                 where IsAttributeDataForType(attr, typeof(PluginFileAttribute))
                                 select attr;
      foreach (var pluginFileAttribute in pluginFileAttributes) {
        string pluginFileName = (string)pluginFileAttribute.ConstructorArguments[0].Value;
        PluginFileType fileType = (PluginFileType)pluginFileAttribute.ConstructorArguments[1].Value;
        yield return new PluginFile(Path.GetFullPath(Path.Combine(PluginDir, pluginFileName)), fileType);
      }
    }

    private static void GetPluginMetaData(Type pluginType, out string pluginName, out string pluginDescription, out string pluginVersion) {
      // there must be a single attribute of type PluginAttribute
      var pluginMetaDataAttr = (from attr in CustomAttributeData.GetCustomAttributes(pluginType)
                                where IsAttributeDataForType(attr, typeof(PluginAttribute))
                                select attr).Single();

      pluginName = (string)pluginMetaDataAttr.ConstructorArguments[0].Value;

      // default description and version
      pluginVersion = "0.0.0.0";
      pluginDescription = string.Empty;
      if (pluginMetaDataAttr.ConstructorArguments.Count() == 2) {
        // if two arguments are given the second argument is the version
        pluginVersion = (string)pluginMetaDataAttr.ConstructorArguments[1].Value;
      } else if (pluginMetaDataAttr.ConstructorArguments.Count() == 3) {
        // if three arguments are given the second argument is the description and the third is the version
        pluginDescription = (string)pluginMetaDataAttr.ConstructorArguments[1].Value;
        pluginVersion = (string)pluginMetaDataAttr.ConstructorArguments[2].Value;
      }
    }

    private static bool IsAttributeDataForType(CustomAttributeData attributeData, Type attributeType) {
      return attributeData.Constructor.DeclaringType.AssemblyQualifiedName == attributeType.AssemblyQualifiedName;
    }

    // builds a dependency tree of all plugin descriptions
    // searches matching plugin descriptions based on the list of dependency names for each plugin
    // and sets the dependencies in the plugin descriptions
    private void BuildDependencyTree(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (var desc in pluginDescriptions.Where(x => x.PluginState != PluginState.Disabled)) {
        var missingDependencies = new List<PluginDependency>();
        foreach (var dependency in pluginDependencies[desc]) {
          var matchingDescriptions = from availablePlugin in pluginDescriptions
                                     where availablePlugin.PluginState != PluginState.Disabled
                                     where availablePlugin.Name == dependency.Name
                                     where IsCompatiblePluginVersion(availablePlugin.Version, dependency.Version)
                                     select availablePlugin;
          if (matchingDescriptions.Count() > 0) {
            desc.AddDependency(matchingDescriptions.Single());
          } else {
            missingDependencies.Add(dependency);
          }
        }
        // no plugin description that matches the dependencies are available => plugin is disabled
        if (missingDependencies.Count > 0) {
          StringBuilder errorStrBuilder = new StringBuilder();
          errorStrBuilder.AppendLine("Missing dependencies:");
          foreach (var missingDep in missingDependencies) {
            errorStrBuilder.AppendLine(missingDep.Name + " " + missingDep.Version);
          }
          desc.Disable(errorStrBuilder.ToString());
        }
      }
    }

    /// <summary>
    /// Checks if version <paramref name="available"/> is compatible to version <paramref name="requested"/>.
    /// Note: the compatibility relation is not bijective.
    /// Compatibility rules:
    ///  * major and minor number must be the same
    ///  * build and revision number of <paramref name="available"/> must be larger or equal to <paramref name="requested"/>.
    /// </summary>
    /// <param name="available">The available version which should be compared to <paramref name="requested"/>.</param>
    /// <param name="requested">The requested version that must be matched.</param>
    /// <returns></returns>
    private static bool IsCompatiblePluginVersion(Version available, Version requested) {
      // this condition must be removed after all plugins have been updated to declare plugin and dependency versions
      if (
        (requested.Major == 0 && requested.Minor == 0) ||
        (available.Major == 0 && available.Minor == 0)) return true;
      return
        available.Major == requested.Major &&
        available.Minor == requested.Minor &&
        available.Build >= requested.Build &&
        available.Revision >= requested.Revision;
    }

    private void CheckPluginDependencyCycles(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (var plugin in pluginDescriptions) {
        // if the plugin is not disabled check if there are cycles
        if (plugin.PluginState != PluginState.Disabled && HasCycleInDependencies(plugin, plugin.Dependencies)) {
          plugin.Disable("Dependency graph has a cycle.");
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
        List<PluginDescription> disabledPlugins = new List<PluginDescription>();
        if (IsAnyDependencyDisabled(pluginDescription, disabledPlugins)) {
          StringBuilder errorStrBuilder = new StringBuilder();
          errorStrBuilder.AppendLine("Dependencies are disabled:");
          foreach (var disabledPlugin in disabledPlugins) {
            errorStrBuilder.AppendLine(disabledPlugin.Name + " " + disabledPlugin.Version);
          }
          pluginDescription.Disable(errorStrBuilder.ToString());
        }
      }
    }


    private bool IsAnyDependencyDisabled(PluginDescription descr, List<PluginDescription> disabledPlugins) {
      if (descr.PluginState == PluginState.Disabled) {
        disabledPlugins.Add(descr);
        return true;
      }
      foreach (PluginDescription dependency in descr.Dependencies) {
        IsAnyDependencyDisabled(dependency, disabledPlugins);
      }
      return disabledPlugins.Count > 0;
    }

    // tries to load all plugin assemblies into the execution context
    // if an assembly of a plugin cannot be loaded the plugin is disabled
    private void CheckExecutionContextLoad(IEnumerable<PluginDescription> pluginDescriptions) {
      // load all loadable plugins (all dependencies available) into the execution context
      foreach (var desc in PluginDescriptionIterator.IterateDependenciesBottomUp(pluginDescriptions
                                                                                .Where(x => x.PluginState != PluginState.Disabled))) {
        // store the assembly names so that we can later retrieve the assemblies loaded in the appdomain by name
        var assemblyNames = new List<string>();
        foreach (string assemblyLocation in desc.AssemblyLocations) {
          if (desc.PluginState != PluginState.Disabled) {
            try {
              string assemblyName = (from assembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                                     where string.Equals(Path.GetFullPath(assembly.Location), Path.GetFullPath(assemblyLocation), StringComparison.CurrentCultureIgnoreCase)
                                     select assembly.FullName).Single();
              // now load the assemblies into the execution context  
              // this can still lead to an exception
              // even when the assemby was successfully loaded into the reflection only context before 
              // when loading the assembly using it's assemblyName it can be loaded from a different location than before (e.g. the GAC)
              Assembly.Load(assemblyName);
              assemblyNames.Add(assemblyName);
            }
            catch (BadImageFormatException) {
              desc.Disable(Path.GetFileName(assemblyLocation) + " is not a valid assembly.");
            }
            catch (FileLoadException) {
              desc.Disable("Can't load file " + Path.GetFileName(assemblyLocation));
            }
            catch (FileNotFoundException) {
              desc.Disable("File " + Path.GetFileName(assemblyLocation) + " is missing.");
            }
            catch (SecurityException) {
              desc.Disable("File " + Path.GetFileName(assemblyLocation) + " can't be loaded because of security constraints.");
            }
            catch (NotSupportedException ex) {
              // disable the plugin
              desc.Disable("Problem while loading plugin assemblies:" + Environment.NewLine + "NotSupportedException: " + ex.Message);
            }
          }
        }
        desc.AssemblyNames = assemblyNames;
      }
    }

    // assumes that all plugin assemblies have been loaded into the execution context via CheckExecutionContextLoad
    // for each enabled plugin:
    // calls OnLoad method of the plugin 
    // and raises the PluginLoaded event
    private void LoadPlugins(IEnumerable<PluginDescription> pluginDescriptions) {
      List<Assembly> assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
      foreach (var desc in pluginDescriptions) {
        if (desc.PluginState == PluginState.Enabled) {
          // cannot use ApplicationManager to retrieve types because it is not yet instantiated
          foreach (string assemblyName in desc.AssemblyNames) {
            var asm = (from assembly in assemblies
                       where assembly.FullName == assemblyName
                       select assembly)
                      .SingleOrDefault();
            if (asm == null) throw new InvalidPluginException("Could not load assembly " + assemblyName + " for plugin " + desc.Name);
            foreach (Type pluginType in asm.GetTypes()) {
              if (typeof(IPlugin).IsAssignableFrom(pluginType) && !pluginType.IsAbstract && !pluginType.IsInterface && !pluginType.HasElementType) {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                plugin.OnLoad();
                OnPluginLoaded(new PluginInfrastructureEventArgs(desc));
              }
            }
          } // end foreach assembly in plugin
          desc.Load();
        }
      } // end foreach plugin description
    }

    // checks if all declared plugin files are actually available and disables plugins with missing files
    private void CheckPluginFiles(IEnumerable<PluginDescription> pluginDescriptions) {
      foreach (PluginDescription desc in pluginDescriptions) {
        IEnumerable<string> missingFiles;
        if (ArePluginFilesMissing(desc, out missingFiles)) {
          StringBuilder errorStrBuilder = new StringBuilder();
          errorStrBuilder.AppendLine("Missing files:");
          foreach (string fileName in missingFiles) {
            errorStrBuilder.AppendLine(fileName);
          }
          desc.Disable(errorStrBuilder.ToString());
        }
      }
    }

    private bool ArePluginFilesMissing(PluginDescription pluginDescription, out IEnumerable<string> missingFiles) {
      List<string> missing = new List<string>();
      foreach (string filename in pluginDescription.Files.Select(x => x.Name)) {
        if (!FileLiesInDirectory(PluginDir, filename) ||
          !File.Exists(filename)) {
          missing.Add(filename);
        }
      }
      missingFiles = missing;
      return missing.Count > 0;
    }

    private static bool FileLiesInDirectory(string dir, string fileName) {
      var basePath = Path.GetFullPath(dir);
      return Path.GetFullPath(fileName).StartsWith(basePath);
    }

    private static PluginDescription GetDeclaringPlugin(Type appType, IEnumerable<PluginDescription> plugins) {
      return (from p in plugins
              from asmLocation in p.AssemblyLocations
              where Path.GetFullPath(asmLocation).Equals(Path.GetFullPath(appType.Assembly.Location), StringComparison.CurrentCultureIgnoreCase)
              select p).Single();
    }

    // register assembly in the assembly cache for the ReflectionOnlyAssemblyResolveEvent
    private void RegisterLoadedAssembly(Assembly asm) {
      if (reflectionOnlyAssemblies.ContainsKey(asm.FullName) || reflectionOnlyAssemblies.ContainsKey(asm.GetName().Name)) {
        throw new ArgumentException("An assembly with the name " + asm.GetName().Name + " has been registered already.", "asm");
      }
      reflectionOnlyAssemblies.Add(asm.FullName, asm);
      reflectionOnlyAssemblies.Add(asm.GetName().Name, asm); // add short name
    }

    private void OnPluginLoaded(PluginInfrastructureEventArgs e) {
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
