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

using System.Windows.Forms;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  public partial class DataAnalysisImportTypeDialog : Form {

    public string Path {
      get { return ProblemTextBox.Text; }
    }

    public DataAnalysisImportType ImportType {
      get {
        return new DataAnalysisImportType() {
          Shuffle = ShuffleDataCheckbox.Checked,
          Training = TrainingTestTrackBar.Value
        };
      }
    }

    public DataAnalysisImportTypeDialog() {
      InitializeComponent();
    }

    private void TrainingTestTrackBar_ValueChanged(object sender, System.EventArgs e) {
      TrainingLabel.Text = "Training: " + TrainingTestTrackBar.Value + " %";
      TestLabel.Text = "Test: " + (TrainingTestTrackBar.Maximum - TrainingTestTrackBar.Value) + " %";
    }

    private void openFileButton_Click(object sender, System.EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        ProblemTextBox.Text = openFileDialog.FileName;
        OkButton.Enabled = true;
      }
    }
  }
}
