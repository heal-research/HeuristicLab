#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
namespace HeuristicLab.Collections {
  [StorableType("5be307b0-41d7-4d8d-a289-fba09fce33f4")]
  public interface IObservableKeyedCollection<TKey, TItem> : IObservableCollection<TItem>, INotifyObservableKeyedCollectionItemsChanged<TKey, TItem> {
    TItem this[TKey key] { get; }

    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, out TItem item);

    bool Remove(TKey key);
  }
}
