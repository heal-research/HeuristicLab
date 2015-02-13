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
using System.Linq;

namespace HeuristicLab.DataPreprocessing.Views {
  class FindPreprocessingItemsIterator : IFindPreprocessingItemsIterator {

    private IDictionary<int, IList<int>> items;
    private Tuple<int, int> currentCell;

    public FindPreprocessingItemsIterator(IDictionary<int, IList<int>> items) {
      this.items = items;
      Reset(); 
    }

    public void SetStartCell(int columnIndex, int rowIndex) {
      Tuple<int, int> startCell = GetNextFoundCell(columnIndex, rowIndex);
      if (startCell == null) {
        int tmpColumnIndex = columnIndex;
        do {
          ++tmpColumnIndex;
          if (!items.ContainsKey(tmpColumnIndex)) {
            tmpColumnIndex = 0;
          }
          startCell = GetNextFoundCell(tmpColumnIndex, 0);
        } while (startCell == null && tmpColumnIndex != columnIndex);

        if (startCell == null && tmpColumnIndex == columnIndex && rowIndex != 0) {
          startCell = GetNextFoundCell(columnIndex, 0);
        }
      }
      currentCell = startCell;
    }

    private Tuple<int, int> GetNextFoundCell(int columnIndex, int rowIndex) {
      Tuple<int, int> next = null;
      for (int i = 0; i < items[columnIndex].Count; ++i) {
        if (items[columnIndex][i] >= rowIndex) {
          next = new Tuple<int, int>(columnIndex, i);
          break;
        }
      }
      return next;
    }

    public bool MoveNext() {
      bool result = false;
      bool endReached = false;
      if (CurrentCellExists()) {
        do {
          if (currentCell.Item2 < items[currentCell.Item1].Count - 1) {
            currentCell = new Tuple<int, int>(currentCell.Item1, currentCell.Item2 + 1);
          } else if (currentCell.Item1 < items.Count - 1) {
            currentCell = new Tuple<int, int>(currentCell.Item1 + 1, 0);
          } else {
            endReached = true;
          }
        } while (!endReached && !CurrentCellExists());
        result = !endReached;
      }
      return result;
    }

    public Tuple<int, int> GetCurrent() {
      Tuple<int, int> resultCell = null;
      if (items != null && CurrentCellExists()) {
        int cellRow = items[currentCell.Item1].ElementAt(currentCell.Item2);
        resultCell = new Tuple<int, int>(currentCell.Item1, cellRow);
      }
      return resultCell;
    }

    public void Reset() {
      SetStartCell(0, 0);
      if (!CurrentCellExists()) {
        MoveNext();
      }
    }

    private bool CurrentCellExists() {
      bool result = false;
      if (currentCell != null && items != null) {
        result = items.ContainsKey(currentCell.Item1) && currentCell.Item2 < items[currentCell.Item1].Count;
      }
      return result;
    }
  }
}
