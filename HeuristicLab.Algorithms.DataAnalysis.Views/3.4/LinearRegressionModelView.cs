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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Linear regression model")]
  [Content(typeof(LinearRegressionModel), true)]
  public partial class LinearRegressionModelView : ItemView {

    public new LinearRegressionModel Content {
      get { return (LinearRegressionModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parametersView.Enabled = Content != null;
      covarianceMatrixView.Enabled = Content != null;
    }

    public LinearRegressionModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parametersView.Content = null;
        covarianceMatrixView.Content = null;
      } else {
        // format numbers to make them easier to read
        var w = new StringArray(Content.W.Select(x => $"{x:e4}").ToArray());
        w.ElementNames = Content.ParameterNames;
        parametersView.Content = w;

        // format all numbers in matrix
        var cStr = new string[Content.C.GetLength(0), Content.C.GetLength(1)];
        for (int i = 0; i < Content.C.GetLength(0); i++)
          for (int j = 0; j < Content.C.GetLength(0); j++)
            cStr[i, j] = $"{Content.C[i, j]:e4}";
        var c = new StringMatrix(cStr);
        c.RowNames = Content.ParameterNames;
        c.ColumnNames = Content.ParameterNames;
        covarianceMatrixView.Content = c;
      }
    }
  }
}
