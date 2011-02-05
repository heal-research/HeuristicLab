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
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionGrammar : IItem {
    Symbol StartSymbol { get; }
    void AddSymbol(Symbol symbol);
    void RemoveSymbol(Symbol symbol);
    IEnumerable<Symbol> Symbols { get; }
    bool ContainsSymbol(Symbol symbol);
    void SetAllowedChild(Symbol parent, Symbol child, int argumentIndex);
    bool IsAllowedChild(Symbol parent, Symbol child, int argumentIndex);
    int GetMinExpressionLength(Symbol start);
    int GetMaxExpressionLength(Symbol start);
    int GetMinExpressionDepth(Symbol start);
    int GetMinSubtreeCount(Symbol symbol);
    void SetMinSubtreeCount(Symbol symbol, int value);
    int GetMaxSubtreeCount(Symbol symbol);
    void SetMaxSubtreeCount(Symbol symbol, int value);
  }
}
