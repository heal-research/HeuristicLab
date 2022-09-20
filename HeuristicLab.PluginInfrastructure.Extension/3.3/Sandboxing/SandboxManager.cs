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
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Sandboxing {
  public static class SandboxManager {
    /// <summary>
    /// Returns a new AppDomain with loaded assemblies/plugins from applicationBase
    /// </summary>    
    public static AppDomain CreateAndInitSandbox(string appDomainName, string applicationBase, string configFilePath) {
      PermissionSet pSet;
      pSet = new PermissionSet(PermissionState.Unrestricted);

      AppDomainSetup setup = new AppDomainSetup();
      setup.PrivateBinPath = applicationBase;
      setup.ApplicationBase = applicationBase;
      setup.ConfigurationFile = configFilePath;

      Type applicationManagerType = typeof(DefaultApplicationManager);
      AppDomain applicationDomain = AppDomain.CreateDomain(appDomainName, null, setup, pSet, null);
      DefaultApplicationManager applicationManager = (DefaultApplicationManager)applicationDomain.CreateInstanceAndUnwrap(applicationManagerType.Assembly.FullName, applicationManagerType.FullName, true, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

      PluginManager pm = new PluginManager(applicationBase);
      pm.DiscoverAndCheckPlugins();
      applicationManager.PrepareApplicationDomain(pm.Applications, pm.Plugins);

      return applicationDomain;
    }
  }
}
