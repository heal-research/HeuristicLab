#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators {
  [StorableClass]
  [Item("ProbabilisticTreeCreator", "An operator that creates new symbolic expression trees with uniformly distributed size")]
  public sealed class ProbabilisticTreeCreator : SymbolicExpressionTreeCreator {
    private const int MAX_TRIES = 100;
    [StorableConstructor]
    private ProbabilisticTreeCreator(bool deserializing) : base(deserializing) { }
    private ProbabilisticTreeCreator(ProbabilisticTreeCreator original, Cloner cloner) : base(original, cloner) { }
    public ProbabilisticTreeCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProbabilisticTreeCreator(this, cloner);
    }

    protected override SymbolicExpressionTree Create(
      IRandom random,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments) {
      return Create(random, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefinitions.Value, maxFunctionArguments.Value);
    }

    public static SymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefinitions, int maxFunctionArguments
      ) {
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(new SymbolicExpressionTreeGrammar(grammar));
      tree.Root = PTC2(random, rootNode, maxTreeSize, maxTreeHeight, maxFunctionDefinitions, maxFunctionArguments);
      return tree;
    }

    private class TreeExtensionPoint {
      public SymbolicExpressionTreeNode Parent { get; set; }
      public int ChildIndex { get; set; }
      public int ExtensionPointDepth { get; set; }
    }

    public static SymbolicExpressionTreeNode PTC2(IRandom random, SymbolicExpressionTreeNode seedNode,
      int maxTreeSize, int maxDepth, int maxFunctionDefinitions, int maxFunctionArguments) {
      // tree size is limited by the grammar and by the explicit size constraints
      int allowedMinSize = seedNode.Grammar.GetMinExpressionLength(seedNode.Symbol);
      int allowedMaxSize = Math.Min(maxTreeSize, seedNode.Grammar.GetMaxExpressionLength(seedNode.Symbol));
      int tries = 0;
      while (tries++ < MAX_TRIES) {
        // select a target tree size uniformly in the possible range (as determined by explicit limits and limits of the grammar)
        int treeSize = random.Next(allowedMinSize, allowedMaxSize + 1);
        if (treeSize <= 1 || maxDepth <= 1) return seedNode;

        bool success = CreateFullTreeFromSeed(random, seedNode, seedNode.Grammar, treeSize, maxDepth, maxFunctionDefinitions, maxFunctionArguments);

        // if successful => check constraints and return the tree if everything looks ok        
        if (success && seedNode.GetSize() <= maxTreeSize && seedNode.GetHeight() <= maxDepth) {
          return seedNode;
        } else {
          // clean seedNode
          while (seedNode.SubTrees.Count > 0) seedNode.RemoveSubTree(0);
        }
        // try a different size MAX_TRIES times
      }
      throw new ArgumentException("Couldn't create a random valid tree.");
    }

    private static bool CreateFullTreeFromSeed(IRandom random, SymbolicExpressionTreeNode root, ISymbolicExpressionGrammar globalGrammar,
      int size, int maxDepth, int maxFunctionDefinitions, int maxFunctionArguments) {
      try {
        TryCreateFullTreeFromSeed(random, root, globalGrammar, size, maxDepth, maxFunctionDefinitions, maxFunctionArguments);
        return true;
      }
      catch (ArgumentException) { return false; }
    }

    private static void TryCreateFullTreeFromSeed(IRandom random, SymbolicExpressionTreeNode root, ISymbolicExpressionGrammar globalGrammar,
      int size, int maxDepth, int maxFunctionDefinitions, int maxFunctionArguments) {
      List<TreeExtensionPoint> extensionPoints = new List<TreeExtensionPoint>();
      int currentSize = 1;
      int totalListMinSize = globalGrammar.GetMinExpressionLength(root.Symbol) - 1;
      int actualArity = SampleArity(random, root, size);
      for (int i = 0; i < actualArity; i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        root.AddSubTree(dummy);
        extensionPoints.Add(new TreeExtensionPoint { Parent = root, ChildIndex = i, ExtensionPointDepth = 0 });
      }
      // while there are pending extension points and we have not reached the limit of adding new extension points
      while (extensionPoints.Count > 0 && totalListMinSize + currentSize < size) {
        int randomIndex = random.Next(extensionPoints.Count);
        TreeExtensionPoint nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = nextExtension.Parent;
        int argumentIndex = nextExtension.ChildIndex;
        int extensionDepth = nextExtension.ExtensionPointDepth;
        if (extensionDepth + parent.Grammar.GetMinExpressionDepth(parent.Symbol) >= maxDepth) {
          ReplaceWithMinimalTree(random, root, parent, argumentIndex, maxFunctionDefinitions, maxFunctionArguments);
        } else {
          var allowedSymbols = from s in parent.Grammar.Symbols
                               where parent.Grammar.IsAllowedChild(parent.Symbol, s, argumentIndex)
                               where parent.Grammar.GetMinExpressionDepth(s) + extensionDepth - 1 < maxDepth
                               where parent.Grammar.GetMaxExpressionLength(s) > size - totalListMinSize - currentSize
                               select s;
          Symbol selectedSymbol = SelectRandomSymbol(random, allowedSymbols);
          SymbolicExpressionTreeNode newTree = selectedSymbol.CreateTreeNode();
          if (newTree.HasLocalParameters) newTree.ResetLocalParameters(random);
          parent.RemoveSubTree(argumentIndex);
          parent.InsertSubTree(argumentIndex, newTree);

          InitializeNewTreeNode(random, root, newTree, maxFunctionDefinitions, maxFunctionArguments);

          currentSize++;
          totalListMinSize--;

          actualArity = SampleArity(random, newTree, size - currentSize);
          for (int i = 0; i < actualArity; i++) {
            // insert a dummy sub-tree and add the pending extension to the list
            var dummy = new SymbolicExpressionTreeNode();
            newTree.AddSubTree(dummy);
            extensionPoints.Add(new TreeExtensionPoint { Parent = newTree, ChildIndex = i, ExtensionPointDepth = extensionDepth + 1 });
          }
          totalListMinSize += newTree.Grammar.GetMinExpressionLength(newTree.Symbol);
        }
      }
      // fill all pending extension points
      while (extensionPoints.Count > 0) {
        int randomIndex = random.Next(extensionPoints.Count);
        TreeExtensionPoint nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = nextExtension.Parent;
        int a = nextExtension.ChildIndex;
        int d = nextExtension.ExtensionPointDepth;
        ReplaceWithMinimalTree(random, root, parent, a, maxFunctionDefinitions, maxFunctionArguments);
      }
    }

    private static void ReplaceWithMinimalTree(IRandom random, SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode parent, int argumentIndex, int maxFunctionDefinitions, int maxFunctionArguments) {
      // determine possible symbols that will lead to the smallest possible tree
      var possibleSymbols = (from s in parent.GetAllowedSymbols(argumentIndex)
                             group s by parent.Grammar.GetMinExpressionLength(s) into g
                             orderby g.Key
                             select g).First();
      var selectedSymbol = SelectRandomSymbol(random, possibleSymbols);
      var tree = selectedSymbol.CreateTreeNode();
      if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
      parent.RemoveSubTree(argumentIndex);
      parent.InsertSubTree(argumentIndex, tree);
      InitializeNewTreeNode(random, root, tree, maxFunctionDefinitions, maxFunctionArguments);
      for (int i = 0; i < tree.GetMinSubtreeCount(); i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        tree.AddSubTree(dummy);
        // replace the just inserted dummy by recursive application
        ReplaceWithMinimalTree(random, root, tree, i, maxFunctionDefinitions, maxFunctionArguments);
      }
    }

    private static void InitializeNewTreeNode(IRandom random, SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode newTree, int maxFunctionDefinitions, int maxFunctionArguments) {
      // NB it is assumed that defuns are only allowed as children of root and nowhere else
      // also assumes that newTree is already attached to root somewhere
      if (IsTopLevelBranch(root, newTree)) {
        ((SymbolicExpressionTreeTopLevelNode)newTree).SetGrammar((ISymbolicExpressionGrammar)root.Grammar.Clone());

        // allow invokes of existing ADFs with higher index
        int argIndex = root.SubTrees.IndexOf(newTree);
        for (int i = argIndex + 1; i < root.SubTrees.Count; i++) {
          var otherDefunNode = root.SubTrees[i] as DefunTreeNode;
          if (otherDefunNode != null) {
            GrammarModifier.AddDynamicSymbol(newTree.Grammar, newTree.Symbol, otherDefunNode.FunctionName, otherDefunNode.NumberOfArguments);
          }
        }
      }
      if (newTree.Symbol is Defun) {
        var defunTree = newTree as DefunTreeNode;
        string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefinitions * 10 - 1)).ToString(); // >= 100 functions => ###
        var allowedNames = from index in Enumerable.Range(0, maxFunctionDefinitions)
                           select "ADF" + index.ToString(formatString);
        var takenNames = (from node in root.IterateNodesPrefix().OfType<DefunTreeNode>()
                          select node.FunctionName).Distinct();
        var remainingNames = allowedNames.Except(takenNames).ToList();
        string functionName = remainingNames[random.Next(remainingNames.Count)];
        // set name and number of arguments of the ADF
        int nArgs = random.Next(maxFunctionArguments);
        defunTree.FunctionName = functionName;
        defunTree.NumberOfArguments = nArgs;
        if (nArgs > 0) {
          GrammarModifier.AddDynamicArguments(defunTree.Grammar, defunTree.Symbol, Enumerable.Range(0, nArgs));
        }
        // in existing branches with smaller index allow invoke of current function
        int argIndex = root.SubTrees.IndexOf(newTree);
        for (int i = 0; i < argIndex; i++) {
          // if not dummy node
          if (root.SubTrees[i].Symbol != null) {
            var existingBranch = root.SubTrees[i];
            GrammarModifier.AddDynamicSymbol(existingBranch.Grammar, existingBranch.Symbol, functionName, nArgs);
          }
        }
      }
    }

    private static bool IsTopLevelBranch(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode branch) {
      return branch is SymbolicExpressionTreeTopLevelNode;
    }

    private static Symbol SelectRandomSymbol(IRandom random, IEnumerable<Symbol> symbols) {
      var symbolList = symbols.ToList();
      var ticketsSum = symbolList.Select(x => x.InitialFrequency).Sum();
      if (ticketsSum == 0.0) throw new ArgumentException("The initial frequency of all allowed symbols is zero.");
      var r = random.NextDouble() * ticketsSum;
      double aggregatedTickets = 0;
      for (int i = 0; i < symbolList.Count; i++) {
        aggregatedTickets += symbolList[i].InitialFrequency;
        if (aggregatedTickets > r) {
          return symbolList[i];
        }
      }
      // this should never happen
      throw new ArgumentException("There is a problem with the initial frequency setting of allowed symbols.");
    }

    private static int SampleArity(IRandom random, SymbolicExpressionTreeNode node, int targetSize) {
      // select actualArity randomly with the constraint that the sub-trees in the minimal arity can become large enough
      int minArity = node.GetMinSubtreeCount();
      int maxArity = node.GetMaxSubtreeCount();
      if (maxArity > targetSize) {
        maxArity = targetSize;
      }
      // the min number of sub-trees has to be set to a value that is large enough so that the largest possible tree is at least tree size
      // if 1..3 trees are possible and the largest possible first sub-tree is smaller larger than the target size then minArity should be at least 2
      long aggregatedLongestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedLongestExpressionLength += (from s in node.GetAllowedSymbols(i)
                                              select node.Grammar.GetMaxExpressionLength(s)).Max();
        if (aggregatedLongestExpressionLength < targetSize) minArity = i;
        else break;
      }

      // the max number of sub-trees has to be set to a value that is small enough so that the smallest possible tree is at most tree size 
      // if 1..3 trees are possible and the smallest possible first sub-tree is already larger than the target size then maxArity should be at most 0
      long aggregatedShortestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedShortestExpressionLength += (from s in node.GetAllowedSymbols(i)
                                               select node.Grammar.GetMinExpressionLength(s)).Min();
        if (aggregatedShortestExpressionLength > targetSize) {
          maxArity = i;
          break;
        }
      }
      if (minArity > maxArity) throw new ArgumentException();
      return random.Next(minArity, maxArity + 1);
    }
  }
}