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
using System.Linq;
using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("A56BFB05-8F11-4766-9FBF-20C7010F1CA3")]
  [Item("ShapeConstraints", "Represents shape constraints associated with a regression problem data e.g. monotonicity constraints.")]
  public class ShapeConstraints : CheckedItemList<ShapeConstraint> {
    public IEnumerable<ShapeConstraint> EnabledConstraints => base.CheckedItems.Select(checkedItem => checkedItem.Value);

    protected override void OnItemsAdded(IEnumerable<IndexedItem<ShapeConstraint>> items) {
      base.OnItemsAdded(items);
      foreach (var item in items)
        item.Value.Changed += Item_Changed;
    }

    protected override void OnItemsRemoved(IEnumerable<IndexedItem<ShapeConstraint>> items) {
      base.OnItemsRemoved(items);
      foreach (var item in items)
        item.Value.Changed -= Item_Changed;
    }

    protected override void OnCollectionReset(
      IEnumerable<IndexedItem<ShapeConstraint>> items,
      IEnumerable<IndexedItem<ShapeConstraint>> oldItems) {
      base.OnCollectionReset(items, oldItems);
      foreach (var item in items)
        item.Value.Changed += Item_Changed;
      foreach (var item in oldItems)
        item.Value.Changed -= Item_Changed;
    }

    private void Item_Changed(object sender, EventArgs e) {
      RaiseChanged();
    }

    [StorableConstructor]
    protected ShapeConstraints(StorableConstructorFlag _) : base(_) { }

    protected ShapeConstraints(ShapeConstraints original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShapeConstraints(this, cloner);
    }

    public ShapeConstraints() : base() {
    }

    public event EventHandler Changed;

    private void RaiseChanged() {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, EventArgs.Empty);
    }
  }
}