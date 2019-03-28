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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization.Operators;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("2634824A-D8E6-4047-9146-79EDF50BEED8")]
  [Item("SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator", "A similarity calculator based on the size of the maximum common subtree between two trees")]
  public class SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator : SolutionSimilarityCalculator {
    [Storable]
    private readonly SymbolicExpressionTreeNodeEqualityComparer comparer;
    public bool MatchVariableWeights {
      get { return comparer.MatchVariableWeights; }
      set { comparer.MatchVariableWeights = value; }
    }

    public bool MatchConstantValues {
      get { return comparer.MatchConstantValues; }
      set { comparer.MatchConstantValues = value; }
    }

    protected override bool IsCommutative { get { return true; } }

    [StorableConstructor]
    protected SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator(this, cloner);
    }

    protected SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator(SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      comparer = cloner.Clone(original.comparer);
    }

    public SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator() {
      comparer = new SymbolicExpressionTreeNodeEqualityComparer {
        MatchConstantValues = true,
        MatchVariableNames = true,
        MatchVariableWeights = true
      };
    }

    public SymbolicExpressionTreeMaxCommonSubtreeSimilarityCalculator(bool matchVariableWeights, bool matchConstantValues) {
      comparer = new SymbolicExpressionTreeNodeEqualityComparer {
        MatchConstantValues = matchConstantValues,
        MatchVariableNames = true,
        MatchVariableWeights = matchVariableWeights
      };
    }

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2) {
      return MaxCommonSubtreeSimilarity(t1, t2, comparer);
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var t1 = leftSolution.Variables[SolutionVariableName].Value as ISymbolicExpressionTree;
      var t2 = rightSolution.Variables[SolutionVariableName].Value as ISymbolicExpressionTree;

      if (t1 == null || t2 == null)
        throw new ArgumentException("Cannot calculate similarity when one of the arguments is null.");

      return MaxCommonSubtreeSimilarity(t1, t2, comparer);
    }

    public static double MaxCommonSubtreeSimilarity(ISymbolicExpressionTree a, ISymbolicExpressionTree b, ISymbolicExpressionTreeNodeSimilarityComparer comparer) {
      int max = 0;
      var rootA = a.Root.GetSubtree(0).GetSubtree(0);
      var rootB = b.Root.GetSubtree(0).GetSubtree(0);
      foreach (var aa in rootA.IterateNodesBreadth()) {
        int lenA = aa.GetLength();
        if (lenA <= max) continue;
        foreach (var bb in rootB.IterateNodesBreadth()) {
          int lenB = bb.GetLength();
          if (lenB <= max) continue;
          int matches = SymbolicExpressionTreeMatching.Match(aa, bb, comparer);
          if (max < matches)
            max = matches;
        }
      }
      return 2.0 * max / (rootA.GetLength() + rootB.GetLength());
    }
  }
}
