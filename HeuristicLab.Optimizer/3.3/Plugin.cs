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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Optimizer.Properties;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  [Plugin("HeuristicLab.Optimizer", "3.3.16.0")]
  [PluginFile("HeuristicLab.Optimizer-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Clients.Access", "3.3")]
  [PluginDependency("HeuristicLab.Clients.Common", "3.3")]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Core.Views", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Data.Views", "3.3")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Parameters", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Instances", "3.3")]
  public class HeuristicLabOptimizerPlugin : PluginBase {
  }

  public class HeuristicLabOptimizerApplication {
    public void Run(ICommandLineArgument[] args) {
      HeuristicLab.MainForm.WindowsForms.MainForm mainForm = null;

      if (Settings.Default.MainFormType == OptimizerMainFormTypes.DockingMainForm) {
        mainForm = new OptimizerDockingMainForm(typeof(IOptimizerUserInterfaceItemProvider));
      } else if (Settings.Default.MainFormType == OptimizerMainFormTypes.MultipleDocumentMainForm) {
        mainForm = new OptimizerMultipleDocumentMainForm(typeof(IOptimizerUserInterfaceItemProvider));
      } else if (Settings.Default.MainFormType == OptimizerMainFormTypes.SingleDocumentMainForm) {
        mainForm = new OptimizerSingleDocumentMainForm(typeof(IOptimizerUserInterfaceItemProvider));
      }

      if (mainForm != null) {
        if (CheckSavedMainFormSettings()) {
          mainForm.Location = Settings.Default.MainFormLocation;
          mainForm.Size = Settings.Default.MainFormSize;
          mainForm.WindowState = Settings.Default.MainFormWindowState;
        }

        mainForm.FormClosing += (sender, eventArgs) => {
          if (mainForm.WindowState != FormWindowState.Minimized)
            Settings.Default.MainFormWindowState = mainForm.WindowState;
          if (mainForm.WindowState != FormWindowState.Normal) {
            Settings.Default.MainFormLocation = mainForm.RestoreBounds.Location;
            Settings.Default.MainFormSize = mainForm.RestoreBounds.Size;
          } else if (mainForm.WindowState == FormWindowState.Normal) {
            Settings.Default.MainFormLocation = mainForm.Location;
            Settings.Default.MainFormSize = mainForm.Size;
          }
        };


        ClientInformation.InitializeAsync();
        UserInformation.InitializeAsync();

        mainForm.ShowContentInViewHost = true;
        var filesToOpen = args.OfType<OpenArgument>().Select(x => x.Value);
        mainForm.Load += (sender, eventArgs) => FileManager.OpenFiles(filesToOpen);
        Application.Run(mainForm);
      } else {
        MessageBox.Show("Error loading setting for the MainForm Type. Please check your configuration file!", "HeuristicLab", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private bool CheckSavedMainFormSettings() {
      var formArea = new Rectangle(Settings.Default.MainFormLocation, Settings.Default.MainFormSize);
      var screenArea = Screen.FromRectangle(formArea).WorkingArea;
      var overlappingArea = Rectangle.Intersect(formArea, screenArea);
      bool offLimits = overlappingArea.IsEmpty || overlappingArea.Width * overlappingArea.Height < formArea.Width * formArea.Height * 0.25;
      return !formArea.IsEmpty && !offLimits;
    }
  }
}
