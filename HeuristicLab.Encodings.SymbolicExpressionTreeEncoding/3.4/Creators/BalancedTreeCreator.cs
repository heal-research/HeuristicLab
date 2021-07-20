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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [NonDiscoverableType]
  [StorableType("AA3649C4-18CF-480B-AA41-F5D6F148B494")]
  [Item("BalancedTreeCreator", "An operator that produces trees with a specified distribution")]
  public class BalancedTreeCreator : SymbolicExpressionTreeCreator {
    private const string IrregularityBiasParameterName = "IrregularityBias";

    public IFixedValueParameter<PercentValue> IrregularityBiasParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[IrregularityBiasParameterName]; }
    }

    public double IrregularityBias {
      get { return IrregularityBiasParameter.Value.Value; }
      set { IrregularityBiasParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected BalancedTreeCreator(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(IrregularityBiasParameterName)) {
        Parameters.Add(new FixedValueParameter<PercentValue>(IrregularityBiasParameterName, "Allows to bias tree initialization towards less balanced/regular shapes. Set to 0% for most balanced and 100% for least balanced trees. (default = 0%)", new PercentValue(0.0)));
      }
    }

    protected BalancedTreeCreator(BalancedTreeCreator original, Cloner cloner) : base(original, cloner) { }

    public BalancedTreeCreator() {
      Parameters.Add(new FixedValueParameter<PercentValue>(IrregularityBiasParameterName, new PercentValue(0.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BalancedTreeCreator(this, cloner);
    }

    public override ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxLength, int maxDepth) {
      return Create(random, grammar, maxLength, maxDepth, IrregularityBias);
    }

    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxLength, int maxDepth, double irregularityBias = 0) {
      int targetLength = random.Next(3, maxLength); // because we have 2 extra nodes for the root and start symbols, and the end is exclusive
      return CreateExpressionTree(random, grammar, targetLength, maxDepth, irregularityBias);
    }

    public static ISymbolicExpressionTree CreateExpressionTree(IRandom random, ISymbolicExpressionGrammar grammar, int targetLength, int maxDepth, double irregularityBias = 1) {
      // even lengths cannot be achieved without symbols of odd arity
      // therefore we randomly pick a neighbouring odd length value
      var tree = MakeStump(random, grammar); // create a stump consisting of just a ProgramRootSymbol and a StartSymbol
      CreateExpression(random, tree.Root.GetSubtree(0), targetLength - tree.Length, maxDepth - 2, irregularityBias); // -2 because the stump has length 2 and depth 2
      return tree;
    }

    private static ISymbolicExpressionTreeNode SampleNode(IRandom random, ISymbolicExpressionTreeGrammar grammar, IEnumerable<ISymbol> allowedSymbols, int minChildArity, int maxChildArity) {
      var candidates = new List<ISymbol>();
      var weights = new List<double>();

      foreach (var s in allowedSymbols) {
        var minSubtreeCount = grammar.GetMinimumSubtreeCount(s);
        var maxSubtreeCount = grammar.GetMaximumSubtreeCount(s);

        if (maxChildArity < minSubtreeCount || minChildArity > maxSubtreeCount) { continue; }

        candidates.Add(s);
        weights.Add(s.InitialFrequency);
      }
      var symbol = candidates.SampleProportional(random, 1, weights).First();
      var node = symbol.CreateTreeNode();
      if (node.HasLocalParameters) {
        node.ResetLocalParameters(random);
      }
      return node;
    }

    public static void CreateExpression(IRandom random, ISymbolicExpressionTreeNode root, int targetLength, int maxDepth, double irregularityBias = 1) {
      var grammar = root.Grammar;
      var minSubtreeCount = grammar.GetMinimumSubtreeCount(root.Symbol);
      var maxSubtreeCount = grammar.GetMaximumSubtreeCount(root.Symbol);
      var arity = random.Next(minSubtreeCount, maxSubtreeCount + 1);
      int openSlots = arity;

      var allowedSymbols = grammar.AllowedSymbols.Where(x => !(x is ProgramRootSymbol || x is GroupSymbol || x is Defun || x is StartSymbol)).ToList();
      bool hasUnarySymbols = allowedSymbols.Any(x => grammar.GetMinimumSubtreeCount(x) <= 1 && grammar.GetMaximumSubtreeCount(x) >= 1);

      if (!hasUnarySymbols && targetLength % 2 == 0) {
        // without functions of arity 1 some target lengths cannot be reached
        targetLength = random.NextDouble() < 0.5 ? targetLength - 1 : targetLength + 1;
      }

      var tuples = new List<NodeInfo>(targetLength) { new NodeInfo { Node = root, Depth = 0, Arity = arity } };

      // we use tuples.Count instead of targetLength in the if condition 
      // because depth limits may prevent reaching the target length 
      for (int i = 0; i < tuples.Count; ++i) {
        var t = tuples[i];
        var node = t.Node;

        for (int childIndex = 0; childIndex < t.Arity; ++childIndex) {
          // min and max arity here refer to the required arity limits for the child node
          int minChildArity = 0;
          int maxChildArity = 0;

          var allowedChildSymbols = allowedSymbols.Where(x => grammar.IsAllowedChildSymbol(node.Symbol, x, childIndex)).ToList();

          // if we are reaching max depth we have to fill the slot with a leaf node (max arity will be zero)
          // otherwise, find the maximum value from the grammar which does not exceed the length limit 
          if (t.Depth < maxDepth - 1 && openSlots < targetLength) {

            // we don't want to allow sampling a leaf symbol if it prevents us from reaching the target length
            // this should be allowed only when we have enough open expansion points (more than one)
            // the random check against the irregularity bias helps to increase shape variability when the conditions are met
            int minAllowedArity = allowedChildSymbols.Min(x => grammar.GetMaximumSubtreeCount(x));
            if (minAllowedArity == 0 && (openSlots - tuples.Count <= 1 || random.NextDouble() > irregularityBias)) {
              minAllowedArity = 1;
            }

            // finally adjust min and max arity according to the expansion limits
            int maxAllowedArity = allowedChildSymbols.Max(x => grammar.GetMaximumSubtreeCount(x));
            maxChildArity = Math.Min(maxAllowedArity, targetLength - openSlots);
            minChildArity = Math.Min(minAllowedArity, maxChildArity);
          }
         
          // sample a random child with the arity limits
          var child = SampleNode(random, grammar, allowedChildSymbols, minChildArity, maxChildArity);

          // get actual child arity limits
          minChildArity = Math.Max(minChildArity, grammar.GetMinimumSubtreeCount(child.Symbol));
          maxChildArity = Math.Min(maxChildArity, grammar.GetMaximumSubtreeCount(child.Symbol));
          minChildArity = Math.Min(minChildArity, maxChildArity);

          // pick a random arity for the new child node
          var childArity = random.Next(minChildArity, maxChildArity + 1);
          var childDepth = t.Depth + 1;
          node.AddSubtree(child);
          tuples.Add(new NodeInfo { Node = child, Depth = childDepth, Arity = childArity });
          openSlots += childArity;
        }
      }
    }

    protected override ISymbolicExpressionTree Create(IRandom random) {
      var maxLength = MaximumSymbolicExpressionTreeLengthParameter.ActualValue.Value;
      var maxDepth = MaximumSymbolicExpressionTreeDepthParameter.ActualValue.Value;
      var grammar = ClonedSymbolicExpressionTreeGrammarParameter.ActualValue;
      return Create(random, grammar, maxLength, maxDepth);
    }

    #region helpers
    private class NodeInfo {
      public ISymbolicExpressionTreeNode Node;
      public int Depth;
      public int Arity;
    }

    private static ISymbolicExpressionTree MakeStump(IRandom random, ISymbolicExpressionGrammar grammar) {
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      rootNode.AddSubtree(startNode);
      tree.Root = rootNode;
      return tree;
    }

    public void CreateExpression(IRandom random, ISymbolicExpressionTreeNode seedNode, int maxTreeLength, int maxTreeDepth) {
      CreateExpression(random, seedNode, maxTreeLength, maxTreeDepth, IrregularityBias);
    }
    #endregion
  }
}
