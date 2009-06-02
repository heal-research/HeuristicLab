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
using System.Security.Permissions;
using System.Security;

namespace HeuristicLab.PluginInfrastructure {
  public class Runner : MarshalByRefObject {

    private Dictionary<string, Assembly> loadedAssemblies;

    public Runner() {
      loadedAssemblies = new Dictionary<string, Assembly>();
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
        if(loadedAssemblies.ContainsKey(args.Name)) {
          return loadedAssemblies[args.Name];
        }
        return null;
      };
    }

    public void LoadPlugins(ICollection<PluginInfo> plugins) {
      //FileIOPermission fileperm = new FileIOPermission(FileIOPermissionAccess.AllAccess, @"C:\Program Files\HeuristicLab 3.0\plugins\");
      //fileperm.Assert();
      foreach(PluginInfo pluginInfo in plugins) {
        foreach(string assemblyName in pluginInfo.Assemblies) {
          Assembly.LoadFrom(assemblyName);
        }
      }
      //CodeAccessPermission.RevertAssert();
      FireOnLoad();
      PluginManager.Manager.LoadedPlugins = plugins;
    }
    /// <summary>
    /// Loads assemblies from a byte array
    /// </summary>
    /// <param name="plugins">bytearray of all assemblies that should be loaded</param>
    public void LoadAssemblies(ICollection<byte[]> assemblies) {
      foreach (byte[] asm in assemblies) {
        Assembly loadedAsm = Assembly.Load(asm);
        RegisterLoadedAssembly(loadedAsm);
      }
    }

    private void RegisterLoadedAssembly(Assembly asm) {
      loadedAssemblies.Add(asm.FullName, asm);
      loadedAssemblies.Add(asm.GetName().Name, asm); // add short name
    }

    public void Run(ApplicationInfo appInfo) {
      IApplication runnablePlugin = (IApplication)Activator.CreateInstance(appInfo.PluginAssembly, appInfo.PluginType).Unwrap();
      try {
        runnablePlugin.Run();
      } catch (Exception e) {
        throw new Exception(String.Format(
          "Unexpected exception caught: \"{0}\"\r\n" +
          "Type: {1}\r\n" +
          "Plugin {2}:\r\n{3}",
          e.Message,
          e.GetType().FullName,
          appInfo.Name,
          e.ToString()));
      }
    }

    private void FireOnLoad() {
      DiscoveryService service = new DiscoveryService();
      Array.ForEach(service.GetInstances<IPlugin>(), p => p.OnLoad());
    }

    // infinite lease time
    /// <summary>
    /// Initializes the life time service with infinite lease time.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    public override object InitializeLifetimeService() {
      return null;
    }
  }
}
