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
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter which represents an operator.
  /// </summary>
  [Item("OperatorParameter", "A parameter which represents an operator.")]
  [StorableType("59C86C8F-CAE1-45FE-A17C-837925284D93")]
  public class OperatorParameter : OptionalValueParameter<IOperator> {
    [StorableConstructor]
    protected OperatorParameter(StorableConstructorFlag _) : base(_) { }
    protected OperatorParameter(OperatorParameter original, Cloner cloner) : base(original, cloner) { }
    public OperatorParameter() : base("Anonymous") { }
    public OperatorParameter(string name) : base(name) { }
    public OperatorParameter(string name, bool getsCollected) : base(name, getsCollected) { }
    public OperatorParameter(string name, IOperator value) : base(name, value) { }
    public OperatorParameter(string name, IOperator value, bool getsCollected) : base(name, value, getsCollected) { }
    public OperatorParameter(string name, string description) : base(name, description) { }
    public OperatorParameter(string name, string description, bool getsCollected) : base(name, description, getsCollected) { }
    public OperatorParameter(string name, string description, IOperator value) : base(name, description, value) { }
    public OperatorParameter(string name, string description, IOperator value, bool getsCollected) : base(name, description, value, getsCollected) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OperatorParameter(this, cloner);
    }
  }
}
