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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("RegressionSolution View")]
  [Content(typeof(RegressionSolutionBase), false)]
  public partial class RegressionSolutionView : DataAnalysisSolutionView {
    public RegressionSolutionView() {
      InitializeComponent();
    }

    public new RegressionSolutionBase Content {
      get { return (RegressionSolutionBase)base.Content; }
      set { base.Content = value; }
    }

    #region drag and drop
    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (ReadOnly) return;

      var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (dropData is IRegressionProblemData) validDragOperation = true;
      else if (dropData is IRegressionProblem) validDragOperation = true;
      else if (dropData is IValueParameter) {
        var param = (IValueParameter)dropData;
        if (param.Value is RegressionProblemData) validDragOperation = true;
      }
    }
    #endregion

    protected override bool CheckCompatibilityOfProblemData(IDataAnalysisProblemData problemData) {
      IRegressionProblemData regressionProblemData = problemData as IRegressionProblemData;
      if (regressionProblemData == null) {
        ErrorHandling.ShowErrorDialog(this, new ArgumentException("The problem data is no regression problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided."));
        return false;
      }

      if (!regressionProblemData.TargetVariable.Equals(Content.ProblemData.TargetVariable)) {
        string message = "The target variables are not matching. Old target variable: '"
                         + Content.ProblemData.TargetVariable
                         + "'. New targetvariable: '" + regressionProblemData.TargetVariable + "'";
        ErrorHandling.ShowErrorDialog(this, new InvalidOperationException(message));
        return false;
      }

      return base.CheckCompatibilityOfProblemData(problemData);
    }
  }
}
