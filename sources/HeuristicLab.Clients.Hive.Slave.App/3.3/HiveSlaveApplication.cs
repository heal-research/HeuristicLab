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

using System.IO;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Clients.Hive.SlaveCore.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.Slave.App {
  [Application("Hive Slave", "Runs the Hive Slave as a HeuristicLab application")]
  internal class HiveSlaveApplication : ApplicationBase {
    private HeuristicLab.Clients.Hive.SlaveCore.Core core;
    public override void Run() {
      CheckWorkingDirectories();

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

    /// <summary>
    /// Normally the configuration file just contains the folder names of the PluginCache and the AppDomain working directory. 
    /// This means that these folders are created in the current directory which is ok for the console client and the windows service. 
    /// For the HL client we can't do that because the plugin infrastructure gets confused when starting HeuristicLab. 
    /// Therefore if there is only a relative path in the config, we change that to the temp path. 
    /// </summary>
    void CheckWorkingDirectories() {
      if (!Path.IsPathRooted(Settings.Default.PluginCacheDir)) {
        Settings.Default.PluginCacheDir = Path.Combine(Path.GetTempPath(), Settings.Default.PluginCacheDir);
        Settings.Default.Save();
      }

      if (!Path.IsPathRooted(Settings.Default.PluginTempBaseDir)) {
        Settings.Default.PluginTempBaseDir = Path.Combine(Path.GetTempPath(), Settings.Default.PluginTempBaseDir);
        Settings.Default.Save();
      }
    }
  }
}
