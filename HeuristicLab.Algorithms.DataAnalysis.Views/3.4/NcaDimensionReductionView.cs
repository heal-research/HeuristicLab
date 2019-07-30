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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Dimension Reduction")]
  [Content(typeof(INcaClassificationSolution), IsDefaultView = false)]
  public partial class NCADimensionReductionView : DataAnalysisSolutionEvaluationView {
    private ViewHost viewHost = new ViewHost();
    private ScatterPlot scatterPlot = new ScatterPlot();

    public new INcaClassificationSolution Content {
      get { return (INcaClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    public NCADimensionReductionView() {
      InitializeComponent();
      messageLabel.Visible = false;
      viewHost.Dock = DockStyle.Fill;
      splitContainer.Panel2.Controls.Add(viewHost);
      rangeComboBox.SelectedIndex = 0;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        scatterPlot.Rows.Clear();
      } else {
        UpdateScatterPlot();
        viewHost.Content = scatterPlot;
      }
    }

    private void UpdateScatterPlot() {
      scatterPlot.Rows.Clear();
      var rows = Content.ProblemData.ClassValues
        .ToDictionary(c => c, c => new ScatterPlotDataRow(Content.ProblemData.GetClassName(c), string.Empty, Enumerable.Empty<Point2D<double>>()));
      foreach (var r in rows.Values)
        r.VisualProperties.PointSize = 5;

      IEnumerable<int> range = null;
      if (rangeComboBox.SelectedIndex == 0) range = Content.ProblemData.TrainingIndices;
      else if (rangeComboBox.SelectedIndex == 1) range = Content.ProblemData.TestIndices;
      else range = Enumerable.Range(0, Content.ProblemData.Dataset.Rows);

      var reduced = Content.Model.Reduce(Content.ProblemData.Dataset, range);

      int idx = 0;
      if (reduced.GetLength(1) == 2) { // last column is the target variable
        foreach (var r in range) {
          var label = Content.ProblemData.Dataset.GetDoubleValue(Content.ProblemData.TargetVariable, r);
          rows[label].Points.Add(new Point2D<double>(reduced[idx++, 0], 1.0));
        }
      } else {
        foreach (var r in range) {
          var label = Content.ProblemData.Dataset.GetDoubleValue(Content.ProblemData.TargetVariable, r);
          rows[label].Points.Add(new Point2D<double>(reduced[idx, 0], reduced[idx, 1]));
          idx++;
        }
      }
      scatterPlot.Rows.AddRange(rows.Values);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      rangeComboBox.Enabled = Content != null && viewHost.Visible;
    }

    #region Event Handlers
    private void rangeComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) UpdateScatterPlot();
    }
    #endregion
  }
}
