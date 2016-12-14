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
using HeuristicLab.Collections;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  public class ShiftMove : Move {
    public int PreviousItemOfOldGroup { get; private set; }
    public int PreviousItemOfNewGroup { get; private set; }
    public int NextItemOfOldGroup { get; private set; }
    public int NextItemOfNewGroup { get; private set; }

    private bool IsFirst { get { return PreviousItemOfOldGroup == Item; } }
    private bool IsLast { get { return NextItemOfOldGroup == Item; } }
    private bool NewGroupClosed { get { return PreviousItemOfNewGroup == NextItemOfNewGroup; } }

    public ShiftMove(int item, int prevOld, int prevNew, int nextOld, int nextNew) {
      Item = item;
      PreviousItemOfOldGroup = prevOld;
      PreviousItemOfNewGroup = prevNew;
      NextItemOfOldGroup = nextOld;
      NextItemOfNewGroup = nextNew;
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
