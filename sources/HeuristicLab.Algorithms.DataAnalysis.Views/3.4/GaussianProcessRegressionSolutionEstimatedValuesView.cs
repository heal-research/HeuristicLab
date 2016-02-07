#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Estimated Values")]
  [Content(typeof(GaussianProcessRegressionSolution), false)]
  public partial class GaussianProcessRegressionSolutionEstimatedValuesView : RegressionSolutionEstimatedValuesView {
    private const string ESTIMATEDVARIANCE_TRAINING_SERIES_NAME = "Estimated Variance (training)";
    private const string ESTIMATEDVARIANCE_TEST_SERIES_NAME = "Estimated Variance (test)";

    public new GaussianProcessRegressionSolution Content {
      get { return (GaussianProcessRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public GaussianProcessRegressionSolutionEstimatedValuesView()
      : base() {
      InitializeComponent();
    }


    protected override StringMatrix CreateValueMatrix() {
      var matrix = base.CreateValueMatrix();

      var columnNames = matrix.ColumnNames.Concat(new[] { ESTIMATEDVARIANCE_TRAINING_SERIES_NAME, ESTIMATEDVARIANCE_TEST_SERIES_NAME }).ToList();
      ((IStringConvertibleMatrix)matrix).Columns += 2;
      matrix.ColumnNames = columnNames;

      var trainingRows = Content.ProblemData.TrainingIndices;
      var testRows = Content.ProblemData.TestIndices;

      var estimated_var_training = Content.GetEstimatedVariance(trainingRows).GetEnumerator();
      var estimated_var_test = Content.GetEstimatedVariance(testRows).GetEnumerator();

      foreach (var row in Content.ProblemData.TrainingIndices) {
        estimated_var_training.MoveNext();
        matrix[row, 7] = estimated_var_training.Current.ToString();
      }

      foreach (var row in Content.ProblemData.TestIndices) {
        estimated_var_test.MoveNext();
        matrix[row, 8] = estimated_var_test.Current.ToString();
      }


      return matrix;
    }
  }
}
