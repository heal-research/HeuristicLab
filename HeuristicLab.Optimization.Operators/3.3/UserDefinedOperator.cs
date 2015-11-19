#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  [Item("UserDefinedOperator", "An operator that can be parameterized with multiple operators which will be executed one after another.")]
  [StorableClass]
  public abstract class UserDefinedOperator : CheckedMultiOperator<IOperator> {
    [StorableConstructor]
    protected UserDefinedOperator(bool deserializing) : base(deserializing) { }
    protected UserDefinedOperator(UserDefinedOperator original, Cloner cloner) : base(original, cloner) { }
    public UserDefinedOperator() : base() { }

    public override IOperation InstrumentedApply() {
      OperationCollection result = new OperationCollection();
      foreach (IOperator op in Operators.CheckedItems.OrderBy(x => x.Index).Select(x => x.Value)) {
        result.Add(ExecutionContext.CreateOperation(op));
      }
      result.Add(base.InstrumentedApply());
      return result;
    }
  }
}
