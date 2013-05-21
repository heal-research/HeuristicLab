#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_34.Tests {
  public static class Util {
    public static string GetSizeDistributionString(IList<ISymbolicExpressionTree> trees, int maxTreeLength, int binSize) {
      int[] histogram = new int[maxTreeLength / binSize];
      for (int i = 0; i < trees.Count; i++) {
        int binIndex = Math.Min(histogram.Length - 1, trees[i].Length / binSize);
        histogram[binIndex]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length - 1; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * binSize);
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)trees.Count);
      }
      strBuilder.Append(Environment.NewLine);
      strBuilder.Append(">= "); strBuilder.Append(histogram.Length * binSize);
      strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[histogram.Length - 1] / (double)trees.Count);

      return "Size distribution: " + strBuilder;
    }

    public static string GetFunctionDistributionString(IList<ISymbolicExpressionTree> trees) {
      Dictionary<string, int> occurances = new Dictionary<string, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (node.Subtrees.Count() > 0) {
            if (!occurances.ContainsKey(node.Symbol.Name))
              occurances[node.Symbol.Name] = 0;
            occurances[node.Symbol.Name]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      return "Function distribution: " + strBuilder;
    }

    public static string GetNumberOfSubtreesDistributionString(IList<ISymbolicExpressionTree> trees) {
      Dictionary<int, int> occurances = new Dictionary<int, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (!occurances.ContainsKey(node.Subtrees.Count()))
            occurances[node.Subtrees.Count()] = 0;
          occurances[node.Subtrees.Count()]++;
          n++;
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var arity in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(arity); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[arity] / n);
      }
      return "Distribution of function arities: " + strBuilder;
    }


    public static string GetTerminalDistributionString(IList<ISymbolicExpressionTree> trees) {
      Dictionary<string, int> occurances = new Dictionary<string, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (node.Subtrees.Count() == 0) {
            if (!occurances.ContainsKey(node.Symbol.Name))
              occurances[node.Symbol.Name] = 0;
            occurances[node.Symbol.Name]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      return "Terminal distribution: " + strBuilder;
    }

    public static void IsValid(ISymbolicExpressionTree tree) {
      int reportedSize = tree.Length;
      int actualSize = tree.IterateNodesPostfix().Count();
      Assert.AreEqual(actualSize, reportedSize);

      foreach (var defunTreeNode in tree.Root.Subtrees.OfType<DefunTreeNode>()) {
        int arity = defunTreeNode.NumberOfArguments;

        foreach (var argTreenode in defunTreeNode.IterateNodesPrefix().OfType<ArgumentTreeNode>()) {
          Assert.IsTrue(argTreenode.SubtreeCount == 0);
          Assert.IsTrue(((Argument)argTreenode.Symbol).ArgumentIndex < arity);
        }

        foreach (var argSymbol in Enumerable.Range(0, defunTreeNode.NumberOfArguments).Select(x => new Argument(x))) {
          Assert.IsTrue(defunTreeNode.Grammar.ContainsSymbol(argSymbol));
          Assert.IsTrue(defunTreeNode.Grammar.GetMaximumSubtreeCount(argSymbol) == 0);
          Assert.IsTrue(defunTreeNode.Grammar.GetMinimumSubtreeCount(argSymbol) == 0);
        }

        var invoke = new InvokeFunction(defunTreeNode.FunctionName);
        foreach (var otherRootNode in tree.Root.Subtrees) {
          if (otherRootNode.Grammar.ContainsSymbol(invoke)) {
            Assert.IsTrue(otherRootNode.Grammar.GetMinimumSubtreeCount(invoke) == arity);
            Assert.IsTrue(otherRootNode.Grammar.GetMaximumSubtreeCount(invoke) == arity);
            Assert.IsFalse(otherRootNode.Grammar.IsAllowedChildSymbol(invoke, invoke));
            for (int i = 0; i < arity; i++) {
              Assert.IsFalse(otherRootNode.Grammar.IsAllowedChildSymbol(invoke, invoke, i));
            }
          }
        }

      }

      foreach (var subtree in tree.Root.Subtrees) {
        Assert.AreNotSame(subtree.Grammar, tree.Root.Grammar);
        IsValid(subtree.Grammar);
      }

      IsValid(tree.Root.Grammar);
      IsValid(tree.Root);
    }

    public static void IsValid(ISymbolicExpressionTreeGrammar grammar) {
      Assert.IsTrue(grammar.Symbols.Count() == grammar.Symbols.Distinct().Count());
      foreach (ISymbol symbol in grammar.AllowedSymbols) {
        Assert.IsTrue(grammar.GetAllowedChildSymbols(symbol).Count() == grammar.GetAllowedChildSymbols(symbol).Distinct().Count());
        for (int i = 0; i < grammar.GetMaximumSubtreeCount(symbol); i++) {
          Assert.IsTrue(grammar.GetAllowedChildSymbols(symbol, i).Count() == grammar.GetAllowedChildSymbols(symbol, i).Distinct().Count());
        }
      }

      foreach (var symbol in grammar.ModifyableSymbols) {
        //check if every symbol has at least one allowed child
        for (int i = 0; i < grammar.GetMaximumSubtreeCount(symbol); i++)
          Assert.IsTrue(grammar.GetAllowedChildSymbols(symbol, i).Any());
      }
    }

    public static void IsValid(ISymbolicExpressionTreeNode treeNode) {
      var matchingSymbol = (from symb in treeNode.Grammar.Symbols
                            where symb.Name == treeNode.Symbol.Name
                            select symb).SingleOrDefault();
      Assert.IsTrue(treeNode.Subtrees.Count() >= treeNode.Grammar.GetMinimumSubtreeCount(matchingSymbol));
      Assert.IsTrue(treeNode.Subtrees.Count() <= treeNode.Grammar.GetMaximumSubtreeCount(matchingSymbol));
      Assert.AreNotEqual(0.0, matchingSymbol.InitialFrequency); // check that no deactivated symbols occur in the tree
      for (int i = 0; i < treeNode.Subtrees.Count(); i++) {
        Assert.IsTrue(treeNode.Grammar.GetAllowedChildSymbols(treeNode.Symbol, i).Select(x => x.Name).Contains(treeNode.GetSubtree(i).Symbol.Name));
        IsValid(treeNode.GetSubtree(i));
      }
    }
  }
}
