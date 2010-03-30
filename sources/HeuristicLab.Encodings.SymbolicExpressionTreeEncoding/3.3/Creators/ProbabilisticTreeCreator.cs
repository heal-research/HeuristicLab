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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("ProbabilisticTreeCreator", "An operator that creates new symbolic expression trees with uniformly distributed size")]
  public class ProbabilisticTreeCreator : SymbolicExpressionTreeCreator {
    private const int MAX_TRIES = 100;
    public ProbabilisticTreeCreator()
      : base() {
    }

    protected override SymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight) {
      return Apply(random, grammar, maxTreeSize.Value, maxTreeHeight.Value);
    }

    public SymbolicExpressionTree Apply(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeSize, int maxTreeHeight) {
      // tree size is limited by the grammar and by the explicit size constraints
      int allowedMinSize = grammar.MinimalExpressionLength(grammar.StartSymbol);
      int allowedMaxSize = Math.Min(maxTreeSize, grammar.MaximalExpressionLength(grammar.StartSymbol));
      // select a target tree size uniformly in the possible range (as determined by explicit limits and limits of the grammar)
      int treeSize = random.Next(allowedMinSize, allowedMaxSize);
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      int tries = 0;
      do {
        try {
          tree.Root = PTC2(random, grammar, grammar.StartSymbol, treeSize+1, maxTreeHeight+1);
          //// determine possible root symbols to create a tree of the target size
          //var possibleRootSymbols = from symbol in grammar.AllowedSymbols(grammar.StartSymbol, 0)
          //                          where treeSize <= grammar.MaximalExpressionLength(symbol)
          //                          where treeSize >= grammar.MinimalExpressionLength(symbol)
          //                          select symbol;
          //Symbol rootSymbol = SelectRandomSymbol(random, possibleRootSymbols);
          //tree.Root = PTC2(random, grammar, rootSymbol, treeSize, maxTreeHeight);
        }
        catch (ArgumentException) {
          // try a different size
          treeSize = random.Next(allowedMinSize, allowedMaxSize);
          tries = 0;
        }
        if (tries++ >= MAX_TRIES) {
          // try a different size
          treeSize = random.Next(allowedMinSize, allowedMaxSize);
          tries = 0;
        }
      } while (tree.Root == null || tree.Size > maxTreeSize || tree.Height > maxTreeHeight);
      return tree;
    }

    private Symbol SelectRandomSymbol(IRandom random, IEnumerable<Symbol> symbols) {
      var symbolList = symbols.ToList();
      return symbolList[random.Next(symbolList.Count)];
    }


    private SymbolicExpressionTreeNode PTC2(IRandom random, ISymbolicExpressionGrammar grammar, Symbol rootSymbol, int size, int maxDepth) {
      SymbolicExpressionTreeNode root = rootSymbol.CreateTreeNode();
      if (size <= 1 || maxDepth <= 1) return root;
      List<object[]> list = new List<object[]>();
      int currentSize = 1;
      int totalListMinSize = grammar.MinimalExpressionLength(rootSymbol) - 1;

      int actualArity = SampleArity(random, grammar, rootSymbol, size);
      for (int i = 0; i < actualArity; i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        root.AddSubTree(null);
        list.Add(new object[] { root, i, 2 });
      }

      // while there are pending extension points and we have not reached the limit of adding new extension points
      while (list.Count > 0 && totalListMinSize + currentSize < size) {
        int randomIndex = random.Next(list.Count);
        object[] nextExtension = list[randomIndex];
        list.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = (SymbolicExpressionTreeNode)nextExtension[0];
        int argumentIndex = (int)nextExtension[1];
        int extensionDepth = (int)nextExtension[2];
        if (extensionDepth + grammar.MinimalExpressionDepth(parent.Symbol) >= maxDepth) {
          parent.RemoveSubTree(argumentIndex);
          SymbolicExpressionTreeNode branch = CreateMinimalTree(random, grammar, grammar.AllowedSymbols(parent.Symbol, argumentIndex));
          parent.InsertSubTree(argumentIndex, branch); // insert a smallest possible tree
          currentSize += branch.GetSize();
          totalListMinSize -= branch.GetSize();
        } else {
          var allowedSubFunctions = from s in grammar.AllowedSymbols(parent.Symbol, argumentIndex)
                                    where grammar.MinimalExpressionDepth(parent.Symbol) + extensionDepth - 1 < maxDepth
                                    where grammar.MaximalExpressionLength(s) > size - totalListMinSize - currentSize ||
                                          totalListMinSize + currentSize >= size * 0.9 // if the necessary size is almost reached then also allow
                                    // terminals or terminal-branches
                                    select s;
          Symbol selectedSymbol = SelectRandomSymbol(random, allowedSubFunctions);
          SymbolicExpressionTreeNode newTree = selectedSymbol.CreateTreeNode();
          parent.RemoveSubTree(argumentIndex);
          parent.InsertSubTree(argumentIndex, newTree);
          currentSize++;
          totalListMinSize--;

          actualArity = SampleArity(random, grammar, selectedSymbol, size - currentSize);
          for (int i = 0; i < actualArity; i++) {
            // insert a dummy sub-tree and add the pending extension to the list
            newTree.AddSubTree(null);
            list.Add(new object[] { newTree, i, extensionDepth + 1 });
          }
          totalListMinSize += grammar.MinimalExpressionLength(selectedSymbol);
        }
      }
      // fill all pending extension points
      while (list.Count > 0) {
        int randomIndex = random.Next(list.Count);
        object[] nextExtension = list[randomIndex];
        list.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = (SymbolicExpressionTreeNode)nextExtension[0];
        int a = (int)nextExtension[1];
        int d = (int)nextExtension[2];
        parent.RemoveSubTree(a);
        parent.InsertSubTree(a,
          CreateMinimalTree(random, grammar, grammar.AllowedSymbols(parent.Symbol, a))); // append a tree with minimal possible height
      }
      return root;
    }

    private int SampleArity(IRandom random, ISymbolicExpressionGrammar grammar, Symbol symbol, int targetSize) {
      // select actualArity randomly with the constraint that the sub-trees in the minimal arity can become large enough
      int minArity = grammar.MinSubTrees(symbol);
      int maxArity = grammar.MaxSubTrees(symbol);
      if (maxArity > targetSize) {
        maxArity = targetSize;
      }
      // the min number of sub-trees has to be set to a value that is large enough so that the largest possible tree is at least tree size
      // if 1..3 trees are possible and the largest possible first sub-tree is smaller larger than the target size then minArity should be at least 2
      long aggregatedLongestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedLongestExpressionLength += (from s in grammar.AllowedSymbols(symbol, i)
                                              select grammar.MaximalExpressionLength(s)).Max();
        if (aggregatedLongestExpressionLength < targetSize) minArity = i;
        else break;
      }

      // the max number of sub-trees has to be set to a value that is small enough so that the smallest possible tree is at most tree size 
      // if 1..3 trees are possible and the smallest possible first sub-tree is already larger than the target size then maxArity should be at most 0
      long aggregatedShortestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedShortestExpressionLength += (from s in grammar.AllowedSymbols(symbol, i)
                                               select grammar.MinimalExpressionLength(s)).Min();
        if (aggregatedShortestExpressionLength > targetSize) {
          maxArity = i;
          break;
        }
      }
      if (minArity > maxArity) throw new ArgumentException();
      return random.Next(minArity, maxArity + 1);
    }

    private SymbolicExpressionTreeNode CreateMinimalTree(IRandom random, ISymbolicExpressionGrammar grammar, IEnumerable<Symbol> symbols) {
      // determine possible symbols that will lead to the smallest possible tree
      var possibleSymbols = (from s in symbols
                             group s by grammar.MinimalExpressionLength(s) into g
                             orderby g.Key
                             select g).First();
      var selectedSymbol = SelectRandomSymbol(random, possibleSymbols);
      // build minimal tree by recursive application
      var tree = selectedSymbol.CreateTreeNode();
      for (int i = 0; i < grammar.MinSubTrees(selectedSymbol); i++)
        tree.AddSubTree(CreateMinimalTree(random, grammar, grammar.AllowedSymbols(selectedSymbol, i)));
      return tree;
    }

    //private bool IsRecursiveExpansionPossible(Symbol symbol) {
    //  return FindCycle(function, new Stack<IFunction>());
    //}

    //private Dictionary<IFunction, bool> inCycle = new Dictionary<IFunction, bool>();
    //private bool FindCycle(IFunction function, Stack<IFunction> functionChain) {
    //  if (inCycle.ContainsKey(function)) {
    //    return inCycle[function];
    //  } else if (IsTerminal(function)) {
    //    inCycle[function] = false;
    //    return false;
    //  } else if (functionChain.Contains(function)) {
    //    inCycle[function] = true;
    //    return true;
    //  } else {
    //    functionChain.Push(function);
    //    bool result = false;
    //    // all slot indexes
    //    for (int i = 0; i < function.MaxSubTrees; i++) {
    //      foreach (IFunction subFunction in GetAllowedSubFunctions(function, i)) {
    //        result |= FindCycle(subFunction, functionChain);
    //      }
    //    }

    //    functionChain.Pop();
    //    inCycle[function] = result;
    //    return result;
    //  }
    //}

  }
}