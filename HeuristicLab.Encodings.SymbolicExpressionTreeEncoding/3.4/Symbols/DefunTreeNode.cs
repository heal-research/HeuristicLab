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

using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
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

    [StorableConstructor]
    private DefunTreeNode(bool deserializing) : base(deserializing) { }
    private DefunTreeNode(DefunTreeNode original, Cloner cloner)
      : base(original, cloner) {
      functionName = original.functionName;
      numberOfArguments = original.numberOfArguments;
    }

    public DefunTreeNode(Defun defunSymbol) : base(defunSymbol) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DefunTreeNode(this, cloner);
    }

    public override string ToString() {
      return FunctionName;
    }
  }
}
