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

    public static void CalculateMinimumExpressionLengths(ISymbolicExpressionGrammarBase grammar,
      Dictionary<string, int> minimumExpressionLengths) {

      minimumExpressionLengths.Clear();
      //terminal symbols => minimum expression length = 1
      foreach (var s in grammar.Symbols.Where(s => grammar.GetMinimumSubtreeCount(s) == 0))
        minimumExpressionLengths[s.Name] = 1;

      var symbolAdded = true;
      while (symbolAdded) {
        symbolAdded = false;
        foreach (var remainingSymbol in grammar.Symbols) {
          if (minimumExpressionLengths.ContainsKey(remainingSymbol.Name)) continue;

          var arguments = grammar.GetMinimumSubtreeCount(remainingSymbol);
          int minLength = 1;

          foreach (int argumentIndex in Enumerable.Range(0, arguments)) {
            var capturedMinimumLengths = minimumExpressionLengths;
            var childSymbols = grammar.GetAllowedChildSymbols(remainingSymbol, argumentIndex)
              .Where(c => c.InitialFrequency > 0.0 && capturedMinimumLengths.ContainsKey(c.Name));

            if (!childSymbols.Any()) {
              minLength = -1;
              break;
            }
            var minLengthPerArgument = childSymbols.Min(c => capturedMinimumLengths[c.Name]);
            minLength += minLengthPerArgument;
          }

          if (minLength != -1) {
            minimumExpressionLengths[remainingSymbol.Name] = minLength;
            symbolAdded = true;
          }
        }
      }

      //set minLength to int.Maxvalue for all symbols that are not reacheable
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

      var symbolAdded = true;
      while (symbolAdded) {
        symbolAdded = false;
        foreach (var remainingSymbol in grammar.Symbols) {
          if (minimumExpressionDepths.ContainsKey(remainingSymbol.Name)) continue;

          var arguments = grammar.GetMinimumSubtreeCount(remainingSymbol);
          int minDepth = -1;

          foreach (int argumentIndex in Enumerable.Range(0, arguments)) {
            var capturedMinimumDepths = minimumExpressionDepths;
            var childSymbols = grammar.GetAllowedChildSymbols(remainingSymbol, argumentIndex)
              .Where(c => c.InitialFrequency > 0.0 && capturedMinimumDepths.ContainsKey(c.Name));
            if (!childSymbols.Any()) {
              minDepth = -1;
              break;
            }
            var minDepthPerArgument = childSymbols.Min(c => capturedMinimumDepths[c.Name]);
            minDepth = Math.Max(minDepth, 1 + minDepthPerArgument);
          }

          if (minDepth != -1) {
            minimumExpressionDepths[remainingSymbol.Name] = minDepth;
            symbolAdded = true;
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
