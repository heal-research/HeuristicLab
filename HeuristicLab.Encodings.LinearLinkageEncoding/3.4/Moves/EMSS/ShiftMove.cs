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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Shift Move", "Shifts an item from one group to another.")]
  [StorableType("577C4FAF-A02C-4896-8EEB-54360989A4B3")]
  public sealed class ShiftMove : EMSSMove {
    [Storable]
    private int previousItemOfOldGroup;
    public int PreviousItemOfOldGroup { get { return previousItemOfOldGroup; } }
    [Storable]
    private int previousItemOfNewGroup;
    public int PreviousItemOfNewGroup { get { return previousItemOfNewGroup; } }
    [Storable]
    private int nextItemOfOldGroup;
    public int NextItemOfOldGroup { get { return nextItemOfOldGroup; } }
    [Storable]
    private int nextItemOfNewGroup;
    public int NextItemOfNewGroup { get { return nextItemOfNewGroup; } }

    private bool IsFirst { get { return PreviousItemOfOldGroup == Item; } }
    private bool IsLast { get { return NextItemOfOldGroup == Item; } }
    private bool NewGroupClosed { get { return PreviousItemOfNewGroup == NextItemOfNewGroup; } }

    [StorableConstructor]
    private ShiftMove(StorableConstructorFlag _) : base(_) { }
    private ShiftMove(ShiftMove original, Cloner cloner)
      : base(original, cloner) {
      previousItemOfOldGroup = original.previousItemOfOldGroup;
      previousItemOfNewGroup = original.previousItemOfNewGroup;
      nextItemOfOldGroup = original.nextItemOfOldGroup;
      nextItemOfNewGroup = original.nextItemOfNewGroup;
    }
    public ShiftMove(int item, int prevOld, int prevNew, int nextOld, int nextNew)
      : base(item) {
      previousItemOfOldGroup = prevOld;
      previousItemOfNewGroup = prevNew;
      nextItemOfOldGroup = nextOld;
      nextItemOfNewGroup = nextNew;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShiftMove(this, cloner);
    }

    public override void Apply(LinearLinkage lle) {
      if ((!IsFirst && lle[PreviousItemOfOldGroup] != Item)
        || lle[Item] != NextItemOfOldGroup
        || lle[PreviousItemOfNewGroup] != NextItemOfNewGroup)
        throw new ArgumentException("Move conditions have changed!");
      if (!IsFirst)
        lle[PreviousItemOfOldGroup] = IsLast ? PreviousItemOfOldGroup : NextItemOfOldGroup;
      lle[PreviousItemOfNewGroup] = Item;
      lle[Item] = NewGroupClosed ? Item : NextItemOfNewGroup;
    }

    public override void Undo(LinearLinkage lle) {
      if (!IsFirst && lle[PreviousItemOfOldGroup] != (IsLast ? PreviousItemOfOldGroup : NextItemOfOldGroup)
        || lle[PreviousItemOfNewGroup] != Item
        || lle[Item] != (NewGroupClosed ? Item : NextItemOfNewGroup))
        throw new ArgumentException("Move conditions have changed, cannot undo move.");

      if (!IsFirst)
        lle[PreviousItemOfOldGroup] = Item;
      lle[PreviousItemOfNewGroup] = NextItemOfNewGroup;
      lle[Item] = NextItemOfOldGroup;
    }

    public override void ApplyToLLEb(int[] lleb) {
      if (!IsLast)
        lleb[NextItemOfOldGroup] = IsFirst ? NextItemOfOldGroup : PreviousItemOfOldGroup;
      lleb[Item] = PreviousItemOfNewGroup;
      if (!NewGroupClosed)
        lleb[NextItemOfNewGroup] = Item;
    }
  }
}
