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
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure {
  internal class Loader : MarshalByRefObject {
    public delegate void PluginLoadedEventHandler(string pluginName);
    public delegate void PluginLoadFailedEventHandler(string pluginName, string args);

    private Dictionary<PluginInfo, IPlugin> activePlugins = new Dictionary<PluginInfo, IPlugin>();
    private Dictionary<PluginInfo, IPlugin> allPlugins = new Dictionary<PluginInfo, IPlugin>();
    private Dictionary<IPlugin, PluginInfo> pluginInfos = new Dictionary<IPlugin, PluginInfo>();

    private Dictionary<string, List<string>> pluginDependencies = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> pluginAssemblies = new Dictionary<string, List<string>>();

    private List<string> loadablePlugins = new List<string>();
    private string pluginDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir;

    internal event PluginLoadFailedEventHandler MissingPluginFile;

    internal event PluginManagerActionEventHandler PluginAction;

    internal PluginInfo[] ActivePlugins {
      get {
        PluginInfo[] plugins = new PluginInfo[activePlugins.Count];
        activePlugins.Keys.CopyTo(plugins, 0);
        return plugins;
      }
    }

    internal List<PluginInfo> InstalledPlugins {
      get {
        return new List<PluginInfo>(allPlugins.Keys);
      }
    }

    private ApplicationInfo[] applications;
    internal ApplicationInfo[] InstalledApplications {
      get {
        return applications;
      }
    }

    private IPlugin FindPlugin(PluginInfo plugin) {
      return activePlugins[plugin];
    }


    /// <summary>
    /// Init first clears all internal datastructures (including plugin lists)
    /// 1. All assemblies in the plugins directory are loaded into the reflection only context.
    /// 2. The loader checks if all dependencies for each assembly are available.
    /// 3. All assemblies for which there are no dependencies missing are loaded into the execution context.
    /// 4. Each loaded assembly is searched for a type that implements IPlugin, then one instance of each IPlugin type is activated
    /// 5. The loader checks if all necessary files for each plugin are available.
    /// 6. The loader builds an acyclic graph of PluginDescriptions (childs are dependencies of a plugin) based on the 
    /// list of assemblies of an plugin and the list of dependencies for each of those assemblies
    /// </summary>
    internal void Init() {
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += delegate(object sender, ResolveEventArgs args) { return Assembly.ReflectionOnlyLoad(args.Name); };
      activePlugins.Clear();
      allPlugins.Clear();
      pluginInfos.Clear();
      pluginsByName.Clear();
      loadablePlugins.Clear();
      pluginDependencies.Clear();
      pluginAssemblies.Clear();

      List<Assembly> assemblies = ReflectionOnlyLoadDlls();
      CheckAssemblyDependencies(assemblies);
      LoadPlugins();
      CheckPluginFiles();

      DiscoveryService service = new DiscoveryService();
      IApplication[] apps = service.GetInstances<IApplication>();
      applications = new ApplicationInfo[apps.Length];

      int i = 0;
      foreach(IApplication application in apps) {
        ApplicationInfo info = new ApplicationInfo();
        info.Name = application.Name;
        info.Version = application.Version;
        info.Description = application.Description;
        info.PluginAssembly = application.GetType().Assembly.GetName().Name;
        info.PluginType = application.GetType().Namespace + "." + application.GetType().Name;

        applications[i++] = info;
      }
    }

    private List<Assembly> ReflectionOnlyLoadDlls() {
      List<Assembly> assemblies = new List<Assembly>();
      // load all installed plugins into the reflection only context
      foreach(String filename in Directory.GetFiles(pluginDir, "*.dll")) {
        assemblies.Add(ReflectionOnlyLoadDll(filename));
      }
      return assemblies;
    }

    private Assembly ReflectionOnlyLoadDll(string filename) {
      return Assembly.ReflectionOnlyLoadFrom(filename);
    }

    private void CheckAssemblyDependencies(List<Assembly> assemblies) {

      foreach(Assembly assembly in assemblies) {

        // GetExportedTypes throws FileNotFoundException when a referenced assembly
        // of the current assembly is missing.
        try {
          Type[] exported = assembly.GetExportedTypes();

          foreach(Type t in exported) {
            // if the type implements IPlugin
            if(Array.Exists<Type>(t.GetInterfaces(), delegate(Type iface) {
              // use AssemblyQualifiedName to compare the types because we can't directly 
              // compare ReflectionOnly types and Execution types
              return iface.AssemblyQualifiedName == typeof(IPlugin).AssemblyQualifiedName;
            })) {
              GetPluginAttributeData(t);
            }

          }
        } catch(FileNotFoundException) {
          // when a referenced assembly cannot be loaded then ignore this assembly in the plugin discovery
          // TASK: add the assembly to some kind of unloadable assemblies list
          // this list could be displayed to the user for diagnosis          
        }
      }

      foreach(string pluginName in this.pluginDependencies.Keys) {
        CheckPluginDependencies(pluginName);
      }
    }

    /// <summary>
    /// Extracts plugin information for this type.
    /// Reads plugin name, list and type of files and dependencies of the plugin. This information is necessary for
    /// plugin dependency checking before plugin activation.
    /// </summary>
    /// <param name="t"></param>
    private void GetPluginAttributeData(Type t) {
      // get all attributes of that type
      IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(t);

      List<string> pluginAssemblies = new List<string>();
      List<string> pluginDependencies = new List<string>();
      string pluginName = "";

      // extract relevant parameters
      // iterate through all custom attributes and search for named arguments that we are interested in 
      foreach(CustomAttributeData attributeData in attributes) {
        List<CustomAttributeNamedArgument> namedArguments = new List<CustomAttributeNamedArgument>(attributeData.NamedArguments);

        // if the current attribute contains a named argument with the name "Name" then extract the plugin name
        CustomAttributeNamedArgument pluginNameArgument = namedArguments.Find(delegate(CustomAttributeNamedArgument arg) {
          return arg.MemberInfo.Name == "Name";
        });
        if(pluginNameArgument.MemberInfo != null) {
          pluginName = (string)pluginNameArgument.TypedValue.Value;
        }

        // if the current attribute contains a named argument with the name "Dependency" then extract the dependency
        // and store it in the list of all dependencies
        CustomAttributeNamedArgument dependencyNameArgument = namedArguments.Find(delegate(CustomAttributeNamedArgument arg) {
          return arg.MemberInfo.Name == "Dependency";
        });

        if(dependencyNameArgument.MemberInfo != null) {
          pluginDependencies.Add((string)dependencyNameArgument.TypedValue.Value);
        }

        // if the current attribute has a named argument "Filename" then find if the argument "Filetype" is also supplied
        // and if the filetype is Assembly then store the name of the assembly in the list of assemblies
        CustomAttributeNamedArgument filenameArg = namedArguments.Find(delegate(CustomAttributeNamedArgument arg) {
          return arg.MemberInfo.Name == "Filename";
        });
        CustomAttributeNamedArgument filetypeArg = namedArguments.Find(delegate(CustomAttributeNamedArgument arg) {
          return arg.MemberInfo.Name == "Filetype";
        });
        if(filenameArg.MemberInfo != null && filetypeArg.MemberInfo != null) {
          if((PluginFileType)filetypeArg.TypedValue.Value == PluginFileType.Assembly) {
            pluginAssemblies.Add(pluginDir + "/" + (string)filenameArg.TypedValue.Value);
          }
        }
      }

      // make sure that we found reasonable values
      if(pluginName != "" && pluginAssemblies.Count > 0) {
        this.pluginDependencies[pluginName] = pluginDependencies;
        this.pluginAssemblies[pluginName] = pluginAssemblies;
      } else {
        throw new InvalidPluginException();
      }
    }

    private bool CheckPluginDependencies(string pluginName) {
      // when we already checked the dependencies of this plugin earlier then just return true
      if(loadablePlugins.Contains(pluginName)) {
        return true;
      } else if(!pluginAssemblies.ContainsKey(pluginName)) {
        // when the plugin is not available return false;
        return false;
      } else {
        // otherwise check if all dependencies of the plugin are OK 
        // if yes then this plugin is also ok and we store it in the list of loadable plugins
        foreach(string dependency in pluginDependencies[pluginName]) {
          if(CheckPluginDependencies(dependency) == false) {
            // if only one dependency is not available that means that the current plugin also is unloadable
            return false;
          }
        }
        // all dependencies OK -> add to loadable list and return true
        loadablePlugins.Add(pluginName);
        return true;
      }
    }


    private Dictionary<string, IPlugin> pluginsByName = new Dictionary<string, IPlugin>();

    private void LoadPlugins() {
      // load all loadable plugins (all dependencies available) into the execution context
      foreach(string plugin in loadablePlugins) {
        {
          foreach(string assembly in pluginAssemblies[plugin]) {
            Assembly.LoadFrom(assembly);
          }
        }
      }

      DiscoveryService service = new DiscoveryService();
      // now search and instantiate an IPlugin type in each loaded assembly
      foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        // don't search for plugins in the PluginInfrastructure
        if(assembly == this.GetType().Assembly)
          continue;
        Type[] availablePluginTypes = service.GetTypes(typeof(IPlugin), assembly);


        foreach(Type pluginType in availablePluginTypes) {
          if(!pluginType.IsAbstract && !pluginType.IsInterface && !pluginType.HasElementType) {
            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
            PluginAction(this, new PluginManagerActionEventArgs(plugin.Name, PluginManagerAction.InitializingPlugin));

            pluginsByName.Add(plugin.Name, plugin);
            PluginInfo pluginInfo = GetPluginInfo(plugin);


            allPlugins.Add(pluginInfo, plugin);
            PluginAction(this, new PluginManagerActionEventArgs(plugin.Name, PluginManagerAction.InitializedPlugin));
          }
        }
      }
    }


    private PluginInfo GetPluginInfo(IPlugin plugin) {
      if(pluginInfos.ContainsKey(plugin)) {
        return pluginInfos[plugin];
      }

      // store the data of the plugin in a description file which can be used without loading the plugin assemblies
      PluginInfo pluginInfo = new PluginInfo();
      pluginInfo.Name = plugin.Name;
      pluginInfo.Version = plugin.Version;
      string baseDir = AppDomain.CurrentDomain.BaseDirectory;

      Array.ForEach<string>(plugin.Files, delegate(string file) {
        string filename = pluginDir + "/" + file;
        // always use \ as the directory separator
        pluginInfo.Files.Add(filename.Replace('/', '\\'));
      });

      // each plugin can have multiple assemlies associated
      // for each assembly of the plugin find the dependencies
      // and get the pluginDescriptions for all dependencies
      foreach(string assembly in pluginAssemblies[plugin.Name]) {
        // always use \ as directory separator (this is necessary for discovery of types in 
        // plugins see DiscoveryService.GetTypes() 
        pluginInfo.Assemblies.Add(assembly.Replace('/', '\\'));

      }
      foreach(string dependency in pluginDependencies[plugin.Name]) {
        // accumulate the dependencies of each assembly into the dependencies of the whole plugin
        PluginInfo dependencyInfo = GetPluginInfo(pluginsByName[dependency]);
        pluginInfo.Dependencies.Add(dependencyInfo);
      }

      pluginInfos[plugin] = pluginInfo;

      return pluginInfo;
    }

    private void CheckPluginFiles() {
      foreach(PluginInfo plugin in allPlugins.Keys) {
        CheckPluginFiles(plugin);
      }
    }

    private bool CheckPluginFiles(PluginInfo pluginInfo) {
      if(activePlugins.ContainsKey(pluginInfo)) {
        return true;
      }
      foreach(PluginInfo dependency in pluginInfo.Dependencies) {
        if(!CheckPluginFiles(dependency)) {
          return false;
        }
      }
      foreach(string filename in pluginInfo.Files) {
        if(!File.Exists(filename)) {
          MissingPluginFile(pluginInfo.Name, filename);
          return false;
        }
      }

      activePlugins.Add(pluginInfo, allPlugins[pluginInfo]);
      return true;
    }

    // infinite lease time
    public override object InitializeLifetimeService() {
      return null;
    }

    internal void OnDelete(PluginInfo pluginInfo) {
      FindPlugin(pluginInfo).OnDelete();
    }

    internal void OnInstall(PluginInfo pluginInfo) {
      FindPlugin(pluginInfo).OnInstall();
    }

    internal void OnPreUpdate(PluginInfo pluginInfo) {
      FindPlugin(pluginInfo).OnPreUpdate();
    }

    internal void OnPostUpdate(PluginInfo pluginInfo) {
      FindPlugin(pluginInfo).OnPostUpdate();
    }
  }
}
