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
  public class ExtractMove : Move {
    public int PreviousItem { get; private set; }
    public int NextItem { get; private set; }

    private bool IsFirst { get { return PreviousItem == Item; } }
    private bool IsLast { get { return NextItem == Item; } }

    public ExtractMove(int item, int prev, int next) {
      Item = item;
      PreviousItem = prev;
      NextItem = next;
    }

    public override void Apply(LinearLinkage lle) {
      if ((!IsFirst && lle[PreviousItem] != Item)
        || lle[Item] != NextItem)
        throw new ArgumentException("Move conditions have changed!");
      if (!IsFirst)
        lle[PreviousItem] = IsLast ? PreviousItem : NextItem;
      lle[Item] = Item;
    }

    public override void Undo(LinearLinkage lle) {
      if (!IsFirst && lle[PreviousItem] != (IsLast ? PreviousItem : NextItem)
        || lle[Item] != Item)
        throw new ArgumentException("Move conditions have changed, cannot undo move.");

      if (!IsFirst)
        lle[PreviousItem] = Item;
      lle[Item] = NextItem;
    }

    public override void UpdateLinks(BidirectionalDictionary<int, int> links) {
      if (!IsFirst) {
        links.RemoveBySecond(Item);
        links.SetByFirst(PreviousItem, IsLast ? PreviousItem : NextItem);
      }
    }
  }
}
