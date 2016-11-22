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
  /// A tournament selection operator which considers a single double quality value for selection.
  /// </summary>
  [Item("TournamentSelector", "A tournament selection operator which considers a single double quality value for selection.")]
  [StorableClass]
  public sealed class TournamentSelector : StochasticSingleObjectiveSelector, ISingleObjectiveSelector {
    public ValueLookupParameter<IntValue> GroupSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["GroupSize"]; }
    }

    [StorableConstructor]
    private TournamentSelector(bool deserializing) : base(deserializing) { }
    private TournamentSelector(TournamentSelector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TournamentSelector(this, cloner);
    }

    public TournamentSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("GroupSize", "The size of the tournament group.", new IntValue(2)));
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      List<double> qualities = QualityParameter.ActualValue.Where(x => IsValidQuality(x.Value)).Select(x => x.Value).ToList();
      int groupSize = GroupSizeParameter.ActualValue.Value;
      IScope[] selected = new IScope[count];

      //check if list with indexes is as long as the original scope list
      //otherwise invalid quality values were filtered
      if (qualities.Count != scopes.Count) {
        throw new ArgumentException("The scopes contain invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      for (int i = 0; i < count; i++) {
        int best = random.Next(scopes.Count);
        int index;
        for (int j = 1; j < groupSize; j++) {
          index = random.Next(scopes.Count);
          if (((maximization) && (qualities[index] > qualities[best])) ||
              ((!maximization) && (qualities[index] < qualities[best]))) {
            best = index;
          }
        }

        if (copy)
          selected[i] = (IScope)scopes[best].Clone();
        else {
          selected[i] = scopes[best];
          scopes.RemoveAt(best);
          qualities.RemoveAt(best);
        }
      }
      return selected;
    }
  }
}
