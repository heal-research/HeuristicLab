#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public sealed class ArgumentTreeNode : SymbolicExpressionTreeTerminalNode {
    internal new Argument Symbol {
      get { return (Argument)base.Symbol; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (!(value is Argument)) throw new ArgumentException();
        base.Symbol = value;
      }
    }

    [StorableConstructor]
    private ArgumentTreeNode(bool deserializing) : base(deserializing) { }
    private ArgumentTreeNode(ArgumentTreeNode original, Cloner cloner) : base(original, cloner) { }
    public ArgumentTreeNode(Argument argSymbol) : base(argSymbol) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArgumentTreeNode(this, cloner);
    }
  }
}
