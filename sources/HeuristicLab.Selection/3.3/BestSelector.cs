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

using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A selection operator which considers a single double quality value and selects the best.
  /// </summary>
  [Item("BestSelector", "A selection operator which considers a single double quality value and selects the best.")]
  [StorableClass]
  [Creatable("Test")]
  public sealed class BestSelector : SingleObjectiveSelector {
    public BestSelector() : base() { }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      IScope[] selected = new IScope[count];

      // create a list for each scope that contains the scope's index in the original scope list
      var temp = qualities.Select((x, index) => new { index, x.Value });
      if (maximization)
        temp = temp.OrderByDescending(x => x.Value);
      else
        temp = temp.OrderBy(x => x.Value);
      var list = temp.ToList();

      if (copy) {
        for (int i = 0, j = 0; i < count; i++, j++) {
          if (j >= list.Count) j = 0;
          selected[i] = (IScope)scopes[list[j].index].Clone();
        }
      } else {
        int i;
        for (i = 0; i < count; i++) {
          selected[i] = scopes[list[i].index];
        }
        // remove the selected scopes starting from the scope with the highest index
        if (i < list.Count) list.RemoveRange(i, list.Count - i);
        list = list.OrderBy(x => x.index).ToList();
        do {
          i--;
          scopes.RemoveAt(list[i].index);
        } while (i > 0);
      }
      return selected;
    }
  }
}
