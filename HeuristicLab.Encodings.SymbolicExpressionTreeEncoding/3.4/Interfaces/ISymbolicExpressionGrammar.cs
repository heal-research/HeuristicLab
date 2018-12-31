#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("1f6afcbe-b309-44e2-8d35-2d33eaeb9649")]
  public interface ISymbolicExpressionGrammar : ISymbolicExpressionGrammarBase, IStatefulItem {
    ISymbol ProgramRootSymbol { get; }
    ISymbol StartSymbol { get; }

    int MinimumFunctionDefinitions { get; set; }
    int MaximumFunctionDefinitions { get; set; }
    int MinimumFunctionArguments { get; set; }
    int MaximumFunctionArguments { get; set; }

    bool ReadOnly { get; set; }
    event EventHandler ReadOnlyChanged;

    void StartGrammarManipulation();
    void FinishedGrammarManipulation();

    ISymbolicExpressionTreeGrammar CreateExpressionTreeGrammar();
  }
}
