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
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols {
  [StorableClass]
  public sealed class DefunTreeNode : SymbolicExpressionTreeTopLevelNode {
    private int numberOfArguments;
    [Storable]
    public int NumberOfArguments {
      get { return numberOfArguments; }
      set { numberOfArguments = value; }
    }
    private string functionName;
    [Storable]
    public string FunctionName {
      get { return functionName; }
      set { this.functionName = value; }
    }

    private DefunTreeNode() : base() { }

    // copy constructor
    private DefunTreeNode(DefunTreeNode original)
      : base(original) {
      functionName = original.functionName;
      numberOfArguments = original.numberOfArguments;
    }

    public DefunTreeNode(Defun defunSymbol) : base(defunSymbol) { }


    public override object Clone() {
      return new DefunTreeNode(this);
    }

    public override string ToString() {
      return FunctionName;
    }
  }
}
