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

using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public static class GrammarModifier {
    internal static void AddInvokeSymbol(ISymbolicExpressionTreeGrammar grammar, string functionName, int nArgs, CutPoint startCutPoint, IEnumerable<CutPoint> argumentCutPoints) {
      if (!grammar.ContainsSymbol(startCutPoint.Child.Symbol)) return;

      var invokeSym = new InvokeFunction(functionName);
      grammar.AddSymbol(invokeSym);
      grammar.SetSubtreeCount(invokeSym, nArgs, nArgs);

      //allow invoke symbol everywhere, where the child of the startCutPoint was allowed
      SetAllowedParentSymbols(grammar, startCutPoint.Child.Symbol, invokeSym);

      if (nArgs > 0) {
        //set allowed child symbols of invoke symbol 
        foreach (ISymbol child in grammar.Symbols) {
          if (child.Name == invokeSym.Name) continue;
          int i = 0;
          foreach (CutPoint argumentCutPoint in argumentCutPoints) {
            if (grammar.IsAllowedChildSymbol(argumentCutPoint.Parent.Symbol, child, argumentCutPoint.ChildIndex))
              grammar.AddAllowedChildSymbol(invokeSym, child, i);
            i++;
          }
        }
      }
    }



    internal static void AddArgumentSymbol(ISymbolicExpressionTreeGrammar originalGrammar, ISymbolicExpressionTreeGrammar grammar, IEnumerable<int> argumentIndexes, IEnumerable<CutPoint> argumentCutPoints) {
      foreach (var pair in argumentIndexes.Zip(argumentCutPoints, (a, b) => new { Index = a, CutPoint = b })) {
        var argSymbol = new Argument(pair.Index);
        grammar.AddSymbol(argSymbol);
        grammar.SetSubtreeCount(argSymbol, 0, 0);

        foreach (var symb in grammar.Symbols) {
          if (symb is ProgramRootSymbol || symb is StartSymbol) continue;
          if (originalGrammar.IsAllowedChildSymbol(symb, pair.CutPoint.Child.Symbol))
            grammar.AddAllowedChildSymbol(symb, argSymbol);
          else {
            for (int i = 0; i < grammar.GetMaximumSubtreeCount(symb); i++) {
              if (originalGrammar.IsAllowedChildSymbol(symb, pair.CutPoint.Child.Symbol, i))
                grammar.AddAllowedChildSymbol(symb, argSymbol, i);
            }
          }
        }
      }
    }

    internal static void SetAllowedParentSymbols(ISymbolicExpressionTreeGrammar grammar, ISymbol symbol, ISymbol newSymbol) {
      foreach (var symb in grammar.Symbols) {
        if (symb is ProgramRootSymbol) continue;
        if (newSymbol is Argument && symb is StartSymbol) continue;
        if (grammar.IsAllowedChildSymbol(symb, symbol))
          grammar.AddAllowedChildSymbol(symb, newSymbol);
        else {
          for (int i = 0; i < grammar.GetMaximumSubtreeCount(symb); i++) {
            if (grammar.IsAllowedChildSymbol(symb, symbol, i))
              grammar.AddAllowedChildSymbol(symb, newSymbol, i);
          }
        }
      }
    }
  }
}
