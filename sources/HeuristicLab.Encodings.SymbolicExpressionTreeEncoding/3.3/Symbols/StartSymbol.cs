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

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols {
  [StorableClass]
  [Item(StartSymbol.StartSymbolName, StartSymbol.StartSymbolDescription)]
  public sealed class StartSymbol : ReadOnlySymbol {
    public const string StartSymbolName = "StartSymbol";
    public const string StartSymbolDescription = "Special symbol that represents the starting node of the result producing branch of a symbolic expression tree.";

    public StartSymbol() : base(StartSymbol.StartSymbolName, StartSymbol.StartSymbolDescription) { }
    [StorableConstructor]
    private StartSymbol(bool deserializing) : base(deserializing) { }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new SymbolicExpressionTreeTopLevelNode(this);
    }
  }
}
