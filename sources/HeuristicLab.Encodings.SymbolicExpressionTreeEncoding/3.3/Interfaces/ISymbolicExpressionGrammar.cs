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
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionGrammar : IItem {
    Symbol StartSymbol { get; }
    Symbol ProgramRootSymbol { get; }
    IEnumerable<Symbol> GetAllowedSymbols(Symbol parent, int argumentIndex);
    int GetMinExpressionLength(Symbol start);
    int GetMaxExpressionLength(Symbol start);
    int GetMinExpressionDepth(Symbol start);
    int GetMinSubTreeCount(Symbol start);
    int GetMaxSubTreeCount(Symbol start);

    bool IsValidExpression(SymbolicExpressionTree expression);
  }
}
