#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// The generalized rank selection operator selects qualities by rank with a varying focus on better qualities. It is implemented as described in Tate, D. M. and Smith, A. E. 1995. A genetic approach to the quadratic assignment problem. Computers & Operations Research, vol. 22, pp. 73-83.
  /// </summary>
  [Item("GeneralizedRankSelector", "The generalized rank selection operator selects qualities by rank with a varying focus on better qualities. It is implemented as described in Tate, D. M. and Smith, A. E. 1995. A genetic approach to the quadratic assignment problem. Computers & Operations Research, vol. 22, pp. 73-83.")]
  [StorableClass]
  public sealed class GeneralizedRankSelector : StochasticSingleObjectiveSelector, ISelector {

    public IValueLookupParameter<DoubleValue> PressureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Pressure"]; }
    }

    [StorableConstructor]
    private GeneralizedRankSelector(bool deserializing) : base(deserializing) { }
    private GeneralizedRankSelector(GeneralizedRankSelector original, Cloner cloner) : base(original, cloner) { }
    public GeneralizedRankSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Pressure", "The selection pressure that is applied, must lie in the interval [1;infinity). A pressure of 1 equals random selection, higher pressure values focus on selecting better qualities.", new DoubleValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeneralizedRankSelector(this, cloner);
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      IScope[] selected = new IScope[count];
      double pressure = PressureParameter.ActualValue.Value;

      var ordered = qualities.Where(x => IsValidQuality(x.Value)).Select((x, index) => new KeyValuePair<int, double>(index, x.Value)).OrderBy(x => x.Value).ToList();
      if (maximization) ordered.Reverse();

      //check if list with indexes is as long as the original scope list
      //otherwise invalid quality values were filtered
      if (ordered.Count != scopes.Count) {
        throw new ArgumentException("The scopes contain invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      int m = scopes.Count;
      for (int i = 0; i < count; i++) {
        double rand = 1 + random.NextDouble() * (Math.Pow(m, 1.0 / pressure) - 1);
        int selIdx = (int)Math.Floor(Math.Pow(rand, pressure) - 1);

        if (copy) {
          selected[i] = (IScope)scopes[ordered[selIdx].Key].Clone();
        } else {
          int idx = ordered[selIdx].Key;
          selected[i] = scopes[idx];
          scopes.RemoveAt(idx);
          ordered.RemoveAt(selIdx);
          for (int j = 0; j < ordered.Count; j++) {
            var o = ordered[j];
            if (o.Key > idx) ordered[j] = new KeyValuePair<int, double>(o.Key - 1, o.Value);
          }
          m--;
        }
      }
      return selected;
    }
  }
}
