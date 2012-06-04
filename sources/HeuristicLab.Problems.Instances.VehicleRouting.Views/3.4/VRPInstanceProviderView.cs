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

using System;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.Instances.Views;

namespace HeuristicLab.Problems.Instances.VehicleRouting.Views {
  [View("VRP InstanceProvider View")]
  [Content(typeof(IProblemInstanceConsumer<IVRPData>), IsDefaultView = true)]
  public partial class VRPInstanceProviderView<T> : ProblemInstanceConsumerViewGeneric<T> where T : class, IVRPData {

    public new IProblemInstanceConsumer<T> Content {
      get { return (IProblemInstanceConsumer<T>)base.Content; }
      set { base.Content = value; }
    }

    public VRPInstanceProviderView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      IVRPInstanceProvider provider = SelectedProvider as IVRPInstanceProvider;
      if (provider != null) {
        using (var dialog = new VRPImportDialog(SelectedProvider.Name)) {
          if (dialog.ShowDialog() == DialogResult.OK) {
            var instance = provider.LoadData(dialog.VRPFileName, dialog.TourFileName);
            try {
              GenericConsumer.Load(instance as T);
            }
            catch (Exception ex) {
              MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
            }
          }
        }
      }
    }
  }
}
