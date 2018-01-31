#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("Line Chart")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionLineChartView : RegressionSolutionLineChartViewBase {


    public RegressionSolutionLineChartView()
      : base() {
      InitializeComponent();
    }

    protected override void GetTrainingSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.TrainingIndices.ToArray();
      y = Content.EstimatedTrainingValues.ToArray();
    }

    protected override void GetTestSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.TestIndices.ToArray();
      y = Content.EstimatedTestValues.ToArray();
    }

    protected override void GetAllValuesSeries(out int[] idx, out double[] y) {
      idx = Content.ProblemData.AllIndices.ToArray();
      y = Content.EstimatedValues.ToArray();
    }
  }
}
