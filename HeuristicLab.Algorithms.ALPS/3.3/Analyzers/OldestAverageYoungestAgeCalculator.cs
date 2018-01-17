#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("OldestAverageYoungestAgeCalculator", "An operator which calculates the oldest, average and youngest age of solutions in the scope tree.")]
  [StorableClass]
  public sealed class OldestAverageYoungestAgeCalculator : SingleSuccessorOperator {
    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public IValueLookupParameter<DoubleValue> OldestAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["OldestAge"]; }
    }
    public IValueLookupParameter<DoubleValue> AverageAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AverageAge"]; }
    }
    public IValueLookupParameter<DoubleValue> YoungestAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["YoungestAge"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    private OldestAverageYoungestAgeCalculator(bool deserializing) : base(deserializing) { }
    private OldestAverageYoungestAgeCalculator(OldestAverageYoungestAgeCalculator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OldestAverageYoungestAgeCalculator(this, cloner);
    }
    #endregion

    public OldestAverageYoungestAgeCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The value contained in the scope tree which represents the solution age."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("OldestAge", "The age value of the oldest solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageAge", "The average age of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("YoungestAge", "The age value of the youngest solution."));

      OldestAgeParameter.Hidden = true;
      AverageAgeParameter.Hidden = true;
      YoungestAgeParameter.Hidden = true;
    }

    public override IOperation Apply() {
      var ages = AgeParameter.ActualValue;

      if (ages.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0;
        for (int i = 0; i < ages.Length; i++) {
          if (ages[i].Value < min) min = ages[i].Value;
          if (ages[i].Value > max) max = ages[i].Value;
          sum += ages[i].Value;
        }

        var oldest = OldestAgeParameter.ActualValue;
        if (oldest == null) OldestAgeParameter.ActualValue = new DoubleValue(max);
        else oldest.Value = max;
        var average = AverageAgeParameter.ActualValue;
        if (average == null) AverageAgeParameter.ActualValue = new DoubleValue(sum / ages.Length);
        else average.Value = sum / ages.Length;
        var youngest = YoungestAgeParameter.ActualValue;
        if (youngest == null) YoungestAgeParameter.ActualValue = new DoubleValue(min);
        else youngest.Value = min;
      }
      return base.Apply();
    }
  }
}