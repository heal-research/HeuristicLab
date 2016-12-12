#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("Line Chart (residuals)")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionResidualsLineChartView : RegressionSolutionLineChartView, IDataAnalysisSolutionEvaluationView {


    public RegressionSolutionResidualsLineChartView()
      : base() {
      InitializeComponent();
    }

    protected override void GetTrainingSeries(out int[] x, out double[] y) {
      base.GetTrainingSeries(out x, out y);
      var problemData = Content.ProblemData;
      var target = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, x).ToArray();
      for (int i = 0; i < x.Length; i++) {
        y[i] -= target[i];
      }
    }

    protected override void GetTestSeries(out int[] x, out double[] y) {
      base.GetTestSeries(out x, out y);
      var problemData = Content.ProblemData;
      var target = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, x).ToArray();
      for (int i = 0; i < x.Length; i++) {
        y[i] -= target[i];
      }
    }

    protected override void GetAllValuesSeries(out int[] x, out double[] y) {
      base.GetAllValuesSeries(out x, out y);
      var problemData = Content.ProblemData;
      var target = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, x).ToArray();
      for (int i = 0; i < x.Length; i++) {
        y[i] -= target[i];
      }
    }

    protected override void RedrawChart() {
      base.RedrawChart();
      UpdateSeriesStyle();
    }

    private void UpdateSeriesStyle() {
      base.chart.Series[RegressionSolutionLineChartView.ESTIMATEDVALUES_TRAINING_SERIES_NAME].YAxisType = AxisType.Secondary;
      base.chart.Series[RegressionSolutionLineChartView.ESTIMATEDVALUES_TEST_SERIES_NAME].YAxisType = AxisType.Secondary;
      base.chart.Series[RegressionSolutionLineChartView.ESTIMATEDVALUES_ALL_SERIES_NAME].YAxisType = AxisType.Secondary;
    }

  }
}
