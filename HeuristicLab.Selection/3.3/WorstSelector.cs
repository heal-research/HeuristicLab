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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A selection operator which considers a single double quality value and selects the worst.
  /// </summary>
  [Item("WorstSelector", "A selection operator which considers a single double quality value and selects the worst.")]
  [StorableClass]
  public sealed class WorstSelector : SingleObjectiveSelector, ISingleObjectiveSelector {
    [StorableConstructor]
    private WorstSelector(bool deserializing) : base(deserializing) { }
    private WorstSelector(WorstSelector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new WorstSelector(this, cloner);
    }
    public WorstSelector() : base() { }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      IScope[] selected = new IScope[count];

      // create a list for each scope that contains the scope's index in the original scope list
      var temp = qualities.Where(x => IsValidQuality(x.Value)).Select((x, index) => new { index, x.Value });
      if (maximization)
        temp = temp.OrderBy(x => x.Value);
      else
        temp = temp.OrderByDescending(x => x.Value);
      var list = temp.ToList();

      //check if list with indexes is as long as the original scope list
      //otherwise invalid quality values were filtered
      if (list.Count != scopes.Count) {
        throw new ArgumentException("The scopes contain invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      if (copy) {
        int j = 0;
        for (int i = 0; i < count; i++) {
          selected[i] = (IScope)scopes[list[j].index].Clone();
          j++;
          if (j >= list.Count) j = 0;
        }
      } else {
        for (int i = 0; i < count; i++) {
          selected[i] = scopes[list[0].index];
          scopes[list[0].index] = null;
          list.RemoveAt(0);
        }
        scopes.RemoveAll(x => x == null);
      }
      return selected;
    }
  }
}
