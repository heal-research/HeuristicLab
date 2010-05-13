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

using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An analyzer which applies arbitrary many other analyzers.
  /// </summary>
  [Item("MultiAnalyzer", "An analyzer which applies arbitrary many other analyzers.")]
  [StorableClass]
  public class MultiAnalyzer : CheckedMultiOperator<IAnalyzer>, IAnalyzer {
    public override bool CanChangeName {
      get { return false; }
    }

    public ValueLookupParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["UpdateInterval"]; }
    }
    public LookupParameter<IntValue> UpdateCounterParameter {
      get { return (LookupParameter<IntValue>)Parameters["UpdateCounter"]; }
    }

    public IntValue UpdateInterval {
      get { return UpdateIntervalParameter.Value; }
      set { UpdateIntervalParameter.Value = value; }
    }

    public MultiAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("UpdateInterval", "The interval in which the contained analyzers should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>("UpdateCounter", "The value which counts how many times the MultiAnalyzer was called since the last update."));
    }
    [StorableConstructor]
    protected MultiAnalyzer(bool deserializing) : base(deserializing) { }

    public override IOperation Apply() {
      IntValue interval = UpdateIntervalParameter.ActualValue;
      if (interval == null) interval = new IntValue(1);

      IntValue counter = UpdateCounterParameter.ActualValue;
      if (counter == null) {
        counter = new IntValue(interval.Value);
        UpdateCounterParameter.ActualValue = counter;
      } else counter.Value++;

      if (counter.Value == interval.Value) {
        counter.Value = 0;
        OperationCollection next = new OperationCollection();
        foreach (IndexedItem<IAnalyzer> item in Operators.CheckedItems)
          next.Add(ExecutionContext.CreateOperation(item.Value));
        next.Add(base.Apply());
        return next;
      } else {
        return base.Apply();
      }
    }
  }
}
