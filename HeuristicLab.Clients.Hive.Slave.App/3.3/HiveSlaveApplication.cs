#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.Slave.App {
  [Application("Hive Slave", "Runs the Hive Slave as a HeuristicLab application")]
  internal class HiveSlaveApplication : ApplicationBase {
    private HeuristicLab.Clients.Hive.SlaveCore.Core core;
    public override void Run(ICommandLineArgument[] args) {

      core = new HeuristicLab.Clients.Hive.SlaveCore.Core();
      Thread coreThread = new Thread(core.Start);
      coreThread.IsBackground = true;
      coreThread.Start();

      MainWindow window = new MainWindow();
      window.Content = new SlaveItem();
      Application.ApplicationExit += new System.EventHandler(Application_ApplicationExit);
      Application.Run(window);
    }

    void Application_ApplicationExit(object sender, System.EventArgs e) {
      core.Shutdown();
    }
  }
}
