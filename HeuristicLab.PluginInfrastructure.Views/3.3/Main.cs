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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Starter;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Static class that contains the main entry point of the plugin infrastructure.
  /// </summary>
  public static class Main {
    /// <summary>
    /// Main entry point of the plugin infrastructure. Loads the starter form.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Run(string[] args) {
      if ((!FrameworkVersionErrorDialog.NET4_5Installed && !FrameworkVersionErrorDialog.MonoInstalled)
        || (FrameworkVersionErrorDialog.MonoInstalled && !FrameworkVersionErrorDialog.MonoCorrectVersionInstalled)) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrameworkVersionErrorDialog());
      } else {
        try {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new StarterForm(args));
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(ex);
        }
      }
    }
  }
}
