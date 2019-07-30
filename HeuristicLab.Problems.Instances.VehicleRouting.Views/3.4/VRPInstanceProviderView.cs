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
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances.Views;

namespace HeuristicLab.Problems.Instances.VehicleRouting.Views {
  [View("VRP InstanceProvider View")]
  [Content(typeof(IVRPInstanceProvider<>), IsDefaultView = true)]
  public partial class VRPInstanceProviderView<T> : ProblemInstanceProviderView<T> where T : class, IVRPData {

    public VRPInstanceProviderView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      var provider = Content as IVRPInstanceProvider<T>;
      if (provider != null) {
        using (var dialog = new VRPImportDialog(Content.Name)) {
          if (dialog.ShowDialog() == DialogResult.OK) {
            var instance = provider.Import(dialog.VRPFileName, dialog.TourFileName);
            try {
              GenericConsumer.Load(instance as T);
              instancesComboBox.SelectedIndex = -1;
            } catch (Exception ex) {
              MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
            }
          }
        }
      }
    }

    protected override void exportButton_Click(object sender, EventArgs e) {
      var provider = Content as IVRPInstanceProvider<T>;
      if (provider != null) {
        if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
          try {
            provider.Export(GenericExporter.Export(), saveFileDialog.FileName);
          } catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          }
        }
      }
    }
  }
}
