#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Cluster Visualization")]
  [Content(typeof(IClusteringSolution), IsDefaultView = false)]
  public partial class ClusteringSolutionVisualizationView : DataAnalysisSolutionEvaluationView {
    private ViewHost viewHost = new ViewHost();
    private ScatterPlot scatterPlot = new ScatterPlot();

    public new IClusteringSolution Content {
      get { return (IClusteringSolution)base.Content; }
      set { base.Content = value; }
    }

    public ClusteringSolutionVisualizationView() {
      InitializeComponent();
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

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      rangeComboBox.Enabled = Content != null;
    }

    private void UpdateScatterPlot() {
      scatterPlot.Rows.Clear();

      IEnumerable<int> range = null;
      if (rangeComboBox.SelectedIndex == 0) range = Content.ProblemData.TrainingIndices;
      else if (rangeComboBox.SelectedIndex == 1) range = Content.ProblemData.TestIndices;
      else range = Enumerable.Range(0, Content.ProblemData.Dataset.Rows);

      IDictionary<int, Tuple<double, string>> classes = Content.Model.GetClusterValues(Content.ProblemData.Dataset, Enumerable.Range(0, Content.ProblemData.Dataset.Rows))
          .Select((v, i) => new { Row = i, Cluster = (double)v })
          .ToDictionary(x => x.Row, y => Tuple.Create(y.Cluster, "Cluster " + y.Cluster));

      var rows = classes.Values.Select(x => x.Item2).Distinct().ToDictionary(c => c, c => new ScatterPlotDataRow(c, string.Empty, Enumerable.Empty<Point2D<double>>()));

      var reduced = PCAReduce(Content.ProblemData.Dataset, range, Content.ProblemData.AllowedInputVariables);

      int idx = 0;
      foreach (var r in range) {
        rows[classes[r].Item2].Points.Add(new Point2D<double>(reduced[idx, 0], reduced[idx, 1]));
        idx++;
      }

      scatterPlot.Rows.AddRange(rows.Values);
    }

    private static double[,] PCAReduce(Dataset dataset, IEnumerable<int> rows, IEnumerable<string> variables) {
      var instances = rows.ToArray();
      var attributes = variables.ToArray();
      var data = new double[instances.Length, attributes.Length + 1];

      for (int j = 0; j < attributes.Length; j++) {
        int i = 0;
        var values = dataset.GetDoubleValues(attributes[j], instances);
        foreach (var v in values) {
          data[i++, j] = v;
        }
      }
      int info;
      double[] variances;
      var matrix = new double[0, 0];
      alglib.pcabuildbasis(data, instances.Length, attributes.Length, out info, out variances, out matrix);

      var result = new double[instances.Length, 2];
      int r = 0;
      foreach (var inst in instances) {
        int i = 0;
        foreach (var attrib in attributes) {
          double val = dataset.GetDoubleValue(attrib, inst);
          for (int j = 0; j < result.GetLength(1); j++)
            result[r, j] += val * matrix[i, j];
          i++;
        }
        r++;
      }

      return result;
    }

    #region Event Handlers
    private void rangeComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) UpdateScatterPlot();
    }
    #endregion
  }
}
