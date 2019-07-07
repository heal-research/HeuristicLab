#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Values")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionEstimatedValuesView : DataAnalysisSolutionEvaluationView {
    private const string TARGETVARIABLE_SERIES_NAME = "Target Variable";
    private const string ESTIMATEDVALUES_SERIES_NAME = "Estimated Values (all)";
    private const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Values (training)";
    private const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Values (test)";

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    private StringConvertibleMatrixView matrixView;

    public RegressionSolutionEstimatedValuesView()
      : base() {
      InitializeComponent();
      matrixView = new StringConvertibleMatrixView();
      matrixView.ShowRowsAndColumnsTextBox = false;
      matrixView.ShowStatisticalInformation = false;
      matrixView.Dock = DockStyle.Fill;
      this.Controls.Add(matrixView);
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    protected virtual void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected virtual void Content_ModelChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateEstimatedValues();
    }

    protected virtual StringMatrix CreateValueMatrix() {
      string[,] values = new string[Content.ProblemData.Dataset.Rows, 8];

      double[] target = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToArray();
      var estimated = Content.EstimatedValues.GetEnumerator();
      var estimated_training = Content.EstimatedTrainingValues.GetEnumerator();
      var estimated_test = Content.EstimatedTestValues.GetEnumerator();

      foreach (var row in Content.ProblemData.TrainingIndices) {
        estimated_training.MoveNext();
        values[row, 3] = estimated_training.Current.ToString();
      }

      foreach (var row in Content.ProblemData.TestIndices) {
        estimated_test.MoveNext();
        values[row, 4] = estimated_test.Current.ToString();
      }

      foreach (var row in Enumerable.Range(0, Content.ProblemData.Dataset.Rows)) {
        estimated.MoveNext();
        double est = estimated.Current;
        double res = target[row] - est;
        values[row, 0] = row.ToString();
        values[row, 1] = target[row].ToString();
        values[row, 2] = est.ToString();
        values[row, 5] = res.ToString();
        values[row, 6] = Math.Abs(res).ToString();
        values[row, 7] = Math.Abs(res / target[row]).ToString();
      }

      var matrix = new StringMatrix(values);
      matrix.ColumnNames = new string[] { "Id", TARGETVARIABLE_SERIES_NAME, ESTIMATEDVALUES_SERIES_NAME, ESTIMATEDVALUES_TRAINING_SERIES_NAME, ESTIMATEDVALUES_TEST_SERIES_NAME, "Residuals (all)", "Absolute Error (all)", "Relative Error (all)" };
      matrix.SortableView = true;
      return matrix;
    }

    protected virtual void UpdateEstimatedValues() {
      if (InvokeRequired) Invoke((Action)UpdateEstimatedValues);
      else {
        StringMatrix matrix = null;
        if (Content != null) {
          matrix = CreateValueMatrix();
        }
        matrixView.Content = matrix;
      }
    }
    #endregion
  }
}
