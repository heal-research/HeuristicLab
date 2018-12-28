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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("DepthConstrainedCrossover", "An operator which performs subtree swapping within a specific depth range. The range parameter controls the crossover behavior:\n" +
                                     "- HighLevel (upper 25% of the tree)\n" +
                                     "- Standard (mid 50% of the tree)\n" +
                                     "- LowLevel (lower 25% of the tree)")]
  [StorableType("E48D1FEB-8D3F-4394-9AD4-7C4A24116FD4")]
  public sealed class SymbolicDataAnalysisExpressionDepthConstrainedCrossover<T> :
    SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    private enum Ranges { HighLevel, Standard, LowLevel };
    private const string DepthRangeParameterName = "DepthRange";

    #region Parameter properties
    public IConstrainedValueParameter<StringValue> DepthRangeParameter {
      get { return (IConstrainedValueParameter<StringValue>)Parameters[DepthRangeParameterName]; }
    }
    #endregion

    #region Properties
    public StringValue DepthRange {
      get { return (StringValue)DepthRangeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionDepthConstrainedCrossover(StorableConstructorFlag _) : base(_) { }
    private SymbolicDataAnalysisExpressionDepthConstrainedCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner)
      : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionDepthConstrainedCrossover()
      : base() {
      Parameters.Add(new ConstrainedValueParameter<StringValue>(DepthRangeParameterName, "Depth range specifier"));
      DepthRangeParameter.ValidValues.Add(new StringValue(Enum.GetName(typeof(Ranges), Ranges.HighLevel)).AsReadOnly());
      DepthRangeParameter.ValidValues.Add(new StringValue(Enum.GetName(typeof(Ranges), Ranges.Standard)).AsReadOnly());
      DepthRangeParameter.ValidValues.Add(new StringValue(Enum.GetName(typeof(Ranges), Ranges.LowLevel)).AsReadOnly());
      name = "DepthConstrainedCrossover";
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicDataAnalysisExpressionDepthConstrainedCrossover<T>(this, cloner); }

    public override ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      return Cross(random, parent0, parent1, MaximumSymbolicExpressionTreeDepth.Value, MaximumSymbolicExpressionTreeLength.Value, DepthRange.Value);
    }


    /// <summary>
    /// Takes two parent individuals P0 and P1. 
    /// Randomly choose nodes that fall within the specified depth range in both parents.
    /// </summary>
    /// <param name="random">Pseudo-random number generator.</param>
    /// <param name="parent0">First parent.</param>
    /// <param name="parent1">Second parent.</param>
    /// <param name="maxDepth">Maximum allowed length depth.</param>
    /// <param name="maxLength">Maximum allowed tree length.</param>
    /// <param name="mode">Controls the crossover behavior: 
    /// - HighLevel (upper 25% of the tree),
    /// - Standard (mid 50%)
    /// - LowLevel (low 25%)</param>
    /// <returns></returns>
    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, int maxDepth, int maxLength, string mode) {
      int depth = parent0.Root.GetDepth() - 1; // substract 1 because the tree levels are counted from 0 
      var depthRange = new IntRange();
      const int depthOffset = 2; // skip the first 2 levels (root + startNode)
      switch ((Ranges)Enum.Parse(typeof(Ranges), mode)) {
        case Ranges.HighLevel:
          depthRange.Start = depthOffset; // skip the first 2 levels (root + startNode)
          depthRange.End = depthRange.Start + (int)Math.Round(depth * 0.25);
          break;
        case Ranges.Standard:
          depthRange.Start = depthOffset + (int)Math.Round(depth * 0.25);
          depthRange.End = depthRange.Start + (int)Math.Round(depth * 0.5);
          break;
        case Ranges.LowLevel:
          depthRange.Start = depthOffset + (int)Math.Round(depth * 0.75);
          depthRange.End = Math.Max(depthRange.Start, depth);
          break;
      }

      // make sure that the depth range does not exceeded the actual depth of parent0
      if (depthRange.Start > depth)
        depthRange.Start = depth;
      if (depthRange.End < depthRange.Start)
        depthRange.End = depthRange.Start;

      var crossoverPoints0 = (from node in GetNodesAtDepth(parent0, depthRange) select new CutPoint(node.Parent, node)).ToList();

      if (crossoverPoints0.Count == 0)
        throw new Exception("No crossover points available in the first parent");

      var crossoverPoint0 = crossoverPoints0.SampleRandom(random);
      int level = parent0.Root.GetBranchLevel(crossoverPoint0.Child);
      int length = parent0.Root.GetLength() - crossoverPoint0.Child.GetLength();

      var allowedBranches = (from s in GetNodesAtDepth(parent1, depthRange)
                             where s.GetDepth() + level <= maxDepth
                             where s.GetLength() + length <= maxLength
                             where crossoverPoint0.IsMatchingPointType(s)
                             select s).ToList();
      if (allowedBranches.Count == 0) return parent0;

      var selectedBranch = allowedBranches.SampleRandom(random);
      Swap(crossoverPoint0, selectedBranch);
      return parent0;
    }

    private static IEnumerable<ISymbolicExpressionTreeNode> GetNodesAtDepth(ISymbolicExpressionTree tree, IntRange depthRange) {
      var treeDepth = tree.Root.GetDepth();
      return from node in tree.Root.IterateNodesPostfix()
             let depth = treeDepth - node.GetDepth()
             where depthRange.Start <= depth
             where depth <= depthRange.End
             select node;
    }
  }
}
