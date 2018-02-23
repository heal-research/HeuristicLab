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
using System.Collections;
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

      var usedVariables = Content.Model.VariablesUsedForPrediction;

      var usedDoubleVariables = usedVariables
        .Where(name => problemData.Dataset.VariableHasType<double>(name))
      .Distinct();

      var usedFactorVariables = usedVariables
        .Where(name => problemData.Dataset.VariableHasType<string>(name))
        .Distinct();

      // gkronber: for binary factors we actually produce a binary variable in the new dataset
      // but only if the variable is not used as a full factor anyway (LR creates binary columns anyway)
      var usedBinaryFactors =
        Content.Model.SymbolicExpressionTree.IterateNodesPostfix().OfType<BinaryFactorVariableTreeNode>()
        .Where(node => !usedFactorVariables.Contains(node.VariableName))
        .Select(node => Tuple.Create(node.VariableValue, node.VariableValue));

      // create a new problem and dataset
      var variableNames =
        usedDoubleVariables
        .Concat(usedFactorVariables)
        .Concat(usedBinaryFactors.Select(t => t.Item1 + "=" + t.Item2))
        .Concat(new string[] { problemData.TargetVariable })
        .ToArray();
      var variableValues =
        usedDoubleVariables.Select(name => (IList)problemData.Dataset.GetDoubleValues(name).ToList())
        .Concat(usedFactorVariables.Select(name => problemData.Dataset.GetStringValues(name).ToList()))
        .Concat(
          // create binary variable
          usedBinaryFactors.Select(t => problemData.Dataset.GetReadOnlyStringValues(t.Item1).Select(val => val == t.Item2 ? 1.0 : 0.0).ToList())
        )
        .Concat(new[] { problemData.Dataset.GetDoubleValues(problemData.TargetVariable).ToList() });

      var newDs = new Dataset(variableNames, variableValues);
      var newProblemData = new RegressionProblemData(newDs, variableNames.Take(variableNames.Length - 1), variableNames.Last());
      newProblemData.TrainingPartition.Start = problemData.TrainingPartition.Start;
      newProblemData.TrainingPartition.End = problemData.TrainingPartition.End;
      newProblemData.TestPartition.Start = problemData.TestPartition.Start;
      newProblemData.TestPartition.End = problemData.TestPartition.End;

      var solution = LinearRegression.CreateLinearRegressionSolution(newProblemData, out rmse, out cvRmsError);
      solution.Name = "Baseline (linear subset)";
      return solution;
    }


    protected override IEnumerable<IRegressionSolution> CreateBaselineSolutions() {
      foreach (var sol in base.CreateBaselineSolutions()) yield return sol;

      // does not support lagged variables
      if (Content.Model.SymbolicExpressionTree.IterateNodesPrefix().OfType<LaggedVariableTreeNode>().Any()) yield break;

      yield return CreateLinearRegressionSolution();
    }
  }
}
