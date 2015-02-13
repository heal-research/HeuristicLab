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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which increments a double variable.
  /// </summary>
  [Item("DoubleCounter", "An operator which increments a double variable.")]
  [StorableClass]
  public sealed class DoubleCounter : SingleSuccessorOperator {
    public LookupParameter<DoubleValue> ValueParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<DoubleValue> IncrementParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Increment"]; }
    }
    public DoubleValue Increment {
      get { return IncrementParameter.Value; }
      set { IncrementParameter.Value = value; }
    }

    [StorableConstructor]
    private DoubleCounter(bool deserializing) : base(deserializing) { }
    private DoubleCounter(DoubleCounter original, Cloner cloner)
      : base(original, cloner) {
    }
    public DoubleCounter()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Value", "The value which should be incremented."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Increment", "The increment which is added to the value.", new DoubleValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleCounter(this, cloner);
    }

    public override IOperation Apply() {
      if (ValueParameter.ActualValue == null) ValueParameter.ActualValue = new DoubleValue();
      ValueParameter.ActualValue.Value += IncrementParameter.ActualValue.Value;
      return base.Apply();
    }
  }
}
