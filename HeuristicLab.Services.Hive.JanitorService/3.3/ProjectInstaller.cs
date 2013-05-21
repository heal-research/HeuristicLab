#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;


namespace HeuristicLab.Services.Hive.JanitorService {
  [RunInstaller(true)]
  public partial class ProjectInstaller : System.Configuration.Install.Installer {
    public ProjectInstaller() {
      InitializeComponent();
    }

    protected override void OnBeforeUninstall(IDictionary savedState) {
      base.OnBeforeUninstall(savedState);

      //try to shutdown the service before uninstalling it
      using (var serviceController = new ServiceController(this.serviceInstaller1.ServiceName, Environment.MachineName)) {
        try {
          serviceController.Stop();
        }
        catch { }
      }
    }

    protected override void OnAfterInstall(IDictionary savedState) {
      base.OnAfterInstall(savedState);

      //try to start the service after installation
      using (var serviceController = new ServiceController(this.serviceInstaller1.ServiceName, Environment.MachineName)) {
        try {
          serviceController.Start();
        }
        catch { }
      }
    }

  }
}
