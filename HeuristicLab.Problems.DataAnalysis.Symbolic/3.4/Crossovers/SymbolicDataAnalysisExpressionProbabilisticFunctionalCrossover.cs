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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("ProbabilisticFunctionalCrossover", "An operator which performs subtree swapping based on the behavioral similarity between subtrees:\n" +
                                            "- Take two parent individuals P0 and P1\n" +
                                            "- Randomly choose a node N from P0\n" +
                                            "- For each matching node M from P1, calculate the behavioral distance:\n" +
                                            "\t\tD(N,M) = 0.5 * ( abs(max(N) - max(M)) + abs(min(N) - min(M)) )\n" +
                                            "- Make a probabilistic weighted choice of node M from P1, based on the inversed and normalized behavioral distance")]
  [StorableType("EC1901B8-689B-4FC9-A79E-2778B3D0609E")]
  public sealed class SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    [StorableConstructor]
    private SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover(StorableConstructorFlag _) : base(_) { }
    private SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner)
      : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover()
      : base() {
      name = "ProbabilisticFunctionalCrossover";
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicDataAnalysisExpressionProbabilisticFunctionalCrossover<T>(this, cloner); }

    public override ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      List<int> rows = GenerateRowsToEvaluate().ToList();
      T problemData = ProblemDataParameter.ActualValue;
      return Cross(random, parent0, parent1, interpreter, problemData,
                   rows, MaximumSymbolicExpressionTreeDepth.Value, MaximumSymbolicExpressionTreeLength.Value);
    }

    /// <summary>
    /// Takes two parent individuals P0 and P1. 
    /// Randomly choose a node i from the first parent, then for each matching node j from the second parent, calculate the behavioral distance based on the range:
    /// d(i,j) = 0.5 * ( abs(max(i) - max(j)) + abs(min(i) - min(j)) ).
    /// Next, assign probabilities for the selection of a node j based on the inversed and normalized behavioral distance, then make a random weighted choice.
    /// </summary>
    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1,
                                                ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, T problemData, IList<int> rows, int maxDepth, int maxLength) {
      var crossoverPoints0 = new List<CutPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        // the if clauses prevent the root or the startnode from being selected, although the startnode can be the parent of the node being swapped.
        if (n.Parent != null && n.Parent != parent0.Root) {
          crossoverPoints0.Add(new CutPoint(n.Parent, n));
        }
      });

      var crossoverPoint0 = crossoverPoints0.SampleRandom(random);
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

      var dataset = problemData.Dataset;

      // create symbols in order to improvize an ad-hoc tree so that the child can be evaluated
      var rootSymbol = new ProgramRootSymbol();
      var startSymbol = new StartSymbol();
      var tree0 = CreateTreeFromNode(random, crossoverPoint0.Child, rootSymbol, startSymbol); // this will change crossoverPoint0.Child.Parent
      double min0 = 0.0, max0 = 0.0;
      foreach (double v in interpreter.GetSymbolicExpressionTreeValues(tree0, dataset, rows)) {
        if (min0 > v) min0 = v;
        if (max0 < v) max0 = v;
      }
      crossoverPoint0.Child.Parent = crossoverPoint0.Parent; // restore correct parent 

      var weights = new List<double>();
      foreach (var node in allowedBranches) {
        var parent = node.Parent;
        var tree1 = CreateTreeFromNode(random, node, rootSymbol, startSymbol);
        double min1 = 0.0, max1 = 0.0;
        foreach (double v in interpreter.GetSymbolicExpressionTreeValues(tree1, dataset, rows)) {
          if (min1 > v) min1 = v;
          if (max1 < v) max1 = v;
        }
        double behavioralDistance = (Math.Abs(min0 - min1) + Math.Abs(max0 - max1)) / 2; // this can be NaN of Infinity because some trees are crazy like exp(exp(exp(...))), we correct that below
        weights.Add(behavioralDistance);
        node.Parent = parent; // restore correct node parent
      }

      // remove branches with an infinite or NaN behavioral distance
      for (int i = weights.Count - 1; i >= 0; --i) {
        if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i])) {
          weights.RemoveAt(i);
          allowedBranches.RemoveAt(i);
        }
      }
      // check if there are any allowed branches left
      if (allowedBranches.Count == 0)
        return parent0;

      ISymbolicExpressionTreeNode selectedBranch;
      double sum = weights.Sum();

      if (sum.IsAlmost(0.0) || weights.Count == 1) // if there is only one allowed branch, or if all weights are zero
        selectedBranch = allowedBranches[0];
      else {
        for (int i = 0; i != weights.Count; ++i) // normalize and invert values
          weights[i] = 1 - weights[i] / sum;

        sum = weights.Sum(); // take new sum

        // compute the probabilities (selection weights)
        for (int i = 0; i != weights.Count; ++i)
          weights[i] /= sum;

#pragma warning disable 612, 618
        selectedBranch = allowedBranches.SelectRandom(weights, random);
#pragma warning restore 612, 618
      }
      Swap(crossoverPoint0, selectedBranch);
      return parent0;
    }
  }
}
