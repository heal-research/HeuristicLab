#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Operator which increments an integer variable.
  /// </summary>
  [Item("Counter", "An operator which increments an integer variable.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class Counter : SingleSuccessorOperator {
    public LookupParameter<IntData> ValueParameter {
      get { return (LookupParameter<IntData>)Parameters["Value"]; }
    }
    public ValueLookupParameter<IntData> IncrementParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["Increment"]; }
    }
    public IntData Increment {
      get { return IncrementParameter.Value; }
      set { IncrementParameter.Value = value; }
    }

    public Counter()
      : base() {
      Parameters.Add(new LookupParameter<IntData>("Value", "The value which should be incremented."));
      Parameters.Add(new ValueLookupParameter<IntData>("Increment", "The increment which is added to the value.", new IntData(1)));
    }

    public override IExecutionContext Apply() {
      if (ValueParameter.ActualValue == null) ValueParameter.ActualValue = new IntData();
      ValueParameter.ActualValue.Value += IncrementParameter.ActualValue.Value;
      return base.Apply();
    }
  }
}
