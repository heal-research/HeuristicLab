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
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  [Item("CounterOperator", "An operator which increments integer values.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class CounterOperator : StandardOperatorBase {
    public Parameter<IntData> Value {
      get { return (Parameter<IntData>)Parameters["Value"]; }
    }
    public Parameter<IntData> Increment {
      get { return (Parameter<IntData>)Parameters["Increment"]; }
    }

    public CounterOperator()
      : base() {
      Parameters.Add(new Parameter<IntData>("Value", "..."));
      Parameters.Add(new Parameter<IntData>("Increment", "..."));
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      IntData value = Value.GetValue(context);
      IntData increment = Increment.GetValue(context);
      value.Value += increment.Value;
      if (value.Value < 10000)
        return base.Apply(context);
      else
        return new ExecutionContextCollection();
    }
  }
}
