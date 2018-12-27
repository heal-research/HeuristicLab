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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [NonDiscoverableType]
  [StorableClass]
  [Item("ProbabilisticTreeCreator", "An operator that creates new symbolic expression trees with uniformly distributed length")]
  public class ProbabilisticTreeCreator : SymbolicExpressionTreeCreator,
    ISymbolicExpressionTreeSizeConstraintOperator, ISymbolicExpressionTreeGrammarBasedOperator {
    private const int MAX_TRIES = 100;

    [StorableConstructor]
    protected ProbabilisticTreeCreator(bool deserializing) : base(deserializing) { }
    protected ProbabilisticTreeCreator(ProbabilisticTreeCreator original, Cloner cloner) : base(original, cloner) { }
    public ProbabilisticTreeCreator()
      : base() {

    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProbabilisticTreeCreator(this, cloner);
    }


    protected override ISymbolicExpressionTree Create(IRandom random) {
      return Create(random, ClonedSymbolicExpressionTreeGrammarParameter.ActualValue,
        MaximumSymbolicExpressionTreeLengthParameter.ActualValue.Value, MaximumSymbolicExpressionTreeDepthParameter.ActualValue.Value);
    }

    public override ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      return Create(random, grammar, maxTreeLength, maxTreeDepth);
    }

    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      rootNode.AddSubtree(startNode);
      PTC2(random, startNode, maxTreeLength, maxTreeDepth);
      tree.Root = rootNode;
      return tree;
    }

    public static ISymbolicExpressionTree CreateExpressionTree(IRandom random, ISymbolicExpressionGrammar grammar, int targetLength,
      int maxTreeDepth) {
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      rootNode.AddSubtree(startNode);
      bool success = TryCreateFullTreeFromSeed(random, startNode, targetLength - 2, maxTreeDepth - 1);
      if (!success) throw new InvalidOperationException(string.Format("Could not create a tree with target length {0} and max depth {1}", targetLength, maxTreeDepth));

      tree.Root = rootNode;
      return tree;

    }

    private class TreeExtensionPoint {
      public ISymbolicExpressionTreeNode Parent { get; set; }
      public int ChildIndex { get; set; }
      public int ExtensionPointDepth { get; set; }
      public int MaximumExtensionLength { get; set; }
      public int MinimumExtensionLength { get; set; }
    }

    public static void PTC2(IRandom random, ISymbolicExpressionTreeNode seedNode,
      int maxLength, int maxDepth) {
      // make sure it is possible to create a trees smaller than maxLength and maxDepth
      if (seedNode.Grammar.GetMinimumExpressionLength(seedNode.Symbol) > maxLength)
        throw new ArgumentException("Cannot create trees of length " + maxLength + " or shorter because of grammar constraints.", "maxLength");
      if (seedNode.Grammar.GetMinimumExpressionDepth(seedNode.Symbol) > maxDepth)
        throw new ArgumentException("Cannot create trees of depth " + maxDepth + " or smaller because of grammar constraints.", "maxDepth");

      // tree length is limited by the grammar and by the explicit size constraints
      int allowedMinLength = seedNode.Grammar.GetMinimumExpressionLength(seedNode.Symbol);
      int allowedMaxLength = Math.Min(maxLength, seedNode.Grammar.GetMaximumExpressionLength(seedNode.Symbol, maxDepth));
      int tries = 0;
      while (tries++ < MAX_TRIES) {
        // select a target tree length uniformly in the possible range (as determined by explicit limits and limits of the grammar)
        int targetTreeLength;
        targetTreeLength = random.Next(allowedMinLength, allowedMaxLength + 1);
        if (targetTreeLength <= 1 || maxDepth <= 1) return;

        bool success = TryCreateFullTreeFromSeed(random, seedNode, targetTreeLength - 1, maxDepth - 1);

        // if successful => check constraints and return the tree if everything looks ok        
        if (success && seedNode.GetLength() <= maxLength && seedNode.GetDepth() <= maxDepth) {
          return;
        } else {
          // clean seedNode
          while (seedNode.Subtrees.Any()) seedNode.RemoveSubtree(0);
        }
        // try a different length MAX_TRIES times
      }
      throw new ArgumentException("Couldn't create a random valid tree.");
    }

    private static bool TryCreateFullTreeFromSeed(IRandom random, ISymbolicExpressionTreeNode root,
      int targetLength, int maxDepth) {
      List<TreeExtensionPoint> extensionPoints = new List<TreeExtensionPoint>();
      int currentLength = 0;
      int actualArity = SampleArity(random, root, targetLength, maxDepth);
      if (actualArity < 0) return false;

      for (int i = 0; i < actualArity; i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        root.AddSubtree(dummy);
        var x = new TreeExtensionPoint { Parent = root, ChildIndex = i, ExtensionPointDepth = 0 };
        FillExtensionLengths(x, maxDepth);
        extensionPoints.Add(x);
      }
      //necessary to use long data type as the extension point length could be int.MaxValue
      long minExtensionPointsLength = extensionPoints.Select(x => (long)x.MinimumExtensionLength).Sum();
      long maxExtensionPointsLength = extensionPoints.Select(x => (long)x.MaximumExtensionLength).Sum();

      // while there are pending extension points and we have not reached the limit of adding new extension points
      while (extensionPoints.Count > 0 && minExtensionPointsLength + currentLength <= targetLength) {
        int randomIndex = random.Next(extensionPoints.Count);
        TreeExtensionPoint nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        ISymbolicExpressionTreeNode parent = nextExtension.Parent;
        int argumentIndex = nextExtension.ChildIndex;
        int extensionDepth = nextExtension.ExtensionPointDepth;

        if (parent.Grammar.GetMinimumExpressionDepth(parent.Symbol) > maxDepth - extensionDepth) {
          ReplaceWithMinimalTree(random, root, parent, argumentIndex);
          int insertedTreeLength = parent.GetSubtree(argumentIndex).GetLength();
          currentLength += insertedTreeLength;
          minExtensionPointsLength -= insertedTreeLength;
          maxExtensionPointsLength -= insertedTreeLength;
        } else {
          //remove currently chosen extension point from calculation
          minExtensionPointsLength -= nextExtension.MinimumExtensionLength;
          maxExtensionPointsLength -= nextExtension.MaximumExtensionLength;

          var symbols = from s in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, argumentIndex)
                        where s.InitialFrequency > 0.0
                        where parent.Grammar.GetMinimumExpressionDepth(s) <= maxDepth - extensionDepth
                        where parent.Grammar.GetMinimumExpressionLength(s) <= targetLength - currentLength - minExtensionPointsLength
                        select s;
          if (maxExtensionPointsLength < targetLength - currentLength)
            symbols = from s in symbols
                      where parent.Grammar.GetMaximumExpressionLength(s, maxDepth - extensionDepth) >= targetLength - currentLength - maxExtensionPointsLength
                      select s;
          var allowedSymbols = symbols.ToList();

          if (allowedSymbols.Count == 0) return false;
          var weights = allowedSymbols.Select(x => x.InitialFrequency).ToList();

#pragma warning disable 612, 618
          var selectedSymbol = allowedSymbols.SelectRandom(weights, random);
#pragma warning restore 612, 618

          ISymbolicExpressionTreeNode newTree = selectedSymbol.CreateTreeNode();
          if (newTree.HasLocalParameters) newTree.ResetLocalParameters(random);
          parent.RemoveSubtree(argumentIndex);
          parent.InsertSubtree(argumentIndex, newTree);

          var topLevelNode = newTree as SymbolicExpressionTreeTopLevelNode;
          if (topLevelNode != null)
            topLevelNode.SetGrammar((ISymbolicExpressionTreeGrammar)root.Grammar.Clone());

          currentLength++;
          actualArity = SampleArity(random, newTree, targetLength - currentLength, maxDepth - extensionDepth);
          if (actualArity < 0) return false;
          for (int i = 0; i < actualArity; i++) {
            // insert a dummy sub-tree and add the pending extension to the list
            var dummy = new SymbolicExpressionTreeNode();
            newTree.AddSubtree(dummy);
            var x = new TreeExtensionPoint { Parent = newTree, ChildIndex = i, ExtensionPointDepth = extensionDepth + 1 };
            FillExtensionLengths(x, maxDepth);
            extensionPoints.Add(x);
            maxExtensionPointsLength += x.MaximumExtensionLength;
            minExtensionPointsLength += x.MinimumExtensionLength;
          }
        }
      }
      // fill all pending extension points
      while (extensionPoints.Count > 0) {
        int randomIndex = random.Next(extensionPoints.Count);
        TreeExtensionPoint nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        ISymbolicExpressionTreeNode parent = nextExtension.Parent;
        int a = nextExtension.ChildIndex;
        ReplaceWithMinimalTree(random, root, parent, a);
      }
      return true;
    }

    private static void ReplaceWithMinimalTree(IRandom random, ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode parent,
      int childIndex) {
      // determine possible symbols that will lead to the smallest possible tree
      var possibleSymbols = (from s in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, childIndex)
                             where s.InitialFrequency > 0.0
                             group s by parent.Grammar.GetMinimumExpressionLength(s) into g
                             orderby g.Key
                             select g).First().ToList();
      var weights = possibleSymbols.Select(x => x.InitialFrequency).ToList();

