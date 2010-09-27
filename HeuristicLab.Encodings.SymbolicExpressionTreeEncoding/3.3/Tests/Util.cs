#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  public static class Util {
    public static string GetSizeDistributionString(IList<SymbolicExpressionTree> trees, int maxTreeSize, int binSize) {
      int[] histogram = new int[maxTreeSize / binSize];
      for (int i = 0; i < trees.Count; i++) {
        int binIndex = Math.Min(histogram.Length - 1, trees[i].Size / binSize);
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

    public static string GetFunctionDistributionString(IList<SymbolicExpressionTree> trees) {
      Dictionary<string, int> occurances = new Dictionary<string, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (node.SubTrees.Count > 0) {
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

    public static string GetNumberOfSubTreesDistributionString(IList<SymbolicExpressionTree> trees) {
      Dictionary<int, int> occurances = new Dictionary<int, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (!occurances.ContainsKey(node.SubTrees.Count))
            occurances[node.SubTrees.Count] = 0;
          occurances[node.SubTrees.Count]++;
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


    public static string GetTerminalDistributionString(IList<SymbolicExpressionTree> trees) {
      Dictionary<string, int> occurances = new Dictionary<string, int>();
      double n = 0.0;
      for (int i = 0; i < trees.Count; i++) {
        foreach (var node in trees[i].IterateNodesPrefix()) {
          if (node.SubTrees.Count == 0) {
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

    public static void IsValid(SymbolicExpressionTree tree) {
      int reportedSize = tree.Size;
      int actualSize = tree.IterateNodesPostfix().Count();
      Assert.AreEqual(actualSize, reportedSize);

      foreach (var defunTreeNode in tree.Root.SubTrees.OfType<DefunTreeNode>()) {
        int arity = defunTreeNode.NumberOfArguments;
        var invoke = new InvokeFunction(defunTreeNode.FunctionName);
        foreach (var otherRootNode in tree.Root.SubTrees) {
          if (otherRootNode.Grammar.ContainsSymbol(invoke)) {
            Assert.IsTrue(otherRootNode.Grammar.GetMinSubtreeCount(invoke) == arity);
            Assert.IsTrue(otherRootNode.Grammar.GetMaxSubtreeCount(invoke) == arity);
          }
        }
      }
      //Assert.AreEqual(tree.Root.Symbol, tree.Root.Grammar.StartSymbol);
      //foreach (var subtree in tree.Root.SubTrees)
      //  Assert.AreNotSame(subtree.Grammar, tree.Root.Grammar);
      IsValid(tree.Root);
    }

    public static void IsValid(SymbolicExpressionTreeNode treeNode) {
      var matchingSymbol = (from symb in treeNode.Grammar.Symbols
                            where symb.Name == treeNode.Symbol.Name
                            select symb).SingleOrDefault();
      Assert.IsTrue(treeNode.SubTrees.Count >= treeNode.Grammar.GetMinSubtreeCount(matchingSymbol));
      Assert.IsTrue(treeNode.SubTrees.Count <= treeNode.Grammar.GetMaxSubtreeCount(matchingSymbol));
      for (int i = 0; i < treeNode.SubTrees.Count; i++) {
        Assert.IsTrue(treeNode.GetAllowedSymbols(i).Select(x => x.Name).Contains(treeNode.SubTrees[i].Symbol.Name));
        IsValid(treeNode.SubTrees[i]);
      }
    }
  }
}
