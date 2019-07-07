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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views.Classification;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  [View("Solution Comparison")]
  [Content(typeof(ISymbolicClassificationSolution))]
  public partial class SolutionComparisonView : ClassificationSolutionComparisonView {

    public SolutionComparisonView() {
      InitializeComponent();
    }

    public new ISymbolicClassificationSolution Content {
      get { return (ISymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override IEnumerable<IClassificationSolution> GenerateClassificationSolutions() {
      var solutionsBase = base.GenerateClassificationSolutions();
      var solutions = new List<IClassificationSolution>();

      var symbolicSolution = Content;

      // does not support lagged variables
      if (symbolicSolution.Model.SymbolicExpressionTree.IterateNodesPrefix().OfType<LaggedVariableTreeNode>().Any()) return solutionsBase;

      var problemData = (IClassificationProblemData)symbolicSolution.ProblemData.Clone();
      if (!problemData.TrainingIndices.Any()) return null; // don't create an comparison models if the problem does not have a training set (e.g. loaded into an existing model)

      var usedVariables = Content.Model.SymbolicExpressionTree.IterateNodesPostfix()
      .OfType<IVariableTreeNode>()
      .Select(node => node.VariableName).ToArray();

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
      var newProblemData = new ClassificationProblemData(newDs, variableNames.Take(variableNames.Length - 1), variableNames.Last());
      newProblemData.PositiveClass = problemData.PositiveClass;
      newProblemData.TrainingPartition.Start = problemData.TrainingPartition.Start;
      newProblemData.TrainingPartition.End = problemData.TrainingPartition.End;
      newProblemData.TestPartition.Start = problemData.TestPartition.Start;
      newProblemData.TestPartition.End = problemData.TestPartition.End;

      try {
        var oneR = OneR.CreateOneRSolution(newProblemData);
        oneR.Name = "OneR Classification Solution (subset)";
        solutions.Add(oneR);
      } catch (NotSupportedException) { } catch (ArgumentException) { }
      try {
        var lda = LinearDiscriminantAnalysis.CreateLinearDiscriminantAnalysisSolution(newProblemData);
        lda.Name = "Linear Discriminant Analysis Solution (subset)";
        solutions.Add(lda);
      } catch (NotSupportedException) { } catch (ArgumentException) { }

      return solutionsBase.Concat(solutions);
    }
  }
}
