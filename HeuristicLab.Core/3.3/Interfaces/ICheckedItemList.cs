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
using HEAL.Attic;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  [StorableType("ba4a82ca-92eb-47a1-95a7-f41f6ef470f4")]
  public interface ICheckedItemList<T> : IItemList<T> where T : class, IItem {
    event CollectionItemsChangedEventHandler<IndexedItem<T>> CheckedItemsChanged;
    IEnumerable<IndexedItem<T>> CheckedItems { get; }
    bool ItemChecked(T item);
    bool ItemChecked(int itemIndex);
    void SetItemCheckedState(T item, bool checkedState);
    void SetItemCheckedState(IEnumerable<T> items, bool checkedState);
    void SetItemCheckedState(int itemIndex, bool checkedState);
    void SetItemCheckedState(IEnumerable<int> itemIndices, bool checkedState);
    void Add(T item, bool checkedState);
    void Insert(int index, T item, bool checkedState);
  }
}