#pragma warning disable 612, 618
      var selectedSymbol = possibleSymbols.SelectRandom(weights, random);
#pragma warning restore 612, 618

      var tree = selectedSymbol.CreateTreeNode();
      if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
      parent.RemoveSubtree(childIndex);
      parent.InsertSubtree(childIndex, tree);

      var topLevelNode = tree as SymbolicExpressionTreeTopLevelNode;
      if (topLevelNode != null)
        topLevelNode.SetGrammar((ISymbolicExpressionTreeGrammar)root.Grammar.Clone());

      for (int i = 0; i < tree.Grammar.GetMinimumSubtreeCount(tree.Symbol); i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        tree.AddSubtree(dummy);
        // replace the just inserted dummy by recursive application
        ReplaceWithMinimalTree(random, root, tree, i);
      }
    }

    private static void FillExtensionLengths(TreeExtensionPoint extension, int maxDepth) {
      var grammar = extension.Parent.Grammar;
      int maxLength = int.MinValue;
      int minLength = int.MaxValue;
      foreach (ISymbol s in grammar.GetAllowedChildSymbols(extension.Parent.Symbol, extension.ChildIndex)) {
        if (s.InitialFrequency > 0.0) {
          int max = grammar.GetMaximumExpressionLength(s, maxDepth - extension.ExtensionPointDepth);
          maxLength = Math.Max(maxLength, max);
          int min = grammar.GetMinimumExpressionLength(s);
          minLength = Math.Min(minLength, min);
        }
      }

      extension.MaximumExtensionLength = maxLength;
      extension.MinimumExtensionLength = minLength;
    }

    private static int SampleArity(IRandom random, ISymbolicExpressionTreeNode node, int targetLength, int maxDepth) {
      // select actualArity randomly with the constraint that the sub-trees in the minimal arity can become large enough
      int minArity = node.Grammar.GetMinimumSubtreeCount(node.Symbol);
      int maxArity = node.Grammar.GetMaximumSubtreeCount(node.Symbol);
      if (maxArity > targetLength) {
        maxArity = targetLength;
      }
      if (minArity == maxArity) return minArity;

      // the min number of sub-trees has to be set to a value that is large enough so that the largest possible tree is at least tree length
      // if 1..3 trees are possible and the largest possible first sub-tree is smaller larger than the target length then minArity should be at least 2
      long aggregatedLongestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedLongestExpressionLength += (from s in node.Grammar.GetAllowedChildSymbols(node.Symbol, i)
                                              where s.InitialFrequency > 0.0
                                              select node.Grammar.GetMaximumExpressionLength(s, maxDepth)).Max();
        if (i > minArity && aggregatedLongestExpressionLength < targetLength) minArity = i + 1;
        else break;
      }

      // the max number of sub-trees has to be set to a value that is small enough so that the smallest possible tree is at most tree length 
      // if 1..3 trees are possible and the smallest possible first sub-tree is already larger than the target length then maxArity should be at most 0
      long aggregatedShortestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedShortestExpressionLength += (from s in node.Grammar.GetAllowedChildSymbols(node.Symbol, i)
                                               where s.InitialFrequency > 0.0
                                               select node.Grammar.GetMinimumExpressionLength(s)).Min();
        if (aggregatedShortestExpressionLength > targetLength) {
          maxArity = i;
          break;
        }
      }
      if (minArity > maxArity) return -1;
      return random.Next(minArity, maxArity + 1);
    }

  }
}