#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionGrammar : ISymbolicExpressionGrammarBase, IStatefulItem {
    ISymbol ProgramRootSymbol { get; }
    ISymbol StartSymbol { get; }

    int MinimumFunctionDefinitions { get; set; }
    int MaximumFunctionDefinitions { get; set; }
    int MinimumFunctionArguments { get; set; }
    int MaximumFunctionArguments { get; set; }

    bool ReadOnly { get; set; }
    event EventHandler ReadOnlyChanged;

    void AddSymbol(ISymbol symbol);
    void RemoveSymbol(ISymbol symbol);

    void AddAllowedChildSymbol(ISymbol parent, ISymbol child);
    void AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex);
    void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child);
    void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex);

    void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount);

    void StartGrammarManipulation();
    void FinishedGrammarManipulation();
  }
}
