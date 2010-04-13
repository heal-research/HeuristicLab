#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;
using System;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using System.Text;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("ProbabilisticTreeCreator", "An operator that creates new symbolic expression trees with uniformly distributed size")]
  public class ProbabilisticTreeCreator : SymbolicExpressionTreeCreator {

    public ProbabilisticTreeCreator()
      : base() {
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
      // tree size is limited by the grammar and by the explicit size constraints
      int allowedMinSize = grammar.GetMinExpressionLength(grammar.StartSymbol);
      int allowedMaxSize = Math.Min(maxTreeSize, grammar.GetMaxExpressionLength(grammar.StartSymbol));
      // select a target tree size uniformly in the possible range (as determined by explicit limits and limits of the grammar)
      int treeSize = random.Next(allowedMinSize, allowedMaxSize);
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      do {
        try {
          tree.Root = PTC2(random, grammar, grammar.StartSymbol, treeSize + 1, maxTreeHeight + 1, maxFunctionDefinitions, maxFunctionArguments);
        }
        catch (ArgumentException) {
          // try a different size
          treeSize = random.Next(allowedMinSize, allowedMaxSize);
        }
      } while (tree.Root == null || tree.Size > maxTreeSize || tree.Height > maxTreeHeight);
      return tree;
    }

    private class TreeExtensionPoint {
      public SymbolicExpressionTreeNode Parent { get; set; }
      public int ChildIndex { get; set; }
      public int ExtensionPointDepth { get; set; }
    }
    public static SymbolicExpressionTreeNode PTC2(IRandom random, ISymbolicExpressionGrammar grammar, Symbol rootSymbol,
      int size, int maxDepth, int maxFunctionDefinitions, int maxFunctionArguments) {
      SymbolicExpressionTreeNode root = rootSymbol.CreateTreeNode();
      root.Grammar = grammar;
      if (size <= 1 || maxDepth <= 1) return root;
      CreateFullTreeFromSeed(random, root, size, maxDepth, maxFunctionDefinitions, maxFunctionArguments);
      return root;
    }

    private static void CreateFullTreeFromSeed(IRandom random, SymbolicExpressionTreeNode root, int size, int maxDepth, int maxFunctionDefinitions, int maxFunctionArguments) {
      List<TreeExtensionPoint> extensionPoints = new List<TreeExtensionPoint>();
      int currentSize = 1;
      int totalListMinSize = root.Grammar.GetMinExpressionLength(root.Symbol) - 1;
      int actualArity = SampleArity(random, root, size);
      for (int i = 0; i < actualArity; i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        root.AddSubTree(dummy);
        dummy.Grammar = (ISymbolicExpressionGrammar)dummy.Grammar.Clone();
        extensionPoints.Add(new TreeExtensionPoint { Parent = root, ChildIndex = i, ExtensionPointDepth = 2 });
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
          //parent.RemoveSubTree(argumentIndex);
          //var allowedSymbols = from s in parent.Grammar.Symbols
          //                     where parent.Grammar.IsAllowedChild(parent.Symbol, s, argumentIndex)
          //                     select s;
          //SymbolicExpressionTreeNode branch = CreateMinimalTree(random, parent, allowedSymbols);
          //parent.InsertSubTree(argumentIndex, branch); // insert a smallest possible tree
          //currentSize += branch.GetSize();
          //totalListMinSize -= branch.GetSize();
        } else {
          var allowedSymbols = from s in parent.Grammar.Symbols
                               where parent.Grammar.IsAllowedChild(parent.Symbol, s, argumentIndex)
                               where parent.Grammar.GetMinExpressionDepth(s) + extensionDepth - 1 < maxDepth
                               where parent.Grammar.GetMaxExpressionLength(s) > size - totalListMinSize - currentSize
                               /*||
                                     totalListMinSize + currentSize >= size * 0.9 // if the necessary size is almost reached then also allow
                               // terminals or terminal-branches*/
                               // where !IsDynamicSymbol(s) || IsDynamicSymbolAllowed(grammar, root, parent, s)
                               select s;
          Symbol selectedSymbol = SelectRandomSymbol(random, allowedSymbols);
          SymbolicExpressionTreeNode newTree = selectedSymbol.CreateTreeNode();
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
            if (IsTopLevelBranch(root, dummy))
              dummy.Grammar = (ISymbolicExpressionGrammar)dummy.Grammar.Clone();
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
      parent.RemoveSubTree(argumentIndex);
      parent.InsertSubTree(argumentIndex, tree);
      InitializeNewTreeNode(random, root, tree, maxFunctionDefinitions, maxFunctionArguments);
      for (int i = 0; i < tree.GetMinSubtreeCount(); i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        tree.AddSubTree(dummy);
        dummy.Grammar = (ISymbolicExpressionGrammar)dummy.Grammar.Clone(); 
        // replace the just inserted dummy by recursive application
        ReplaceWithMinimalTree(random, root, tree, i, maxFunctionDefinitions, maxFunctionArguments);
      }
    }
   
    private static void InitializeNewTreeNode(IRandom random, SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode newTree, int maxFunctionDefinitions, int maxFunctionArguments) {
      // NB it is assumed that defuns are only allowed as children of root and nowhere else
      // also assumes that newTree is already attached to root somewhere
      if (IsTopLevelBranch(root, newTree))
        newTree.Grammar = (ISymbolicExpressionGrammar)newTree.Grammar.Clone();
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
          AddDynamicArguments(defunTree.Grammar, nArgs);
        }
        int argIndex = root.SubTrees.IndexOf(newTree);
        // allow invokes of ADFs with higher index
        for (int i = argIndex + 1; i < root.SubTrees.Count; i++) {
          var otherDefunNode = root.SubTrees[i] as DefunTreeNode;
          if (otherDefunNode != null) {
            var allowedParents = from sym in defunTree.Grammar.Symbols
                                 where defunTree.Grammar.IsAllowedChild(defunTree.Symbol, sym, 0)
                                 select sym;
            var allowedChildren = allowedParents;
            AddDynamicSymbol(defunTree.Grammar, allowedParents, allowedChildren, otherDefunNode.FunctionName, otherDefunNode.NumberOfArguments);
          }
        }
        // make the ADF available as symbol for other branches (with smaller index to prevent recursions)
        for (int i = 0; i < argIndex; i++) {
          // if not dummy node
          if (root.SubTrees[i].Symbol != null) {
            var topLevelGrammar = root.SubTrees[i].Grammar;
            var allowedParents = from sym in root.SubTrees[i].Grammar.Symbols
                                 where root.SubTrees[i].Grammar.IsAllowedChild(root.SubTrees[i].Symbol, sym, 0)
                                 select sym;
            var allowedChildren = allowedParents;

            AddDynamicSymbol(topLevelGrammar, allowedParents, allowedChildren, functionName, nArgs);
          }
        }
      }
    }

    private static void AddDynamicSymbol(ISymbolicExpressionGrammar grammar, IEnumerable<Symbol> allowedParents, IEnumerable<Symbol> allowedChildren, string symbolName, int nArgs) {
      var invokeSym = new InvokeFunction(symbolName);
      grammar.AddSymbol(invokeSym);
      grammar.SetMinSubtreeCount(invokeSym, nArgs);
      grammar.SetMaxSubtreeCount(invokeSym, nArgs);
      foreach (var parent in allowedParents) {
        for (int arg = 0; arg < grammar.GetMaxSubtreeCount(parent); arg++) {
          grammar.SetAllowedChild(parent, invokeSym, arg);
        }
      }
      foreach (var child in allowedChildren) {
        for (int arg = 0; arg < grammar.GetMaxSubtreeCount(invokeSym); arg++) {
          grammar.SetAllowedChild(invokeSym, child, arg);
        }
      }
    }

    private static void AddDynamicArguments(ISymbolicExpressionGrammar grammar, int nArgs) {
      for (int argIndex = 0; argIndex < nArgs; argIndex++) {
        var argNode = new Argument(argIndex);
        grammar.AddSymbol(argNode);
        grammar.SetMinSubtreeCount(argNode, 0);
        grammar.SetMaxSubtreeCount(argNode, 0);
        // allow the argument as child of any other symbol
        foreach (var symb in grammar.Symbols)
          for (int i = 0; i < grammar.GetMaxSubtreeCount(symb); i++) {
            grammar.SetAllowedChild(symb, argNode, i);
          }
      }
    }

    private static bool IsTopLevelBranch(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode branch) {
      return root.SubTrees.IndexOf(branch) > -1;
    }

    private static Symbol SelectRandomSymbol(IRandom random, IEnumerable<Symbol> symbols) {
      var symbolList = symbols.ToList();
      var ticketsSum = symbolList.Select(x => x.InitialFrequency).Sum();
      var r = random.NextDouble() * ticketsSum;
      double aggregatedTickets = 0;
      for (int i = 0; i < symbolList.Count; i++) {
        aggregatedTickets += symbolList[i].InitialFrequency;
        if (aggregatedTickets >= r) {
          return symbolList[i];
        }
      }
      // this should never happen
      throw new ArgumentException();
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