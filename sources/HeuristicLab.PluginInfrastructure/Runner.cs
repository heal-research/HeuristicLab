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
  internal class Runner : MarshalByRefObject {

    public void LoadPlugins(ICollection<PluginInfo> plugins) {
      //FileIOPermission fileperm = new FileIOPermission(FileIOPermissionAccess.AllAccess, @"C:\Program Files\HeuristicLab 3.0\plugins\");
      //fileperm.Assert();
      foreach(PluginInfo pluginInfo in plugins) {
        foreach(string assemblyName in pluginInfo.Assemblies) {
          Assembly.LoadFrom(assemblyName);
        }
      }
      //CodeAccessPermission.RevertAssert();
      PluginManager.Manager.LoadedPlugins = plugins;
    }

    public void Run(ApplicationInfo appInfo) {
      IApplication runnablePlugin = (IApplication)Activator.CreateInstance(appInfo.PluginAssembly, appInfo.PluginType).Unwrap();
      runnablePlugin.Run();
    }

    // infinite lease time
    public override object InitializeLifetimeService() {
      return null;
    }
  }
}
