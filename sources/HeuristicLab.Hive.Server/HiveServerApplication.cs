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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.ServiceModel;
using HeuristicLab.Hive.Server.Interfaces;

namespace HeuristicLab.Hive.Server {
  [ClassInfo(Name = "Hive Server",
      Description = "Server application for the distributed hive engine.",
      AutoRestart = true)]
  class HiveServerApplication : ApplicationBase {

    private bool Startup() {
      return true;
    }

    public override void Run() {

      DiscoveryService discService =
        new DiscoveryService();

      IClientCommunicator[] instances = 
        discService.GetInstances<IClientCommunicator>();

      if (instances.Length > 0) {
        ServiceHost serviceHost =
                new ServiceHost(instances[0].GetType());

        serviceHost.Open();

        Form mainForm = new MainForm(serviceHost.BaseAddresses[0]);
        Application.Run(mainForm);

        serviceHost.Close();
      } else {
        MessageBox.Show("Error - no ClientCommunicator instance");
      }
    }
  }
}
