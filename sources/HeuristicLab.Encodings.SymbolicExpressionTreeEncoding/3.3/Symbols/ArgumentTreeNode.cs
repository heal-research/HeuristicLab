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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols {
  [StorableClass]
  public sealed class ArgumentTreeNode : SymbolicExpressionTreeNode {
    internal new Argument Symbol {
      get { return (Argument)base.Symbol; }
      set {
        if (value == null) throw new ArgumentNullException();
        if(!(value is Argument)) throw new ArgumentException();
        base.Symbol = value; 
      }
    }
    
    // copy constructor
    private ArgumentTreeNode(ArgumentTreeNode original)
      : base(original) {
    }

    public ArgumentTreeNode(Argument argSymbol) : base(argSymbol) { }

    public override object Clone() {
      return new ArgumentTreeNode(this);
    }
  }
}
