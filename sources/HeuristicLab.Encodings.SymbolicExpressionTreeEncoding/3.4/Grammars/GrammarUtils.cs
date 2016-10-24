#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public static class GrammarUtils {
    private static IEnumerable<ISymbol> GetTopmostSymbols(this ISymbolicExpressionGrammarBase grammar) {
      // build parents list so we can find out the topmost symbol(s)
      var parents = new Dictionary<ISymbol, List<ISymbol>>();
      foreach (var symbol in grammar.Symbols.Where(x => grammar.GetMinimumSubtreeCount(x) > 0)) {
        var minSubtreeCount = grammar.GetMinimumSubtreeCount(symbol);
        for (int argIndex = 0; argIndex < minSubtreeCount; ++argIndex) {
          foreach (var childSymbol in grammar.GetAllowedActiveSymbols(symbol, argIndex)) {
            if (!parents.ContainsKey(childSymbol))
              parents[childSymbol] = new List<ISymbol>();
            parents[childSymbol].Add(symbol);
          }
        }
      }
      // the topmost symbols have no parents
      return parents.Values.SelectMany(x => x).Distinct().Where(x => !parents.ContainsKey(x));
    }

    private static IEnumerable<ISymbol> IterateBreadthReverse(this ISymbolicExpressionGrammarBase grammar, ISymbol topSymbol) {
      // sort symbols in reverse breadth order (starting from the topSymbol)
      // each symbol is visited only once (this avoids infinite recursion)
      var numberedSymbols = new List<Tuple<ISymbol, int>> { Tuple.Create(topSymbol, 0) };
      var visited = new HashSet<ISymbol> { topSymbol };
      int i = 0, index = 0;
      while (i < numberedSymbols.Count) {
        var symbol = numberedSymbols[i].Item1;
        var minSubtreeCount = grammar.GetMinimumSubtreeCount(symbol);

        for (int argIndex = 0; argIndex < minSubtreeCount; ++argIndex) {
          foreach (var childSymbol in grammar.GetAllowedActiveSymbols(symbol, argIndex)) {
            if (grammar.GetMinimumSubtreeCount(childSymbol) == 0)
              continue;

            if (visited.Add(childSymbol))
              numberedSymbols.Add(Tuple.Create(childSymbol, ++index));
          }
        }
        ++i;
      }
      numberedSymbols.Reverse();
      return numberedSymbols.Select(x => x.Item1);
    }

    private static IEnumerable<ISymbol> GetAllowedActiveSymbols(this ISymbolicExpressionGrammarBase grammar, ISymbol symbol, int argIndex) {
      return grammar.GetAllowedChildSymbols(symbol, argIndex).Where(s => s.InitialFrequency > 0);
    }

    public static void CalculateMinimumExpressionLengths(ISymbolicExpressionGrammarBase grammar,
      Dictionary<string, int> minimumExpressionLengths) {
      minimumExpressionLengths.Clear();
      //terminal symbols => minimum expression length = 1
      foreach (var s in grammar.Symbols.Where(x => grammar.GetMinimumSubtreeCount(x) == 0))
        minimumExpressionLengths[s.Name] = 1;

      foreach (var topSymbol in grammar.GetTopmostSymbols()) {
        // get all symbols below in reverse breadth order
        // this way we ensure lengths are calculated bottom-up
        var symbols = grammar.IterateBreadthReverse(topSymbol).ToList();
        foreach (var symbol in symbols) {
          long minLength = 1;
          for (int argIndex = 0; argIndex < grammar.GetMinimumSubtreeCount(symbol); ++argIndex) {
            long length = grammar.GetAllowedActiveSymbols(symbol, argIndex)
              .Where(x => minimumExpressionLengths.ContainsKey(x.Name))
              .Select(x => minimumExpressionLengths[x.Name]).DefaultIfEmpty(int.MaxValue).Min();
            minLength += length;
          }
          int oldLength;
          if (minimumExpressionLengths.TryGetValue(symbol.Name, out oldLength))
            minLength = Math.Min(minLength, oldLength);
          minimumExpressionLengths[symbol.Name] = (int)Math.Min(int.MaxValue, minLength);
        }
        // correction step for cycles
        bool changed = true;
        while (changed) {
          changed = false;
          foreach (var symbol in symbols) {
            long minLength = Enumerable.Range(0, grammar.GetMinimumSubtreeCount(symbol))
              .Sum(x => grammar.GetAllowedActiveSymbols(symbol, x)
              .Select(s => (long)minimumExpressionLengths[s.Name]).DefaultIfEmpty(int.MaxValue).Min()) + 1;
            if (minLength < minimumExpressionLengths[symbol.Name]) {
              minimumExpressionLengths[symbol.Name] = (int)Math.Min(minLength, int.MaxValue);
              changed = true;
            }
          }
        }
      }

      //set minLength to int.MaxValue for all symbols that are not reacheable
      foreach (var remainingSymbols in grammar.Symbols) {
        if (!minimumExpressionLengths.ContainsKey(remainingSymbols.Name))
          minimumExpressionLengths[remainingSymbols.Name] = int.MaxValue;
      }
    }

    public static void CalculateMinimumExpressionDepth(ISymbolicExpressionGrammarBase grammar,
      Dictionary<string, int> minimumExpressionDepths) {

      minimumExpressionDepths.Clear();
      //terminal symbols => minimum expression depth = 1
      foreach (var s in grammar.Symbols.Where(s => grammar.GetMinimumSubtreeCount(s) == 0))
        minimumExpressionDepths[s.Name] = 1;

      foreach (var topSymbol in grammar.GetTopmostSymbols()) {
        // get all symbols below in reverse breadth order
        // this way we ensure lengths are calculated bottom-up
        var symbols = grammar.IterateBreadthReverse(topSymbol).ToList();
        foreach (var symbol in symbols) {
          long minDepth = -1;
          for (int argIndex = 0; argIndex < grammar.GetMinimumSubtreeCount(symbol); ++argIndex) {
            long depth = grammar.GetAllowedActiveSymbols(symbol, argIndex)
              .Where(x => minimumExpressionDepths.ContainsKey(x.Name))
              .Select(x => (long)minimumExpressionDepths[x.Name]).DefaultIfEmpty(int.MaxValue).Min() + 1;
            minDepth = Math.Max(minDepth, depth);
          }
          int oldDepth;
          if (minimumExpressionDepths.TryGetValue(symbol.Name, out oldDepth))
            minDepth = Math.Min(minDepth, oldDepth);
          minimumExpressionDepths[symbol.Name] = (int)Math.Min(int.MaxValue, minDepth);
        }
        // correction step for cycles
        bool changed = true;
        while (changed) {
          changed = false;
          foreach (var symbol in symbols) {
            long minDepth = Enumerable.Range(0, grammar.GetMinimumSubtreeCount(symbol))
              .Max(x => grammar.GetAllowedActiveSymbols(symbol, x)
              .Select(s => (long)minimumExpressionDepths[s.Name]).DefaultIfEmpty(int.MaxValue).Min()) + 1;
            if (minDepth < minimumExpressionDepths[symbol.Name]) {
              minimumExpressionDepths[symbol.Name] = (int)Math.Min(minDepth, int.MaxValue);
              changed = true;
            }
          }
        }
      }

      //set minDepth to int.Maxvalue for all symbols that are not reacheable
      foreach (var remainingSymbols in grammar.Symbols) {
        if (!minimumExpressionDepths.ContainsKey(remainingSymbols.Name))
          minimumExpressionDepths[remainingSymbols.Name] = int.MaxValue;
      }
    }
  }
}
