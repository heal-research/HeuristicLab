#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing {
  public interface ISearchLogic {

    int Rows { get; }
    int Columns { get; }
    IEnumerable<string> VariableNames { get; }

    /// <summary>
    /// Return the indices of the missing values where the key 
    /// correspdonds to the columbn index and the values to the row indices
    /// </summary>
    /// <returns></returns>
    IDictionary<int, IList<int>> GetMissingValueIndices();

    /// <summary>
    /// Return the row indices of the cells which contain a missing value
    /// </summary>
    /// <returns></returns>
    IList<int> GetMissingValueIndices(int columnIndex);

    bool IsMissingValue(int columnIndex, int rowIndex);

    IEnumerable<T> GetValuesWithoutNaN<T>(int columnIndex, bool considerSelection = false);
  }
}
