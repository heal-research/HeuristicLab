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

using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A linear rank selection operator which considers the rank based on a single double quality value for selection.
  /// </summary>
  [Item("LinearRankSelector", "A linear rank selection operator which considers the rank based on a single double quality value for selection.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class LinearRankSelector : StochasticSingleObjectiveSelector {
    public LinearRankSelector()
      : base() {
      CopySelected.Value = true;
    }

    protected override ScopeList Select(ScopeList scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      ItemArray<DoubleData> qualities = QualityParameter.ActualValue;
      ScopeList selected = new ScopeList();

      // create a list for each scope that contains the scope's index in the original scope list and its lots
      var temp = qualities.Select((x, index) => new { index, x.Value });
      if (maximization)
        temp = temp.OrderBy(x => x.Value);
      else
        temp = temp.OrderByDescending(x => x.Value);
      var list = temp.Select((x, lots) => new { x.index, lots }).ToList();

      int lotSum = list.Count * (list.Count + 1) / 2;
      for (int i = 0; i < count; i++) {
        int selectedLot = random.Next(lotSum) + 1;
        int index = 0;
        int currentLot = list[index].lots;
        while (currentLot < selectedLot) {
          index++;
          currentLot += list[index].lots;
        }
        if (copy)
          selected.Add((IScope)scopes[list[index].index].Clone());
        else {
          selected.Add(scopes[list[index].index]);
          scopes.RemoveAt(list[index].index);
          lotSum -= list[index].lots;
          list.RemoveAt(index);
        }
      }
      return selected;
    }
  }
}
