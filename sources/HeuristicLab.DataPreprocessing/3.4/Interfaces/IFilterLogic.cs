#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.DataPreprocessing.Filter;

namespace HeuristicLab.DataPreprocessing {
  public interface IFilterLogic {
    bool[] GetFilterResult(IList<IFilter> filters, bool isAndCombination);
    /// <summary>
    /// Return an array which indicates whether the corresponding row should be filtered (true) or kept (false)
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="isAndCombination"></param>
    /// <returns></returns>
    bool[] Preview(IList<IFilter> filters, bool isAndCombination);
    void Apply(IList<IFilter> filters, bool isAndCombination);
    IFilteredPreprocessingData PreprocessingData { get; }
    void Reset();
    bool IsFiltered { get; }

    event EventHandler FilterChanged;
  }
}
