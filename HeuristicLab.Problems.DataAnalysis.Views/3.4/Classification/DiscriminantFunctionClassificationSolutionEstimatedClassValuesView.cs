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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Class Values")]
  [Content(typeof(IDiscriminantFunctionClassificationSolution))]
  public partial class DiscriminantFunctionClassificationSolutionEstimatedClassValuesView : ClassificationSolutionEstimatedClassValuesView {
    private const string TARGETVARIABLE_SERIES_NAME = "TargetVariable";
    private const string ESTIMATEDVALUES_SERIES_NAME = "Estimated Class Values (all)";
    private const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Class Values (training)";
    private const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Class Values (test)";
    private const string ESTIMATEDVALUES_DISCRIMINANT_SERIES_NAME = "Discriminant Values (all)";

    public new IDiscriminantFunctionClassificationSolution Content {
      get { return (IDiscriminantFunctionClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    public DiscriminantFunctionClassificationSolutionEstimatedClassValuesView()
      : base() {
      InitializeComponent();
    }

    protected override void UpdateEstimatedValues() {
      if (InvokeRequired) Invoke((Action)UpdateEstimatedValues);
      else {
        StringMatrix matrix = null;
        if (Content != null) {
          string[,] values = new string[Content.ProblemData.Dataset.Rows, 6];
          double[] target = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToArray();
          double[] estimatedClassValue = Content.EstimatedClassValues.ToArray();
          double[] estimatedValues = Content.EstimatedValues.ToArray();
          for (int row = 0; row < target.Length; row++) {
            values[row, 0] = row.ToString();
            values[row, 1] = target[row].ToString();
            values[row, 2] = estimatedClassValue[row].ToString();
            values[row, 5] = estimatedValues[row].ToString();
          }

          var estimatedTraining = Content.EstimatedTrainingClassValues.GetEnumerator();
          estimatedTraining.MoveNext();
          foreach (var trainingRow in Content.ProblemData.TrainingIndices) {
            values[trainingRow, 3] = estimatedTraining.Current.ToString();
            estimatedTraining.MoveNext();
          }
          var estimatedTest = Content.EstimatedTestClassValues.GetEnumerator();
          estimatedTest.MoveNext();
          foreach (var testRow in Content.ProblemData.TestIndices) {
            values[testRow, 4] = estimatedTest.Current.ToString();
            estimatedTest.MoveNext();
          }

          matrix = new StringMatrix(values);
          matrix.ColumnNames = new string[] { "Id", TARGETVARIABLE_SERIES_NAME, ESTIMATEDVALUES_SERIES_NAME, ESTIMATEDVALUES_TRAINING_SERIES_NAME, ESTIMATEDVALUES_TEST_SERIES_NAME, ESTIMATEDVALUES_DISCRIMINANT_SERIES_NAME };
          matrix.SortableView = true;
        }
        matrixView.Content = matrix;
      }
    }
  }
}
