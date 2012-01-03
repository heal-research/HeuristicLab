#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;

namespace HeuristicLab.PluginInfrastructure.Sandboxing {
  public static class SandboxManager {

    /// <summary>
    /// Creates an privileged sandbox, meaning that the executed code is fully trusted and permissions are not restricted.
    /// This method is a fall back for trusted users in HeuristicLab Hive. 
    /// </summary>    
    public static AppDomain CreateAndInitPrivilegedSandbox(string appDomainName, string applicationBase, string configFilePath) {
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

    /// <summary>
    /// Creates a sandbox with restricted permissions.
    /// Code that is executed in such an AppDomain is partially-trusted and is not allowed to call or override
    /// methods that require full trust. 
    /// </summary>    
    public static AppDomain CreateAndInitSandbox(string appDomainName, string applicationBase, string configFilePath) {
      PermissionSet pSet;

      pSet = new PermissionSet(PermissionState.None);
      pSet.AddPermission(new SecurityPermission(PermissionState.None));
      pSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
      pSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Infrastructure));
      pSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
      pSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
      //needed for HeuristicLab.Persistence, see DynamicMethod Constructor (String, Type, array<Type []()>[], Type, Boolean)
      pSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlEvidence));
      pSet.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));

      FileIOPermission ioPerm = new FileIOPermission(PermissionState.None);
      //allow path discovery for system drive, needed by HeuristicLab.Persistence: Serializer.BuildTypeCache() -> Assembly.CodeBase
      ioPerm.AddPathList(FileIOPermissionAccess.PathDiscovery, Path.GetPathRoot(Path.GetFullPath(Environment.SystemDirectory)));
      //allow full access to the appdomain's base directory
      ioPerm.AddPathList(FileIOPermissionAccess.AllAccess, applicationBase);
      pSet.AddPermission(ioPerm);

      AppDomainSetup setup = new AppDomainSetup();
      setup.PrivateBinPath = applicationBase;
      setup.ApplicationBase = applicationBase;
      setup.ConfigurationFile = configFilePath;

      Type applicationManagerType = typeof(SandboxApplicationManager);
      AppDomain applicationDomain = AppDomain.CreateDomain(appDomainName, null, setup, pSet, null);
      SandboxApplicationManager applicationManager = (SandboxApplicationManager)applicationDomain.CreateInstanceAndUnwrap(applicationManagerType.Assembly.FullName, applicationManagerType.FullName, true, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

      PluginManager pm = new PluginManager(applicationBase);
      pm.DiscoverAndCheckPlugins();
      applicationManager.PrepareApplicationDomain(pm.Applications, pm.Plugins);

      return applicationDomain;
    }
  }
}
