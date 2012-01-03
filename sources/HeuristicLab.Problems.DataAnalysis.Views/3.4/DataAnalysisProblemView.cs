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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("DataAnalysisProblem View")]
  [Content(typeof(IDataAnalysisProblem), true)]
  public partial class DataAnalysisProblemView : ProblemView {
    public DataAnalysisProblemView() {
      InitializeComponent();
    }

    public new IDataAnalysisProblem Content {
      get { return (IDataAnalysisProblem)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      ImportButton.Enabled = !Locked && !ReadOnly && Content != null;
    }

    private void ImportButton_Click(object sender, System.EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ImportProblemDataFromFile(openFileDialog.FileName);
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
  }
}
