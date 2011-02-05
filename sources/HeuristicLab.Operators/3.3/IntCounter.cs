#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which increments an integer variable.
  /// </summary>
  [Item("IntCounter", "An operator which increments an integer variable.")]
  [StorableClass]
  public sealed class IntCounter : SingleSuccessorOperator {

    public LookupParameter<IntValue> ValueParameter {
      get { return (LookupParameter<IntValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<IntValue> IncrementParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Increment"]; }
    }
    public IntValue Increment {
      get { return IncrementParameter.Value; }
      set { IncrementParameter.Value = value; }
    }

    [StorableConstructor]
    private IntCounter(bool deserializing) : base(deserializing) { }
    private IntCounter(IntCounter original, Cloner cloner)
      : base(original, cloner) {
    }
    public IntCounter()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("Value", "The value which should be incremented."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Increment", "The increment which is added to the value.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntCounter(this, cloner);
    }

    public override IOperation Apply() {
      if (ValueParameter.ActualValue == null) ValueParameter.ActualValue = new IntValue();
      ValueParameter.ActualValue.Value += IncrementParameter.ActualValue.Value;
      return base.Apply();
    }
  }
}
