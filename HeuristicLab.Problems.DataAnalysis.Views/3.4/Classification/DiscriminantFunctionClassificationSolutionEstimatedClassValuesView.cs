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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Class Values")]
  [Content(typeof(IDiscriminantFunctionClassificationSolution))]
  public partial class DiscriminantFunctionClassificationSolutionEstimatedClassValuesView : ClassificationSolutionEstimatedClassValuesView {
    private const string TargetClassValuesSeriesname = "TargetVariable";
    private const string EstimatedClassValuesSeriesName = "EstimatedClassValues";
    private const string EstimatedValuesSeriesName = "EstimatedValues";

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
          string[,] values = new string[Content.ProblemData.Dataset.Rows, 4];

          double[] target = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToArray();
          double[] estimatedClassValues = Content.EstimatedClassValues.ToArray();
          double[] estimatedValues = Content.EstimatedValues.ToArray();
          for (int row = 0; row < target.Length; row++) {
            values[row, 0] = row.ToString();
            values[row, 1] = target[row].ToString();
            values[row, 2] = estimatedClassValues[row].ToString();
            values[row, 3] = estimatedValues[row].ToString();
          }

          matrix = new StringMatrix(values);
          matrix.ColumnNames = new string[] { "Id", TargetClassValuesSeriesname, EstimatedClassValuesSeriesName, EstimatedValuesSeriesName };
          matrix.SortableView = true;
        }
        matrixView.Content = matrix;
      }
    }
  }
}
