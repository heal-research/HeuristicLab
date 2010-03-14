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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter which represents an operator.
  /// </summary>
  [Item("OperatorParameter", "A parameter which represents an operator.")]
  [StorableClass]
  [Creatable("Test")]
  public class OperatorParameter : OptionalValueParameter<IOperator> {
    public OperatorParameter()
      : base("Anonymous") {
    }
    public OperatorParameter(string name)
      : base(name) {
    }
    public OperatorParameter(string name, IOperator value)
      : base(name, value) {
      Value = value;
    }
    public OperatorParameter(string name, string description)
      : base(name, description) {
    }
    public OperatorParameter(string name, string description, IOperator value)
      : base(name, description, value) {
      Value = value;
    }
  }
}
