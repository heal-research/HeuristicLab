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
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab {
  static class Program {
    [STAThread]
    static void Main(string[] args) {
      if (args.Length == 0) {  // normal mode
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
      } else if (args.Length == 1) {  // start specific application
        PluginManager.Manager.Initialize();

        ApplicationInfo app = null;
        foreach (ApplicationInfo info in PluginManager.Manager.InstalledApplications) {
          if (info.Name == args[0])
            app = info;
        }
        if (app == null) {  // application not found
          MessageBox.Show("Cannot start application.\nApplication " + args[0] + " is not installed.\n\nStarting HeuristicLab in normal mode ...",
                          "HeuristicLab",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new MainForm());
        } else {
          PluginManager.Manager.Run(app);
        }
      }
    }
  }
}
