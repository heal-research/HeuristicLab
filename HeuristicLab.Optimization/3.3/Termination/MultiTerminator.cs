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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("MultiTerminator", "A multi operator, containing termination criteria.")]
  [StorableClass]
  public sealed class MultiTerminator : CheckedMultiOperator<ITerminator>, ITerminator {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.FlagRed; }
    }

    public ILookupParameter<BoolValue> TerminateParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Terminate"]; }
    }

    [StorableConstructor]
    private MultiTerminator(bool deserializing) : base(deserializing) { }
    private MultiTerminator(MultiTerminator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new MultiTerminator(this, cloner); }

    public MultiTerminator()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Terminate", "The parameter which will be set to determine if execution should be terminated or should continue."));
    }

    public override IOperation InstrumentedApply() {
      if (!Operators.CheckedItems.Any()) throw new InvalidOperationException(Name + ": Please add at least one termination criterion.");

      var next = new OperationCollection(base.InstrumentedApply());
      foreach (var item in Operators.CheckedItems)
        next.Add(ExecutionContext.CreateOperation(item.Value));
      return next;
    }

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (var opParam in OperatorParameters) {
        var op = opParam.Value;
        var @checked = Operators.ItemChecked(op);
        if (!@checked) continue;
        var children = GetCollectedValues(opParam);
        foreach (var c in children) {
          if (String.IsNullOrEmpty(c.Key))
            values.Add(opParam.Name, new StringValue(opParam.Value.ToString()));
          else values.Add(opParam.Name + "." + c.Key, c.Value);
        }
      }
    }
  }
}