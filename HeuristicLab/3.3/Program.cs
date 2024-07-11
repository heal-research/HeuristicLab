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
using System.IO;
using System.Windows.Forms;
using HeuristicLab.Optimizer;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab {
  class Program {
    [STAThread]
    static void Main(string[] args) {
      Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var arguments = CommandLineArgumentHandling.GetArguments(args);
      string pluginPath = Path.GetFullPath(Application.StartupPath);
      var pluginManager = new PluginManager(pluginPath);
      var splashScreen = new PluginInfrastructure.Starter.SplashScreen(pluginManager, 1000);      
      splashScreen.Show("Loading HeuristicLab...");
      
      pluginManager.DiscoverAndCheckPlugins();

      DefaultApplicationManager applicationManager = new DefaultApplicationManager();
      applicationManager.PrepareApplicationDomain(pluginManager.Plugins);

      var app = new HeuristicLabOptimizerApplication();
      app.Run(arguments);
    }
  }
}
