#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Class Values")]
  [Content(typeof(IClassificationSolution))]
  public partial class ClassificationSolutionEstimatedClassValuesView : ItemView, IClassificationSolutionEvaluationView {
    private const string TARGETVARIABLE_SERIES_NAME = "Target Variable";
    private const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Class Values (training)";
    private const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Class Values (test)";

    public new IClassificationSolution Content {
      get { return (IClassificationSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    private StringConvertibleMatrixView matrixView;

    public ClassificationSolutionEstimatedClassValuesView()
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

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateEstimatedValues();
    }

    private void UpdateEstimatedValues() {
      if (InvokeRequired) Invoke((Action)UpdateEstimatedValues);
      else {
        DoubleMatrix matrix = null;
        if (Content != null) {
          double[,] values = new double[Content.ProblemData.Dataset.Rows, 3];
          // fill with NaN
          for (int row = 0; row < Content.ProblemData.Dataset.Rows; row++)
            for (int column = 0; column < 3; column++)
              values[row, column] = double.NaN;

          double[] target = Content.ProblemData.Dataset.GetVariableValues(Content.ProblemData.TargetVariable);
          for (int row = 0; row < target.Length; row++) {
            values[row, 0] = target[row];
          }
          var estimatedTraining = Content.EstimatedTrainingClassValues.GetEnumerator();
          estimatedTraining.MoveNext();
          foreach (var trainingRow in Content.ProblemData.TrainingIndizes) {
            values[trainingRow, 1] = estimatedTraining.Current;
            estimatedTraining.MoveNext();
          }
          var estimatedTest = Content.EstimatedTestClassValues.GetEnumerator();
          estimatedTest.MoveNext();
          foreach (var testRow in Content.ProblemData.TestIndizes) {
            values[testRow, 2] = estimatedTest.Current;
            estimatedTest.MoveNext();
          }

          matrix = new DoubleMatrix(values);
          matrix.ColumnNames = new string[] { TARGETVARIABLE_SERIES_NAME, ESTIMATEDVALUES_TRAINING_SERIES_NAME, ESTIMATEDVALUES_TEST_SERIES_NAME };
        }
        matrixView.Content = matrix;
      }
    }
    #endregion
  }
}
