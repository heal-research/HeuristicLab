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

namespace HeuristicLab.Problems.Instances.TSPLIB.Views {
  [View("TSPLIB TSP InstanceProvider View")]
  [Content(typeof(IProblemInstanceConsumer<TSPData>), IsDefaultView = true)]
  public partial class TSPLIBTSPInstanceProviderView : ProblemInstanceConsumerViewGeneric<TSPData> {
    public new IProblemInstanceConsumer<TSPData> Content {
      get { return (IProblemInstanceConsumer<TSPData>)base.Content; }
      set { base.Content = value; }
    }

    public TSPLIBTSPInstanceProviderView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      TSPLIBTSPInstanceProvider provider = SelectedProvider as TSPLIBTSPInstanceProvider;
      if (provider != null) {
        using (var dialog = new TSPLIBImportDialog()) {
          if (dialog.ShowDialog() == DialogResult.OK) {
            var instance = provider.LoadData(dialog.TSPFileName, dialog.TourFileName, dialog.Quality);
            try {
              GenericConsumer.Load(instance);
            }
            catch (Exception ex) {
              MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
            }
          }
        }
      } else {
        base.importButton_Click(sender, e);
      }
    }
  }
}
