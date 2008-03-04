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

    private Dictionary<PluginInfo, List<string>> pluginDependencies = new Dictionary<PluginInfo, List<string>>();
    private List<PluginInfo> preloadedPluginInfos = new List<PluginInfo>();
    private Dictionary<IPlugin, PluginInfo> pluginInfos = new Dictionary<IPlugin, PluginInfo>();
    private Dictionary<PluginInfo, IPlugin> allPlugins = new Dictionary<PluginInfo, IPlugin>();
    private List<PluginInfo> disabledPlugins = new List<PluginInfo>();
    private string pluginDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir;

    internal event PluginLoadFailedEventHandler MissingPluginFile;
    internal event PluginManagerActionEventHandler PluginAction;

    internal ICollection<PluginInfo> ActivePlugins {
      get {
        List<PluginInfo> list = new List<PluginInfo>();
        foreach(PluginInfo info in allPlugins.Keys) {
          if(!disabledPlugins.Exists(delegate(PluginInfo disabledInfo) { return info.Name == disabledInfo.Name; })) {
            list.Add(info);
          }
        }
        return list;
      }
    }

    internal ICollection<PluginInfo> InstalledPlugins {
      get {
        return new List<PluginInfo>(allPlugins.Keys);
      }
    }

    internal ICollection<PluginInfo> DisabledPlugins {
      get {
        return disabledPlugins;
      }
    }

    private ICollection<ApplicationInfo> applications;
    internal ICollection<ApplicationInfo> InstalledApplications {
      get {
        return applications;
      }
    }

    private IPlugin FindPlugin(PluginInfo plugin) {
      if(allPlugins.ContainsKey(plugin)) {
        return allPlugins[plugin];
      } else return null;
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
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += delegate(object sender, ResolveEventArgs args) {
        try {
          return Assembly.ReflectionOnlyLoad(args.Name);
        } catch(FileLoadException ex) {
          return null;
        }
        };
      allPlugins.Clear();
      disabledPlugins.Clear();
      pluginInfos.Clear();
      pluginsByName.Clear();
      pluginDependencies.Clear();

      List<Assembly> assemblies = ReflectionOnlyLoadDlls();
      CheckAssemblyDependencies(assemblies);
      CheckPluginFiles();
      CheckPluginDependencies();
      LoadPlugins();

      DiscoveryService service = new DiscoveryService();
      IApplication[] apps = service.GetInstances<IApplication>();
      applications = new List<ApplicationInfo>();

      foreach(IApplication application in apps) {
        ApplicationInfo info = new ApplicationInfo();
        info.Name = application.Name;
        info.Version = application.Version;
        info.Description = application.Description;
        info.PluginAssembly = application.GetType().Assembly.GetName().Name;
        info.PluginType = application.GetType().Namespace + "." + application.GetType().Name;

        applications.Add(info);
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
            // if there is a type that implements IPlugin
            if(Array.Exists<Type>(t.GetInterfaces(), delegate(Type iface) {
              // use AssemblyQualifiedName to compare the types because we can't directly 
              // compare ReflectionOnly types and Execution types
              return iface.AssemblyQualifiedName == typeof(IPlugin).AssemblyQualifiedName;
            })) {
              // fetch the attributes of the IPlugin type
              GetPluginAttributeData(t);
            }
          }
        } catch(FileNotFoundException ex) {
          PluginInfo info = new PluginInfo();
          AssemblyName name = assembly.GetName();
          info.Name = name.Name;
          info.Version = name.Version;
          info.Assemblies.Add(assembly.FullName);
          info.Files.Add(assembly.Location);
          info.Message = "File not found: " + ex.FileName;
          disabledPlugins.Add(info);
        } catch(FileLoadException ex) {
          PluginInfo info = new PluginInfo();
          AssemblyName name = assembly.GetName();
          info.Name = name.Name;
          info.Version = name.Version;
          info.Files.Add(assembly.Location);
          info.Assemblies.Add(assembly.FullName);
          info.Message = "Couldn't load file: " + ex.FileName;
          disabledPlugins.Add(info);
        }
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
      List<string> pluginFiles = new List<string>();
      string pluginName = "";
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
          pluginFiles.Add(pluginDir + "/" + (string)filenameArg.TypedValue.Value);
          if((PluginFileType)filetypeArg.TypedValue.Value == PluginFileType.Assembly) {
            pluginAssemblies.Add(pluginDir + "/" + (string)filenameArg.TypedValue.Value);
          }
        }
      }

      // minimal sanity check of the attribute values
      if(pluginName != "" && pluginAssemblies.Count > 0) {
        // create a temporary PluginInfo that contains the attribute values
        PluginInfo info = new PluginInfo();
        info.Name = pluginName;
        info.Version = t.Assembly.GetName().Version;
        info.Assemblies = pluginAssemblies;
        info.Files.AddRange(pluginFiles);
        info.Assemblies.AddRange(pluginAssemblies);
        this.pluginDependencies[info] = pluginDependencies;
        preloadedPluginInfos.Add(info);
      } else {
        throw new InvalidPluginException();
      }
    }

    private void CheckPluginDependencies() {
      foreach(PluginInfo pluginInfo in preloadedPluginInfos) {
        // don't need to check plugins that are already disabled
        if(disabledPlugins.Contains(pluginInfo)) {
          continue;
        }
        visitedDependencies.Clear();
        if(!CheckPluginDependencies(pluginInfo.Name)) {
          PluginInfo matchingInfo = preloadedPluginInfos.Find(delegate(PluginInfo info) { return info.Name == pluginInfo.Name; });
          if(matchingInfo == null) throw new InvalidProgramException(); // shouldn't happen
          foreach(string dependency in pluginDependencies[matchingInfo]) {
            PluginInfo dependencyInfo = new PluginInfo();
            dependencyInfo.Name = dependency;
            pluginInfo.Dependencies.Add(dependencyInfo);
          }

          pluginInfo.Message = "Disabled: missing plugin dependency.";
          disabledPlugins.Add(pluginInfo);
        }
      }
    }

    private List<string> visitedDependencies = new List<string>();
    private bool CheckPluginDependencies(string pluginName) {
      if(!preloadedPluginInfos.Exists(delegate(PluginInfo info) { return pluginName == info.Name; }) ||
        disabledPlugins.Exists(delegate(PluginInfo info) { return pluginName == info.Name; }) ||
        visitedDependencies.Contains(pluginName)) {
        // when the plugin is not available return false;
        return false;
      } else {
        // otherwise check if all dependencies of the plugin are OK 
        // if yes then this plugin is also ok and we store it in the list of loadable plugins

        PluginInfo matchingInfo = preloadedPluginInfos.Find(delegate(PluginInfo info) { return info.Name == pluginName; });
        if(matchingInfo == null) throw new InvalidProgramException(); // shouldn't happen
        foreach(string dependency in pluginDependencies[matchingInfo]) {
          visitedDependencies.Add(pluginName);
          if(CheckPluginDependencies(dependency) == false) {
            // if only one dependency is not available that means that the current plugin also is unloadable
            return false;
          }
          visitedDependencies.Remove(pluginName);
        }
        // all dependencies OK
        return true;
      }
    }


    private Dictionary<string, IPlugin> pluginsByName = new Dictionary<string, IPlugin>();
    private void LoadPlugins() {
      // load all loadable plugins (all dependencies available) into the execution context
      foreach(PluginInfo pluginInfo in preloadedPluginInfos) {
        if(!disabledPlugins.Contains(pluginInfo)) {
          foreach(string assembly in pluginInfo.Assemblies) {
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
          }
        }
      }

      foreach(IPlugin plugin in pluginsByName.Values) {
        PluginInfo pluginInfo = GetPluginInfo(plugin);
        allPlugins.Add(pluginInfo, plugin);
        PluginAction(this, new PluginManagerActionEventArgs(plugin.Name, PluginManagerAction.InitializedPlugin));
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

      PluginInfo preloadedInfo = preloadedPluginInfos.Find(delegate(PluginInfo info) { return info.Name == plugin.Name; });
      foreach(string assembly in preloadedInfo.Assemblies) {
        // always use \ as directory separator (this is necessary for discovery of types in 
        // plugins see DiscoveryService.GetTypes() 
        pluginInfo.Assemblies.Add(assembly.Replace('/', '\\'));
      }
      foreach(string dependency in pluginDependencies[preloadedInfo]) {
        // accumulate the dependencies of each assembly into the dependencies of the whole plugin
        PluginInfo dependencyInfo = GetPluginInfo(pluginsByName[dependency]);
        pluginInfo.Dependencies.Add(dependencyInfo);
      }
      pluginInfos[plugin] = pluginInfo;
      return pluginInfo;
    }

    private void CheckPluginFiles() {
      foreach(PluginInfo plugin in preloadedPluginInfos) {
        if(!CheckPluginFiles(plugin)) {
          plugin.Message = "Disabled: missing plugin file.";
          disabledPlugins.Add(plugin);
        }
      }
    }

    private bool CheckPluginFiles(PluginInfo pluginInfo) {
      foreach(string filename in pluginInfo.Files) {
        if(!File.Exists(filename)) {
          if(MissingPluginFile != null) {
            MissingPluginFile(pluginInfo.Name, filename);
          }
          return false;
        }
      }
      return true;
    }

    // infinite lease time
    public override object InitializeLifetimeService() {
      return null;
    }

    internal void OnDelete(PluginInfo pluginInfo) {
      IPlugin plugin = FindPlugin(pluginInfo);
      if(plugin!=null) plugin.OnDelete();
    }

    internal void OnInstall(PluginInfo pluginInfo) {
      IPlugin plugin = FindPlugin(pluginInfo);
      if(plugin != null) plugin.OnInstall();
    }

    internal void OnPreUpdate(PluginInfo pluginInfo) {
      IPlugin plugin = FindPlugin(pluginInfo);
      if(plugin != null) plugin.OnPreUpdate();
    }

    internal void OnPostUpdate(PluginInfo pluginInfo) {
      IPlugin plugin = FindPlugin(pluginInfo);
      if(plugin != null) plugin.OnPostUpdate();
    }
  }
}
