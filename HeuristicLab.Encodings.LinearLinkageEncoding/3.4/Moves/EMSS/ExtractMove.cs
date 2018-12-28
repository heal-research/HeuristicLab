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
  [Item("Extract Move", "Extracts an item into a group of its own.")]
  [StorableType("D42CB819-F307-4081-9227-7F6786E6853B")]
  public sealed class ExtractMove : EMSSMove {
    [Storable]
    private int previousItem;
    public int PreviousItem { get { return previousItem; } }
    [Storable]
    private int nextItem;
    public int NextItem { get { return nextItem; } }

    private bool IsFirst { get { return PreviousItem == Item; } }
    private bool IsLast { get { return NextItem == Item; } }


    [StorableConstructor]
    private ExtractMove(StorableConstructorFlag _) : base(_) { }
    private ExtractMove(ExtractMove original, Cloner cloner)
      : base(original, cloner) {
      previousItem = original.previousItem;
      nextItem = original.nextItem;
    }
    public ExtractMove(int item, int prev, int next)
      : base(item) {
      previousItem = prev;
      nextItem = next;
    }
    
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtractMove(this, cloner);
    }

    public override void Apply(LinearLinkage lle) {
      if ((!IsFirst && lle[PreviousItem] != Item)
        || lle[Item] != NextItem)
        throw new ArgumentException("Move conditions have changed!");
      if (!IsFirst) lle[PreviousItem] = IsLast ? PreviousItem : NextItem;
      lle[Item] = Item;
    }

    public override void Undo(LinearLinkage lle) {
      if (!IsFirst && lle[PreviousItem] != (IsLast ? PreviousItem : NextItem)
        || lle[Item] != Item)
        throw new ArgumentException("Move conditions have changed, cannot undo move.");

      if (!IsFirst) lle[PreviousItem] = Item;
      lle[Item] = NextItem;
    }

    public override void ApplyToLLEb(int[] lleb) {
      if (!IsLast) lleb[NextItem] = IsFirst ? NextItem : PreviousItem;
      lleb[Item] = Item;
    }
  }
}
