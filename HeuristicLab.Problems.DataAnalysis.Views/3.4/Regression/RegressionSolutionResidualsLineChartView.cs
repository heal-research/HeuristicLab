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

using System;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Residuals Line Chart")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionResidualsLineChartView : RegressionSolutionLineChartViewBase, IDataAnalysisSolutionEvaluationView {

    public RegressionSolutionResidualsLineChartView() : base() {
      InitializeComponent();
    }

    protected void CalcResiduals(int[] idx, double[] x) {
      var problemData = Content.ProblemData;
      var target = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, idx).ToArray();
      for (int i = 0; i < idx.Length; i++) {
        if (!double.IsInfinity(target[i]) && !double.IsNaN(target[i]) &&
            !double.IsInfinity(x[i]) && !double.IsNaN(x[i])) {
          x[i] = target[i] - x[i];
        } else {
          x[i] = 0.0;
        }
      }
    }

    protected override void GetTrainingSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.TrainingIndices.ToArray();
      y = Content.EstimatedTrainingValues.ToArray();
      CalcResiduals(idx, y);
    }

    protected override void GetTestSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.TestIndices.ToArray();
      y = Content.EstimatedTestValues.ToArray();
      CalcResiduals(idx, y);
    }

    protected override void GetAllValuesSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.AllIndices.ToArray();
      y = Content.EstimatedValues.ToArray();
      CalcResiduals(idx, y);
    }

    protected override void RedrawChart() {
      base.RedrawChart();
      UpdateSeriesStyle();
    }

    private void UpdateSeriesStyle() {
      if (Content == null) return;

      if (InvokeRequired) {
        Invoke((Action)UpdateSeriesStyle);
        return;
      }

      double[] res;
      int[] idx;
      GetTrainingSeries(out idx, out res);
      chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].YAxisType = AxisType.Secondary;
      chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].ChartType = SeriesChartType.RangeColumn;
      chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.DataBindXY(idx, res.Select(_ => 0.0).ToArray(), res);

      GetTestSeries(out idx, out res);
      chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].YAxisType = AxisType.Secondary;
      chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].ChartType = SeriesChartType.RangeColumn;
      chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.DataBindXY(idx, res.Select(_ => 0.0).ToArray(), res);

      GetAllValuesSeries(out idx, out res);
      chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].YAxisType = AxisType.Secondary;
      chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].ChartType = SeriesChartType.RangeColumn;
      chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Points.DataBindXY(idx, res.Select(_ => 0.0).ToArray(), res);
      ToggleSeriesData(chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME]); // don't show by default
    }

  }
}
