#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  public enum VRPFormat { Empty, TSPLib, Solomon, ORLib, LiLim, Cordeau }

  public sealed partial class VRPImportDialog : Form {
    private string vrpFileName;
    public string VRPFileName {
      get { return vrpFileName; }
    }
    private string tourFileName;
    public string TourFileName {
      get { return tourFileName; }
    }
    private VRPFormat format;
    public VRPFormat Format {
      get { return format; }
    }

    public VRPImportDialog() {
      InitializeComponent();
      vrpFileName = string.Empty;
      tourFileName = string.Empty;
      format = VRPFormat.Empty;
    }

    private void openVRPFileButton_Click(object sender, EventArgs e) {
      if (openVRPFileDialog.ShowDialog(this) == DialogResult.OK) {
        tspFileTextBox.Text = openVRPFileDialog.FileName;
        tspFileTextBox.Enabled = true;
        vrpFileName = openVRPFileDialog.FileName;
        okButton.Enabled = true;

        tourFileTextBox.Text = string.Empty;
        tourFileName = string.Empty;

        format = (VRPFormat)(openVRPFileDialog.FilterIndex);
      }
    }
    private void openTourFileButton_Click(object sender, EventArgs e) {
      if (openTourFileDialog.ShowDialog(this) == DialogResult.OK) {
        tourFileTextBox.Text = openTourFileDialog.FileName;
        tourFileTextBox.Enabled = true;
        tourFileName = openTourFileDialog.FileName;
      }
    }
    private void clearTourFileButton_Click(object sender, EventArgs e) {
      tourFileTextBox.Text = string.Empty;
      tourFileTextBox.Enabled = false;
      tourFileName = string.Empty;
    }

    private void VRPImportDialog_HelpButtonClicked(object sender, CancelEventArgs e) {
      if (MessageBox.Show("Do you want to open the HeuristicLab wiki website?", "Help",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes) {
        System.Diagnostics.Process.Start("http://dev.heuristiclab.com/trac/hl/core/wiki/Vehicle%20Routing%20Problem");
      }
    }
  }
}
