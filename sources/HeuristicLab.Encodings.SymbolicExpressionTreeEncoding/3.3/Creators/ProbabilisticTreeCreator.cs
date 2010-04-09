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
    public ProbabilisticTreeCreator()
      : base() {
    }

    protected override SymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight) {
      return Create(random, grammar, maxTreeSize.Value, maxTreeHeight.Value);
    }

    public static SymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeSize, int maxTreeHeight) {
      // tree size is limited by the grammar and by the explicit size constraints
      int allowedMinSize = grammar.GetMinExpressionLength(grammar.StartSymbol);
      int allowedMaxSize = Math.Min(maxTreeSize, grammar.GetMaxExpressionLength(grammar.StartSymbol));
      // select a target tree size uniformly in the possible range (as determined by explicit limits and limits of the grammar)
      int treeSize = random.Next(allowedMinSize, allowedMaxSize);
      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      do {
        try {
          tree.Root = grammar.ProgramRootSymbol.CreateTreeNode();
          tree.Root.AddSubTree(PTC2(random, grammar, grammar.StartSymbol, treeSize + 1, maxTreeHeight + 1));
        }
        catch (ArgumentException) {
          // try a different size
          treeSize = random.Next(allowedMinSize, allowedMaxSize);
        }
      } while (tree.Root.SubTrees.Count == 0 || tree.Size > maxTreeSize || tree.Height > maxTreeHeight);
      System.Diagnostics.Debug.Assert(grammar.IsValidExpression(tree));
      return tree;
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


    public static SymbolicExpressionTreeNode PTC2(IRandom random, ISymbolicExpressionGrammar grammar, Symbol rootSymbol, int size, int maxDepth) {
      SymbolicExpressionTreeNode root = rootSymbol.CreateTreeNode();
      if (size <= 1 || maxDepth <= 1) return root;
      List<object[]> extensionPoints = new List<object[]>();
      int currentSize = 1;
      int totalListMinSize = grammar.GetMinExpressionLength(rootSymbol) - 1;

      int actualArity = SampleArity(random, grammar, rootSymbol, size);
      for (int i = 0; i < actualArity; i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        root.AddSubTree(null);
        extensionPoints.Add(new object[] { root, i, 2 });
      }

      // while there are pending extension points and we have not reached the limit of adding new extension points
      while (extensionPoints.Count > 0 && totalListMinSize + currentSize < size) {
        int randomIndex = random.Next(extensionPoints.Count);
        object[] nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = (SymbolicExpressionTreeNode)nextExtension[0];
        int argumentIndex = (int)nextExtension[1];
        int extensionDepth = (int)nextExtension[2];
        if (extensionDepth + grammar.GetMinExpressionDepth(parent.Symbol) >= maxDepth) {
          parent.RemoveSubTree(argumentIndex);
          SymbolicExpressionTreeNode branch = CreateMinimalTree(random, grammar, grammar.GetAllowedSymbols(parent.Symbol, argumentIndex));
          parent.InsertSubTree(argumentIndex, branch); // insert a smallest possible tree
          currentSize += branch.GetSize();
          totalListMinSize -= branch.GetSize();
        } else {
          var allowedSubFunctions = from s in grammar.GetAllowedSymbols(parent.Symbol, argumentIndex)
                                    where grammar.GetMinExpressionDepth(parent.Symbol) + extensionDepth - 1 < maxDepth
                                    where grammar.GetMaxExpressionLength(s) > size - totalListMinSize - currentSize ||
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
            extensionPoints.Add(new object[] { newTree, i, extensionDepth + 1 });
          }
          totalListMinSize += grammar.GetMinExpressionLength(selectedSymbol);
        }
      }
      // fill all pending extension points
      while (extensionPoints.Count > 0) {
        int randomIndex = random.Next(extensionPoints.Count);
        object[] nextExtension = extensionPoints[randomIndex];
        extensionPoints.RemoveAt(randomIndex);
        SymbolicExpressionTreeNode parent = (SymbolicExpressionTreeNode)nextExtension[0];
        int a = (int)nextExtension[1];
        int d = (int)nextExtension[2];
        parent.RemoveSubTree(a);
        parent.InsertSubTree(a,
          CreateMinimalTree(random, grammar, grammar.GetAllowedSymbols(parent.Symbol, a))); // append a tree with minimal possible height
      }
      return root;
    }

    private static int SampleArity(IRandom random, ISymbolicExpressionGrammar grammar, Symbol symbol, int targetSize) {
      // select actualArity randomly with the constraint that the sub-trees in the minimal arity can become large enough
      int minArity = grammar.GetMinSubTreeCount(symbol);
      int maxArity = grammar.GetMaxSubTreeCount(symbol);
      if (maxArity > targetSize) {
        maxArity = targetSize;
      }
      // the min number of sub-trees has to be set to a value that is large enough so that the largest possible tree is at least tree size
      // if 1..3 trees are possible and the largest possible first sub-tree is smaller larger than the target size then minArity should be at least 2
      long aggregatedLongestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedLongestExpressionLength += (from s in grammar.GetAllowedSymbols(symbol, i)
                                              select grammar.GetMaxExpressionLength(s)).Max();
        if (aggregatedLongestExpressionLength < targetSize) minArity = i;
        else break;
      }

      // the max number of sub-trees has to be set to a value that is small enough so that the smallest possible tree is at most tree size 
      // if 1..3 trees are possible and the smallest possible first sub-tree is already larger than the target size then maxArity should be at most 0
      long aggregatedShortestExpressionLength = 0;
      for (int i = 0; i < maxArity; i++) {
        aggregatedShortestExpressionLength += (from s in grammar.GetAllowedSymbols(symbol, i)
                                               select grammar.GetMinExpressionLength(s)).Min();
        if (aggregatedShortestExpressionLength > targetSize) {
          maxArity = i;
          break;
        }
      }
      if (minArity > maxArity) throw new ArgumentException();
      return random.Next(minArity, maxArity + 1);
    }

    private static SymbolicExpressionTreeNode CreateMinimalTree(IRandom random, ISymbolicExpressionGrammar grammar, IEnumerable<Symbol> symbols) {
      // determine possible symbols that will lead to the smallest possible tree
      var possibleSymbols = (from s in symbols
                             group s by grammar.GetMinExpressionLength(s) into g
                             orderby g.Key
                             select g).First();
      var selectedSymbol = SelectRandomSymbol(random, possibleSymbols);
      // build minimal tree by recursive application
      var tree = selectedSymbol.CreateTreeNode();
      for (int i = 0; i < grammar.GetMinSubTreeCount(selectedSymbol); i++)
        tree.AddSubTree(CreateMinimalTree(random, grammar, grammar.GetAllowedSymbols(selectedSymbol, i)));
      return tree;
    }
  }
}