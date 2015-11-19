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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("ContextAwareCrossover", "An operator which deterministically choses the best insertion point for a randomly selected node:\n" +
                                 "- Take two parent individuals P0 and P1\n" +
                                 "- Randomly choose a node N from P1\n" +
                                 "- Test all crossover points from P0 to determine the best location for N to be inserted")]
  public sealed class SymbolicDataAnalysisExpressionContextAwareCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    [StorableConstructor]
    private SymbolicDataAnalysisExpressionContextAwareCrossover(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionContextAwareCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisExpressionContextAwareCrossover()
      : base() {
      name = "ContextAwareCrossover";
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionContextAwareCrossover<T>(this, cloner);
    }
    public override ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      if (this.ExecutionContext == null)
        throw new InvalidOperationException("ExecutionContext not set.");
      List<int> rows = GenerateRowsToEvaluate().ToList();
      T problemData = ProblemDataParameter.ActualValue;
      ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator = EvaluatorParameter.ActualValue;

      return Cross(random, parent0, parent1, this.ExecutionContext, evaluator, problemData, rows, MaximumSymbolicExpressionTreeDepth.Value, MaximumSymbolicExpressionTreeLength.Value);
    }

    /// <summary>
    /// Takes two parent individuals P0 and P1. 
    /// Randomly choose a node i from the second parent, then test all possible crossover points from the first parent to determine the best location for i to be inserted.
    /// </summary>
    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, IExecutionContext context,
                                                ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator, T problemData, List<int> rows, int maxDepth, int maxLength) {
      // randomly choose a node from the second parent
      var possibleChildren = new List<ISymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent1.Root)
          possibleChildren.Add(n);
      });

      var selectedChild = possibleChildren.SampleRandom(random);
      var crossoverPoints = new List<CutPoint>();
      var qualities = new List<Tuple<CutPoint, double>>();

      parent0.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent0.Root) {
          var totalDepth = parent0.Root.GetBranchLevel(n) + selectedChild.GetDepth();
          var totalLength = parent0.Root.GetLength() - n.GetLength() + selectedChild.GetLength();
          if (totalDepth <= maxDepth && totalLength <= maxLength) {
            var crossoverPoint = new CutPoint(n.Parent, n);
            if (crossoverPoint.IsMatchingPointType(selectedChild))
              crossoverPoints.Add(crossoverPoint);
          }
        }
      });

      if (crossoverPoints.Any()) {
        // this loop will perform two swap operations per each crossover point
        foreach (var crossoverPoint in crossoverPoints) {
          // save the old parent so we can restore it later
          var parent = selectedChild.Parent;
          // perform a swap and check the quality of the solution
          Swap(crossoverPoint, selectedChild);
          IExecutionContext childContext = new ExecutionContext(context, evaluator, context.Scope);
          double quality = evaluator.Evaluate(childContext, parent0, problemData, rows);
          qualities.Add(new Tuple<CutPoint, double>(crossoverPoint, quality));
          // restore the correct parent
          selectedChild.Parent = parent;
          // swap the replaced subtree back into the tree so that the structure is preserved
          Swap(crossoverPoint, crossoverPoint.Child);
        }

        qualities.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // assuming this sorts the list in ascending order
        var crossoverPoint0 = evaluator.Maximization ? qualities.Last().Item1 : qualities.First().Item1;
        // swap the node that would create the best offspring
        // this last swap makes a total of (2 * crossoverPoints.Count() + 1) swap operations.
        Swap(crossoverPoint0, selectedChild);
      }

      return parent0;
    }
  }
}
