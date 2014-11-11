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

using System;
using System.Collections.Generic;

namespace HeuristicLab.DataPreprocessing {
  public interface IManipulationLogic {
    IEnumerable<string> VariableNames { get; }
    ITransactionalPreprocessingData PreProcessingData { get; }
    void ReOrderToIndices(IEnumerable<int> indices);
    void ReOrderToIndices(IList<Tuple<int, int>> indices);
    void ShuffleToIndices(IList<System.Tuple<int, int>> indices);
    void ReplaceIndicesByAverageValue(IDictionary<int, IList<int>> cells, bool considerSelection = false);
    void ReplaceIndicesByMedianValue(IDictionary<int, IList<int>> cells, bool considerSelection = false);
    void ReplaceIndicesByMostCommonValue(IDictionary<int, IList<int>> cells, bool considerSelection = false);
    void ReplaceIndicesByRandomValue(IDictionary<int, IList<int>> cells, bool considerSelection = false);
    void ReplaceIndicesByLinearInterpolationOfNeighbours(IDictionary<int, IList<int>> cells);
    void ReplaceIndicesBySmoothing(IDictionary<int, IList<int>> cells);
    void ReplaceIndicesByValue(IDictionary<int, IList<int>> cells, string value);
    void ReplaceIndicesByValue<T>(int columnIndex, IEnumerable<int> rowIndices, T value);
    void Shuffle(bool shuffleRangesSeparately);
    List<int> RowsWithMissingValuesGreater(double percent);
    List<int> ColumnsWithMissingValuesGreater(double percent);
    List<int> ColumnsWithVarianceSmaller(double variance);
    void DeleteRowsWithMissingValuesGreater(double percent);
    void DeleteColumnsWithMissingValuesGreater(double percent);
    void DeleteColumnsWithVarianceSmaller(double variance);
  }
}
