#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionGrammarBase : INamedItem {
    IEnumerable<ISymbol> Symbols { get; }
    IEnumerable<ISymbol> AllowedSymbols { get; }

    bool ContainsSymbol(ISymbol symbol);
    ISymbol GetSymbol(string symbolName);

    void AddSymbol(ISymbol symbol);
    void RemoveSymbol(ISymbol symbol);

    bool IsAllowedChildSymbol(ISymbol parent, ISymbol child);
    bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex);
    IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent);
    IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int argumentIndex);

    void AddAllowedChildSymbol(ISymbol parent, ISymbol child);
    void AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex);
    void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child);
    void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex);


    int GetMinimumSubtreeCount(ISymbol symbol);
    int GetMaximumSubtreeCount(ISymbol symbol);
    void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount);

    int GetMinimumExpressionDepth(ISymbol start);
    int GetMaximumExpressionDepth(ISymbol start);
    int GetMinimumExpressionLength(ISymbol start);
    int GetMaximumExpressionLength(ISymbol start, int maxDepth);

    event EventHandler Changed;
  }
}
