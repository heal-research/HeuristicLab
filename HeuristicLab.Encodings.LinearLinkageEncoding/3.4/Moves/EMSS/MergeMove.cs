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
  [Item("Merge Move", "Merges two groups together.")]
  [StorableType("4AC9ADD2-3825-4CFE-8074-6F3CA7251700")]
  public sealed class MergeMove : EMSSMove {
    [Storable]
    private int lastItemOfOtherGroup;
    public int LastItemOfOtherGroup { get { return lastItemOfOtherGroup; } }
    
    [StorableConstructor]
    private MergeMove(StorableConstructorFlag _) : base(_) { }
    private MergeMove(MergeMove original, Cloner cloner)
      : base(original, cloner) {
      lastItemOfOtherGroup = original.lastItemOfOtherGroup;
    }
    public MergeMove(int item, int lastOfOther)
      : base(item) {
      lastItemOfOtherGroup = lastOfOther;
    }
    
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MergeMove(this, cloner);
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
