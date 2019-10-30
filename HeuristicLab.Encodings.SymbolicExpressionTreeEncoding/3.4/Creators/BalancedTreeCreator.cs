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
        Parameters.Add(new FixedValueParameter<PercentValue>(IrregularityBiasParameterName, new PercentValue(0.0)));
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

    private class SymbolCacheEntry {
      public int MinSubtreeCount;
      public int MaxSubtreeCount;
      public int[] MaxChildArity;
    }

    private class SymbolCache {
      public SymbolCache(ISymbolicExpressionGrammarBase grammar) {
        Grammar = grammar;
      }

      public ISymbolicExpressionTreeNode SampleNode(IRandom random, ISymbol parent, int childIndex, int minArity, int maxArity) {
        var symbols = new List<ISymbol>();
        var weights = new List<double>();
        foreach (var child in AllowedSymbols.Where(x => !(x is StartSymbol || x is Defun))) {
          var t = Tuple.Create(parent, child);
          if (!allowedCache.TryGetValue(t, out bool[] allowed)) { continue; }
          if (!allowed[childIndex]) { continue; }

          if (symbolCache.TryGetValue(child, out SymbolCacheEntry cacheItem)) {
            if (cacheItem.MinSubtreeCount < minArity) { continue; }
            if (cacheItem.MaxSubtreeCount > maxArity) { continue; }
          }

          symbols.Add(child);
          weights.Add(child.InitialFrequency);
        }
        if (symbols.Count == 0) {
          throw new ArgumentException("SampleNode: parent symbol " + parent.Name
            + " does not have any allowed child symbols with min arity " + minArity
            + " and max arity " + maxArity + ". Please ensure the grammar is properly configured.");
        }
        var symbol = symbols.SampleProportional(random, 1, weights).First();
        var node = symbol.CreateTreeNode();
        if (node.HasLocalParameters) {
          node.ResetLocalParameters(random);
        }
        return node;
      }

      public ISymbolicExpressionGrammarBase Grammar {
        get { return grammar; }
        set {
          grammar = value;
          RebuildCache();
        }
      }

      public IList<ISymbol> AllowedSymbols { get; private set; }

      public SymbolCacheEntry this[ISymbol symbol] {
        get { return symbolCache[symbol]; }
      }

      public bool[] this[ISymbol parent, ISymbol child] {
        get { return allowedCache[Tuple.Create(parent, child)]; }
      }

      public bool HasUnarySymbols { get; private set; }

      private void RebuildCache() {
        AllowedSymbols = Grammar.AllowedSymbols.Where(x => x.InitialFrequency > 0 && !(x is ProgramRootSymbol)).ToList();

        allowedCache = new Dictionary<Tuple<ISymbol, ISymbol>, bool[]>();
        symbolCache = new Dictionary<ISymbol, SymbolCacheEntry>();

        SymbolCacheEntry TryAddItem(ISymbol symbol) {
          if (!symbolCache.TryGetValue(symbol, out SymbolCacheEntry cacheItem)) {
            cacheItem = new SymbolCacheEntry {
              MinSubtreeCount = Grammar.GetMinimumSubtreeCount(symbol),
              MaxSubtreeCount = Grammar.GetMaximumSubtreeCount(symbol)
            };
            symbolCache[symbol] = cacheItem;
          }
          return cacheItem;
        }

        foreach (var parent in AllowedSymbols) {
          var parentCacheEntry = TryAddItem(parent);
          var maxChildArity = new int[parentCacheEntry.MaxSubtreeCount];

          if (!(parent is StartSymbol || parent is Defun)) {
            HasUnarySymbols |= parentCacheEntry.MaxSubtreeCount == 1;
          }

          foreach (var child in AllowedSymbols) {
            var childCacheEntry = TryAddItem(child);
            var allowed = new bool[parentCacheEntry.MaxSubtreeCount];

            for (int childIndex = 0; childIndex < parentCacheEntry.MaxSubtreeCount; ++childIndex) {
              allowed[childIndex] = Grammar.IsAllowedChildSymbol(parent, child, childIndex);
              maxChildArity[childIndex] = Math.Max(maxChildArity[childIndex], allowed[childIndex] ? childCacheEntry.MaxSubtreeCount : 0);
            }
            allowedCache[Tuple.Create(parent, child)] = allowed;
          }
          parentCacheEntry.MaxChildArity = maxChildArity;
        }
      }

      private ISymbolicExpressionGrammarBase grammar;
      private Dictionary<Tuple<ISymbol, ISymbol>, bool[]> allowedCache;
      private Dictionary<ISymbol, SymbolCacheEntry> symbolCache;
    }

    public static ISymbolicExpressionTree CreateExpressionTree(IRandom random, ISymbolicExpressionGrammar grammar, int targetLength, int maxDepth, double irregularityBias = 1) {
      // even lengths cannot be achieved without symbols of odd arity
      // therefore we randomly pick a neighbouring odd length value
      var tree = MakeStump(random, grammar); // create a stump consisting of just a ProgramRootSymbol and a StartSymbol
      CreateExpression(random, tree.Root.GetSubtree(0), targetLength - 2, maxDepth - 2, irregularityBias); // -2 because the stump has length 2 and depth 2
      return tree;
    }

    public static void CreateExpression(IRandom random, ISymbolicExpressionTreeNode root, int targetLength, int maxDepth, double irregularityBias = 1) {
      var grammar = root.Grammar;
      var symbolCache = new SymbolCache(grammar);
      var entry = symbolCache[root.Symbol];
      var arity = random.Next(entry.MinSubtreeCount, entry.MaxSubtreeCount + 1);
      var tuples = new List<NodeInfo>(targetLength) { new NodeInfo { Node = root, Depth = 0, Arity = arity } };
      int openSlots = arity;

      for (int i = 0; i < tuples.Count; ++i) {
        var t = tuples[i];
        var node = t.Node;
        var parentEntry = symbolCache[node.Symbol];

        for (int childIndex = 0; childIndex < t.Arity; ++childIndex) {
          // min and max arity here refer to the required arity limits for the child node
          int maxChildArity = t.Depth == maxDepth - 1 ? 0 : Math.Min(parentEntry.MaxChildArity[childIndex], targetLength - openSlots);
          int minChildArity = Math.Min((openSlots - tuples.Count > 1 && random.NextDouble() < irregularityBias) ? 0 : 1, maxChildArity);
          var child = symbolCache.SampleNode(random, node.Symbol, childIndex, minChildArity, maxChildArity);
          var childEntry = symbolCache[child.Symbol];
          var childArity = random.Next(childEntry.MinSubtreeCount, childEntry.MaxSubtreeCount + 1);
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
    #endregion
  }
}
