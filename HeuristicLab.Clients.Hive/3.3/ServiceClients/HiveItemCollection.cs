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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  [Item("HiveItem Collection", "Represents a collection of Hive items.")]
  public class HiveItemCollection<T> : ItemCollection<T> where T : class, IHiveItem {
    protected HiveItemCollection(HiveItemCollection<T> original, Cloner cloner) : base(original, cloner) { }
    public HiveItemCollection() : base() { }
    public HiveItemCollection(IEnumerable<T> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) { return new HiveItemCollection<T>(this, cloner); }

    protected override void OnItemsRemoved(IEnumerable<T> items) {
      IEnumerable<T> successful, unsuccessful;
      Exception ex;
      RemoveItems(items, out successful, out unsuccessful, out ex);
      list.AddRange(unsuccessful);
      base.OnItemsRemoved(successful);
      if (ex != null) throw ex;
    }
    protected override void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      IEnumerable<T> successful, unsuccessful;
      Exception ex;
      RemoveItems(oldItems, out successful, out unsuccessful, out ex);
      list.AddRange(unsuccessful);
      base.OnCollectionReset(items.Concat(unsuccessful), oldItems);
      if (ex != null) throw ex;
    }

    public void ClearWithoutHiveDeletion() {
      if (list.Count > 0) {
        T[] items = list.ToArray();
        list.Clear();
        OnPropertyChanged("Count");
        //don't call OnCollectionReset directly as it would delete the job
        base.OnCollectionReset(list, items);
      }
    }

    private void RemoveItems(IEnumerable<T> items, out IEnumerable<T> successful, out IEnumerable<T> unsuccessful, out Exception exception) {
      List<T> removed = new List<T>();
      List<T> notremoved = new List<T>();
      exception = null;
      foreach (T item in items) {
        try {
          HiveClient.Delete(item);
          removed.Add(item);
        }
        catch (Exception ex) {
          exception = ex;
          notremoved.Add(item);
        }
      }
      successful = removed;
      unsuccessful = notremoved;
    }
  }
}