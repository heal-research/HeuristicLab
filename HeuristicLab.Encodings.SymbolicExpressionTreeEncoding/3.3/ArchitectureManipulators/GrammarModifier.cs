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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  public static class GrammarModifier {
    public static void AddDynamicSymbol(ISymbolicExpressionGrammar grammar, Symbol classRepresentative, string symbolName, int nArgs) {
      var invokeSym = new InvokeFunction(symbolName);
      grammar.AddSymbol(invokeSym);
      grammar.SetMinSubtreeCount(invokeSym, nArgs);
      grammar.SetMaxSubtreeCount(invokeSym, nArgs);
      SetSyntaxConstraints(grammar, classRepresentative, invokeSym);
    }


    public static void AddDynamicArguments(ISymbolicExpressionGrammar grammar, Symbol classRepresentative, IEnumerable<int> argumentIndexes) {
      foreach (int argIndex in argumentIndexes) {
        var argSymbol = new Argument(argIndex);
        grammar.AddSymbol(argSymbol);
        grammar.SetMinSubtreeCount(argSymbol, 0);
        grammar.SetMaxSubtreeCount(argSymbol, 0);
        SetSyntaxConstraints(grammar, classRepresentative, argSymbol);
      }
    }

    private static void SetSyntaxConstraints(ISymbolicExpressionGrammar grammar, Symbol classRepresentative, Symbol newSymbol) {
      // allow symbol as child of the representative first to make sure that the symbol 
      // is in the list of parents and children
      for (int i = 0; i < grammar.GetMaxSubtreeCount(classRepresentative); i++) {
        grammar.SetAllowedChild(classRepresentative, newSymbol, i);
      }
      // for all available symbols add the new symbol as allowed child iff the available symbol is an allowed child of the class representative
      foreach (var parent in grammar.Symbols) {
        if (grammar.IsAllowedChild(classRepresentative, parent, 0))
          for (int arg = 0; arg < grammar.GetMaxSubtreeCount(parent); arg++) {
            grammar.SetAllowedChild(parent, newSymbol, arg);
          }
      }
      // for all available symbols add the new symbol as allowed parent iff the available symbol is an allowed child of the class representative
      foreach (var child in grammar.Symbols) {
        if (grammar.IsAllowedChild(classRepresentative, child, 0))
          for (int arg = 0; arg < grammar.GetMaxSubtreeCount(newSymbol); arg++) {
            grammar.SetAllowedChild(newSymbol, child, arg);
          }
      }
    }

  }
}
