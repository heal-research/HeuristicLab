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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  [View("Error Characteristics Curve")]
  [Content(typeof(ISymbolicRegressionSolution))]
  public partial class SymbolicRegressionSolutionErrorCharacteristicsCurveView : RegressionSolutionErrorCharacteristicsCurveView {
    public SymbolicRegressionSolutionErrorCharacteristicsCurveView() {
      InitializeComponent();
    }

    public new ISymbolicRegressionSolution Content {
      get { return (ISymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    private IRegressionSolution CreateLinearRegressionSolution() {
      if (Content == null) throw new InvalidOperationException();
      double rmse, cvRmsError;
      var problemData = (IRegressionProblemData)ProblemData.Clone();
      if (!problemData.TrainingIndices.Any()) return null; // don't create an LR model if the problem does not have a training set (e.g. loaded into an existing model)

      //clear checked inputVariables
      foreach (var inputVariable in problemData.InputVariables.CheckedItems) {
        problemData.InputVariables.SetItemCheckedState(inputVariable.Value, false);
      }

      //check inputVariables used in the symbolic regression model
      var usedVariables =
        Content.Model.SymbolicExpressionTree.IterateNodesPostfix().OfType<VariableTreeNode>().Select(
          node => node.VariableName).Distinct();
      foreach (var variable in usedVariables) {
        problemData.InputVariables.SetItemCheckedState(
          problemData.InputVariables.First(x => x.Value == variable), true);
      }

      var solution = LinearRegression.CreateLinearRegressionSolution(problemData, out rmse, out cvRmsError);
      solution.Name = "Baseline (linear subset)";
      return solution;
    }


    protected override IEnumerable<IRegressionSolution> CreateBaselineSolutions() {
      foreach (var sol in base.CreateBaselineSolutions()) yield return sol;
      yield return CreateLinearRegressionSolution();
    }
  }
}
