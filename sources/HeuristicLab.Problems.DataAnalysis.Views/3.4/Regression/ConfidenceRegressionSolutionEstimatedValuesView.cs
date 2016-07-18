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
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Estimated Values")]
  [Content(typeof(IConfidenceRegressionSolution), false)]
  public partial class ConfidenceRegressionSolutionEstimatedValuesView : RegressionSolutionEstimatedValuesView {
    private const string ESTIMATEDVARIANCES_SERIES_NAME = "Estimated Variances (all)";
    private const string ESTIMATEDVARIANCES_TRAINING_SERIES_NAME = "Estimated Variances (training)";
    private const string ESTIMATEDVARIANCES_TEST_SERIES_NAME = "Estimated Variances (test)";

    public new IConfidenceRegressionSolution Content {
      get { return (IConfidenceRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public ConfidenceRegressionSolutionEstimatedValuesView()
      : base() {
      InitializeComponent();
    }


    protected override StringMatrix CreateValueMatrix() {
      var matrix = base.CreateValueMatrix();

      var columnNames = matrix.ColumnNames.Concat(new[] { ESTIMATEDVARIANCES_SERIES_NAME, ESTIMATEDVARIANCES_TRAINING_SERIES_NAME, ESTIMATEDVARIANCES_TEST_SERIES_NAME }).ToList();
      ((IStringConvertibleMatrix)matrix).Columns += 3;
      matrix.ColumnNames = columnNames;

      var trainingRows = Content.ProblemData.TrainingIndices;
      var testRows = Content.ProblemData.TestIndices;

      var estimated_var = Content.EstimatedVariances.GetEnumerator();
      var estimated_var_training = Content.GetEstimatedVariances(trainingRows).GetEnumerator();
      var estimated_var_test = Content.GetEstimatedVariances(testRows).GetEnumerator();

      foreach (var row in Enumerable.Range(0, Content.ProblemData.Dataset.Rows)) {
        estimated_var.MoveNext();
        matrix[row, 7] = estimated_var.Current.ToString();
      }

      foreach (var row in Content.ProblemData.TrainingIndices) {
        estimated_var_training.MoveNext();
        matrix[row, 8] = estimated_var_training.Current.ToString();
      }

      foreach (var row in Content.ProblemData.TestIndices) {
        estimated_var_test.MoveNext();
        matrix[row, 9] = estimated_var_test.Current.ToString();
      }


      return matrix;
    }
  }
}
