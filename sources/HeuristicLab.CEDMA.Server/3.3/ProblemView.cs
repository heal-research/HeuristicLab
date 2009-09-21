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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using System.Diagnostics;
using HeuristicLab.CEDMA.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class ProblemView : ViewBase {
    private Dataset dataset;

    public ProblemView(Dataset dataset) {
      this.dataset = dataset;
      InitializeComponent();
      datasetView.Dataset = dataset;
      dataset.Changed += (sender, args) => UpdateControls();
      UpdateControls();
    }

    private void importButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        DatasetParser parser = new DatasetParser();
        bool success = false;
        try {
          try {
            parser.Import(openFileDialog.FileName, true);
            success = true;
          }
          catch (DataFormatException ex) {
            ShowWarningMessageBox(ex);
            // not possible to parse strictly => clear and try to parse non-strict
            parser.Reset();
            parser.Import(openFileDialog.FileName, false);
            success = true;
          }
        }
        catch (DataFormatException ex) {
          // if the non-strict parsing also failed then show the exception
          ShowErrorMessageBox(ex);
        }
        if (success) {
          dataset.Rows = parser.Rows;
          dataset.Columns = parser.Columns;
          dataset.Name = parser.ProblemName;
          dataset.Samples = new double[dataset.Rows * dataset.Columns];
          Array.Copy(parser.Samples, dataset.Samples, dataset.Columns * dataset.Rows);
          datasetView.Dataset = dataset;

          for (int i = 0; i < parser.VariableNames.Length; i++) {
            dataset.SetVariableName(i, parser.VariableNames[i]);
          }


          Refresh();
        }
      }
    }

    private void ShowWarningMessageBox(Exception ex) {
      MessageBox.Show(ex.Message,
                      "Warning - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
    }
    private void ShowErrorMessageBox(Exception ex) {
      MessageBox.Show(BuildErrorMessage(ex),
                      "Error - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
    private string BuildErrorMessage(Exception ex) {
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
