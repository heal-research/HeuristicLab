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

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  public class MergeMove : Move {

    public int LastItemOfOtherGroup { get; private set; }

    public MergeMove(int item, int lastOfOther) {
      Item = item;
      LastItemOfOtherGroup = lastOfOther;
    }

    public override void Apply(LinearLinkage lle) {
      if (lle[LastItemOfOtherGroup] != LastItemOfOtherGroup) throw new ArgumentException("Move conditions have changed, group does not terminate at " + LastItemOfOtherGroup);
      lle[LastItemOfOtherGroup] = Item;
    }

    public override void Undo(LinearLinkage lle) {
      if (lle[LastItemOfOtherGroup] != Item) throw new ArgumentException("Move conditions have changed, groups are no longer linked between " + LastItemOfOtherGroup + " and " + Item);
      lle[LastItemOfOtherGroup] = LastItemOfOtherGroup;
    }

    public override void ApplyToLLEb(int[] lleb) {
      lleb[Item] = LastItemOfOtherGroup;
    }
  }
}
