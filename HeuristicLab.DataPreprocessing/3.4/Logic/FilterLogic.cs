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
using HeuristicLab.DataPreprocessing.Filter;

namespace HeuristicLab.DataPreprocessing {
  public class FilterLogic {

    public IFilteredPreprocessingData PreprocessingData { get; private set; }

    public bool IsFiltered {
      get {
        return PreprocessingData.IsFiltered;
      }
    }

    public FilterLogic(IFilteredPreprocessingData preprocessingData) {
      PreprocessingData = preprocessingData;
    }

    public bool[] GetFilterResult(IList<IFilter> filters, bool isAndCombination) {
      IList<IFilter> activeFilters = filters.Where(f => f.Active && f.ConstraintData != null).ToList<IFilter>();

      if (activeFilters.Count == 0) {
        return CreateBoolArray(PreprocessingData.Rows, false);
      }
      return GetActiveFilterResult(activeFilters, isAndCombination);
    }

    private bool[] GetActiveFilterResult(IList<IFilter> activeFilters, bool isAndCombination) {
      bool[] result = CreateBoolArray(PreprocessingData.Rows, !isAndCombination);

      foreach (IFilter filter in activeFilters) {
        bool[] filterResult = filter.Check();
        for (int row = 0; row < result.Length; ++row) {
          result[row] = Result(result[row], filterResult[row], isAndCombination);
        }
      }
      return result;
    }

    public bool[] Preview(IList<IFilter> filters, bool isAndCombination) {
      IList<IFilter> activeFilters = filters.Where(f => f.Active && f.ConstraintData != null).ToList<IFilter>();
      if (activeFilters.Count > 0) {
        var result = GetActiveFilterResult(activeFilters, isAndCombination);
        PreprocessingData.SetFilter(result);
        return result;
      } else {
        return CreateBoolArray(PreprocessingData.Rows, false);
      }
    }

    public bool[] CreateBoolArray(int rows, bool value) {
      return Enumerable.Repeat<bool>(value, rows).ToArray();
    }

    public bool Result(bool current, bool addition, bool isAndCombination) {
      return isAndCombination ? current || addition : current && addition;
    }

    public void Apply(IList<IFilter> filters, bool isAndCombination) {
      PreprocessingData.PersistFilter();
      Reset();
    }

    public void Reset() {
      PreprocessingData.ResetFilter();
    }

    public event EventHandler FilterChanged {
      add { PreprocessingData.FilterChanged += value; }
      remove { PreprocessingData.FilterChanged -= value; }
    }
  }
}
