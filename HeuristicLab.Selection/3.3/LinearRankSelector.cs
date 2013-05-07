#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A linear rank selection operator which considers the rank based on a single double quality value for selection.
  /// </summary>
  [Item("LinearRankSelector", "A linear rank selection operator which considers the rank based on a single double quality value for selection.")]
  [StorableClass]
  public sealed class LinearRankSelector : StochasticSingleObjectiveSelector, ISingleObjectiveSelector {
    [StorableConstructor]
    private LinearRankSelector(bool deserializing) : base(deserializing) { }
    private LinearRankSelector(LinearRankSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearRankSelector(this, cloner);
    }
    public LinearRankSelector() : base() { }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      IScope[] selected = new IScope[count];

      // create a list for each scope that contains the scope's index in the original scope list and its lots
      var temp = qualities.Where(x => IsValidQuality(x.Value)).Select((x, index) => new { index, x.Value });
      if (maximization)
        temp = temp.OrderBy(x => x.Value);
      else
        temp = temp.OrderByDescending(x => x.Value);
      var list = temp.Select((x, index) => new { x.index, lots = index + 1 }).ToList();

      //check if list with indexes is as long as the original scope list
      //otherwise invalid quality values were filtered
      if (list.Count != scopes.Count) {
        throw new ArgumentException("The scopes contain invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      int lotSum = list.Count * (list.Count + 1) / 2;
      for (int i = 0; i < count; i++) {
        int selectedLot = random.Next(lotSum) + 1;
        int j = 0;
        int currentLot = list[j].lots;
        while (currentLot < selectedLot) {
          j++;
          currentLot += list[j].lots;
        }
        if (copy)
          selected[i] = (IScope)scopes[list[j].index].Clone();
        else {
          selected[i] = scopes[list[j].index];
          scopes[list[j].index] = null;
          lotSum -= list[j].lots;
          list.RemoveAt(j);
        }
      }
      if (!copy) scopes.RemoveAll(x => x == null);
      return selected;
    }
  }
}
