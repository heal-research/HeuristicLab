﻿#region License Information
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [NonDiscoverableType]
  [StorableType("ECE25817-D6B8-45CA-9B03-F8B2940FF622")]
  [Item("GrowTreeCreator", "An operator that creates new symbolic expression trees using the 'Grow' method")]
  public class GrowTreeCreator : SymbolicExpressionTreeCreator {
    [StorableConstructor]
    protected GrowTreeCreator(StorableConstructorFlag _) : base(_) { }
    protected GrowTreeCreator(GrowTreeCreator original, Cloner cloner) : base(original, cloner) { }

    public GrowTreeCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GrowTreeCreator(this, cloner);
    }

    public override ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      return Create(random, grammar, maxTreeLength, maxTreeDepth);
    }

    /// <summary>
    /// Create a symbolic expression tree using the 'Grow' method.
    /// All symbols are allowed for nodes, so the resulting trees can be of any shape and size. 
    /// </summary>
    /// <param name="random">Random generator</param>
    /// <param name="grammar">Available tree grammar</param>
    /// <param name="maxTreeDepth">Maximum tree depth</param>
    /// <param name="maxTreeLength">Maximum tree length. This parameter is not used.</param>
    /// <returns></returns>
    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      var tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());


      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      rootNode.AddSubtree(startNode);

      Create(random, startNode, maxTreeDepth - 1);
      tree.Root = rootNode;
      return tree;
    }

    public static void Create(IRandom random, ISymbolicExpressionTreeNode seedNode, int maxDepth) {
      // make sure it is possible to create a trees smaller than maxDepth
      if (seedNode.Grammar.GetMinimumExpressionDepth(seedNode.Symbol) > maxDepth)
        throw new ArgumentException("Cannot create trees of depth " + maxDepth + " or smaller because of grammar constraints.", "maxDepth");

      var arity = SampleArity(random, seedNode);
      // throw an exception if the seedNode happens to be a terminal, since in this case we cannot grow a tree
      if (arity <= 0)
        throw new ArgumentException("Cannot grow tree. Seed node shouldn't have arity zero.");

      var allowedSymbols = seedNode.Grammar.AllowedSymbols.Where(s => s.InitialFrequency > 0.0).ToList();

      for (var i = 0; i < arity; i++) {
        var possibleSymbols = allowedSymbols.Where(s => seedNode.Grammar.IsAllowedChildSymbol(seedNode.Symbol, s, i)).ToList();
        var weights = possibleSymbols.Select(s => s.InitialFrequency).ToList();

#pragma warning disable 612, 618
        var selectedSymbol = possibleSymbols.SelectRandom(weights, random);
#pragma warning restore 612, 618

        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        seedNode.AddSubtree(tree);
      }

      // Only iterate over the non-terminal nodes (those which have arity > 0)
      // Start from depth 2 since the first two levels are formed by the rootNode and the seedNode
      foreach (var subTree in seedNode.Subtrees)
        if (subTree.Grammar.GetMaximumSubtreeCount(subTree.Symbol) > 0)
          RecursiveCreate(random, subTree, 2, maxDepth);
    }

    private static void RecursiveCreate(IRandom random, ISymbolicExpressionTreeNode root, int currentDepth, int maxDepth) {
      var arity = SampleArity(random, root);
      if (arity == 0)
        return;

      var allowedSymbols = root.Grammar.AllowedSymbols.Where(s => s.InitialFrequency > 0.0).ToList();

      for (var i = 0; i < arity; i++) {
        var possibleSymbols = allowedSymbols.Where(s => root.Grammar.IsAllowedChildSymbol(root.Symbol, s, i) &&
                                                        root.Grammar.GetMinimumExpressionDepth(s) - 1 <= maxDepth - currentDepth).ToList();

        if (!possibleSymbols.Any())
          throw new InvalidOperationException("No symbols are available for the tree.");

        var weights = possibleSymbols.Select(s => s.InitialFrequency).ToList();
#pragma warning disable 612, 618
        var selectedSymbol = possibleSymbols.SelectRandom(weights, random);
#pragma warning restore 612, 618

        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        root.AddSubtree(tree);
      }

      if (maxDepth > currentDepth)
        foreach (var subTree in root.Subtrees)
          if (subTree.Grammar.GetMaximumSubtreeCount(subTree.Symbol) != 0)
            RecursiveCreate(random, subTree, currentDepth + 1, maxDepth);
    }

    private static int SampleArity(IRandom random, ISymbolicExpressionTreeNode node) {
      var minArity = node.Grammar.GetMinimumSubtreeCount(node.Symbol);
      var maxArity = node.Grammar.GetMaximumSubtreeCount(node.Symbol);

      return random.Next(minArity, maxArity + 1);
    }
  }
}
