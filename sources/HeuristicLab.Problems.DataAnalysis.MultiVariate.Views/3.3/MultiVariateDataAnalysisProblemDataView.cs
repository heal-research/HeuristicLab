#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.MultiVariate;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Views {
  [View("Multi-Variate Data-Analysis Problem View")]
  [Content(typeof(MultiVariateDataAnalysisProblemData), true)]
  public partial class MultiVariateDataAnalysisProblemDataView : ParameterizedNamedItemView {
    private OpenFileDialog openFileDialog;
    public new MultiVariateDataAnalysisProblemData Content {
      get { return (MultiVariateDataAnalysisProblemData)base.Content; }
      set { base.Content = value; }
    }

    public MultiVariateDataAnalysisProblemDataView() {
      InitializeComponent();
    }

    private void importButton_Click(object sender, EventArgs e) {
      if (openFileDialog == null) openFileDialog = new OpenFileDialog();

      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ImportFromFile(openFileDialog.FileName);
          importButton.Enabled = false;
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
  }
}
