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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Problems.Scheduling.Views {
  public enum SPFormat { Empty, ORLib }

  public sealed partial class SchedulingProblemImportDialog : Form {
    private string spFileName;
    public string SPFileName {
      get { return spFileName; }
    }
    private string optimalScheduleFileName;
    public string OptimalScheduleFileName {
      get { return optimalScheduleFileName; }
    }
    private SPFormat format;
    public SPFormat Format {
      get { return format; }
    }

    public SchedulingProblemImportDialog() {
      InitializeComponent();
      spFileName = string.Empty;
      optimalScheduleFileName = string.Empty;
      format = SPFormat.Empty;
    }

    private void openSchedulingProblemFileButton_Click(object sender, EventArgs e) {
      if (openSchedulingProblemFileDialog.ShowDialog(this) == DialogResult.OK) {
        spFileTextBox.Text = openSchedulingProblemFileDialog.FileName;
        spFileTextBox.Enabled = true;
        spFileName = openSchedulingProblemFileDialog.FileName;
        okButton.Enabled = true;

        optimalScheduleFileTextBox.Text = string.Empty;
        optimalScheduleFileName = string.Empty;

        format = (SPFormat)(openSchedulingProblemFileDialog.FilterIndex);
      }
    }
    private void openOptimalScheduleFileButton_Click(object sender, EventArgs e) {
      if (openOptimalScheduleFileDialog.ShowDialog(this) == DialogResult.OK) {
        optimalScheduleFileTextBox.Text = openOptimalScheduleFileDialog.FileName;
        optimalScheduleFileTextBox.Enabled = true;
        optimalScheduleFileName = openOptimalScheduleFileDialog.FileName;
      }
    }
    private void clearTourFileButton_Click(object sender, EventArgs e) {
      optimalScheduleFileTextBox.Text = string.Empty;
      optimalScheduleFileTextBox.Enabled = false;
      optimalScheduleFileName = string.Empty;
    }

    private void VRPImportDialog_HelpButtonClicked(object sender, CancelEventArgs e) {
      if (MessageBox.Show("Do you want to open the HeuristicLab wiki website?", "Help",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes) {
          System.Diagnostics.Process.Start("http://dev.heuristiclab.com/trac/hl/core/wiki/Scheduling%20Problem");
      }
    }

  }
}
