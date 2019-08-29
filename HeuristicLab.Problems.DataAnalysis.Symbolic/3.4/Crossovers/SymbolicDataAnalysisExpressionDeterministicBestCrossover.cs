#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("DeterministicBestCrossover", "An operator which performs subtree swapping by choosing the best subtree to be swapped in a certain position:\n" +
                                      "- Take two parent individuals P0 and P1\n" +
                                      "- Randomly choose a crossover point C from P0\n" +
                                      "- Test all nodes from P1 to determine the one that produces the best child when inserted at place C in P0")]
  [StorableType("BC019C69-AA9D-4E98-B366-274EFD7922C4")]
  public sealed class SymbolicDataAnalysisExpressionDeterministicBestCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    [StorableConstructor]
    private SymbolicDataAnalysisExpressionDeterministicBestCrossover(StorableConstructorFlag _) : base(_) { }
    private SymbolicDataAnalysisExpressionDeterministicBestCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisExpressionDeterministicBestCrossover()
      : base() {
      name = "DeterministicBestCrossover";
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionDeterministicBestCrossover<T>(this, cloner);
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
    /// Randomly choose a node i from the first parent, then test all nodes j from the second parent to determine the best child that would be obtained by swapping i for j.
    /// </summary>
    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, IExecutionContext context,
                                                ISymbolicDataAnalysisSingleObjectiveEvaluator<T> evaluator, T problemData, List<int> rows, int maxDepth, int maxLength) {
      var crossoverPoints0 = new List<CutPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent0.Root)
          crossoverPoints0.Add(new CutPoint(n.Parent, n));
      });

      CutPoint crossoverPoint0 = crossoverPoints0.SampleRandom(random);
      int level = parent0.Root.GetBranchLevel(crossoverPoint0.Child);
      int length = parent0.Root.GetLength() - crossoverPoint0.Child.GetLength();

      var allowedBranches = new List<ISymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent1.Root) {
          if (n.GetDepth() + level <= maxDepth && n.GetLength() + length <= maxLength && crossoverPoint0.IsMatchingPointType(n))
            allowedBranches.Add(n);
        }
      });

      if (allowedBranches.Count == 0)
        return parent0;

      // create symbols in order to improvize an ad-hoc tree so that the child can be evaluated
      ISymbolicExpressionTreeNode selectedBranch = null;
      var nodeQualities = new List<Tuple<ISymbolicExpressionTreeNode, double>>();
      var originalChild = crossoverPoint0.Child;

      foreach (var node in allowedBranches) {
        var parent = node.Parent;
        Swap(crossoverPoint0, node); // the swap will set the nodes parent to crossoverPoint0.Parent
        IExecutionContext childContext = new ExecutionContext(context, evaluator, context.Scope);
        double quality = evaluator.Evaluate(childContext, parent0, problemData, rows);
        Swap(crossoverPoint0, originalChild); // swap the child back (so that the next swap will not affect the currently swapped node from parent1)
        nodeQualities.Add(new Tuple<ISymbolicExpressionTreeNode, double>(node, quality));
        node.Parent = parent; // restore correct parent
      }

      nodeQualities.Sort((a, b) => a.Item2.CompareTo(b.Item2));
      selectedBranch = evaluator.Maximization ? nodeQualities.Last().Item1 : nodeQualities.First().Item1;

      // swap the node that would create the best offspring
      Swap(crossoverPoint0, selectedBranch);
      return parent0;
    }
  }
}
