using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  public static class Util {
    public static string GetSizeDistributionString(IList<SymbolicExpressionTree> trees, int maxTreeSize, int binSize) {
      int[] histogram = new int[maxTreeSize / binSize];
      for (int i = 0; i < trees.Count; i++) {
        histogram[trees[i].Size / binSize]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * binSize);
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)trees.Count);
      }
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
  }
}
