#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.DataPreprocessing.Filter;
using HEAL.Attic;

namespace HeuristicLab.DataPreprocessing {
  [Item("Filter", "Represents the filter grid.")]
  [StorableType("A1676153-6E8F-48B5-8FC4-EB0E8060179D")]
  public class FilterContent : PreprocessingContent, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Filter; }
    }
    [Storable]
    public ICheckedItemCollection<IFilter> Filters { get; private set; }

    [Storable]
    public bool IsAndCombination { get; set; }

    public IEnumerable<IFilter> ActiveFilters {
      get { return Filters.Where(f => f.Active && f.ConstraintData != null); }
    }

    public bool[] GetRemainingRows() {
      var remainingRows = new bool[PreprocessingData.Rows];
      if (ActiveFilters.Any()) {
        var filterResults = ActiveFilters.Select(f => f.Check()).ToList();
        var rowFilterResults = new bool[filterResults.Count];
        for (int row = 0; row < remainingRows.Length; row++) {
          for (int i = 0; i < filterResults.Count; i++)
            rowFilterResults[i] = filterResults[i][row];

          remainingRows[row] = IsAndCombination
            ? rowFilterResults.All(x => x)
            : rowFilterResults.Any(x => x);
        }
      } else {
        // if not filters active => all rows are remaining
        for (int i = 0; i < remainingRows.Length; i++)
          remainingRows[i] = true;
      }
      return remainingRows;
    }

    #region Constructor, Cloning & Persistence
    public FilterContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
      Filters = new CheckedItemCollection<IFilter>();
      IsAndCombination = true;
    }

    protected FilterContent(FilterContent original, Cloner cloner)
      : base(original, cloner) {
      Filters = cloner.Clone(original.Filters);
      IsAndCombination = original.IsAndCombination;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FilterContent(this, cloner);
    }

    [StorableConstructor]
    protected FilterContent(StorableConstructorFlag _) : base(_) { }
    #endregion
  }
}
