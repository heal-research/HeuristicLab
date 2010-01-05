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
using System.Text;
using System.Linq;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.PluginInfrastructure.Advanced;
using System.Runtime.InteropServices;
using HeuristicLab.PluginInfrastructure.Starter;
using System.IO;

namespace HeuristicLab {
  static class Program {
    [STAThread]
    static void Main(string[] args) {
      if (args.Length == 0) {  // normal mode
        try {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new StarterForm());
        }
        catch (Exception ex) {
          ShowErrorMessageBox(ex);
        }

      } else {
        var cmd = args[0].ToUpperInvariant();
        string pluginDir = Path.GetFullPath(Application.StartupPath);
        switch (cmd) {
          case "START": {
              if (args.Length != 2) {
                PrintUsage();
              } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StarterForm(args[1]));
              }
              break;
            }
          case "SHOW": {
              InstallationManagerConsole managerConsole = new InstallationManagerConsole(pluginDir);
              managerConsole.Show(args.Skip(1));
              break;
            }
          case "INSTALL": {
              InstallationManagerConsole managerConsole = new InstallationManagerConsole(pluginDir);
              managerConsole.Install(args.Skip(1));
              break;
            }
          case "UPDATE": {
              InstallationManagerConsole managerConsole = new InstallationManagerConsole(pluginDir);
              managerConsole.Update(args.Skip(1));
              break;
            }
          case "REMOVE": {
              InstallationManagerConsole managerConsole = new InstallationManagerConsole(pluginDir);
              managerConsole.Remove(args.Skip(1));
              break;
            }
          default: PrintUsage(); break;
        }
      }
    }

    private static void PrintUsage() {
      Console.WriteLine("Usage: HeuristicLab.exe <command> <args>");
      Console.WriteLine("Commands:");
      Console.WriteLine("\tstart <application name>");
      Console.WriteLine("\tshow <plugin name(s)>");
      Console.WriteLine("\tupdate <plugin name(s)>");
      Console.WriteLine("\tremove <plugin name(s)>");
      Console.WriteLine("\tinstall <plugin name(s)>");
    }

    private static void ShowErrorMessageBox(Exception ex) {
      MessageBox.Show(null,
        BuildErrorMessage(ex),
        "Error - " + ex.GetType().Name,
        MessageBoxButtons.OK,
        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }

    private static string BuildErrorMessage(Exception ex) {
      StringBuilder sb = new StringBuilder();
      sb.Append("Sorry, but something went wrong!\n\n" + ex.Message + "\n\n" + ex.StackTrace);

      while (ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append("\n\n-----\n\n" + ex.Message + "\n\n" + ex.StackTrace);
      }
      return sb.ToString();
    }
  }
}
