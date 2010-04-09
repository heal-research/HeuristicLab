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
  public sealed class DefunTreeNode : SymbolicExpressionTreeNode {
    private int numberOfArguments;
    [Storable]
    public int NumberOfArguments {
      get { return numberOfArguments; }
      set { numberOfArguments = value; }
    }
    private string name;
    [Storable]
    public string Name {
      get { return name; }
      set { this.name = value; }
    }

    // copy constructor
    private DefunTreeNode(DefunTreeNode original)
      : base(original) {
      name = original.Name;
      numberOfArguments = original.numberOfArguments;
    }

    public DefunTreeNode(Defun defunSymbol) : base(defunSymbol) { }

    public override object Clone() {
      return new DefunTreeNode(this);
    }
  }
}
