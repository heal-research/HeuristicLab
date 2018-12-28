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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Swap2Move", "Item that describes a swap-2 move.")]
  [StorableType("AEB203A4-03AD-4F86-B200-B0FA8253BCFF")]
  public class Swap2Move : Item {
    [Storable]
    public int Item1 { get; protected set; }
    [Storable]
    public int Item2 { get; protected set; }
    [Storable]
    public LinearLinkage LLE { get; protected set; }

    [StorableConstructor]
    protected Swap2Move(StorableConstructorFlag _) : base(_) { }
    protected Swap2Move(Swap2Move original, Cloner cloner) : base(original, cloner) { }
    public Swap2Move() : this(-1, -1, null) { }
    public Swap2Move(int item1, int item2) : this(item1, item2, null) { }
    public Swap2Move(int item1, int item2, LinearLinkage lle)
      : base() {
      Item1 = item1;
      Item2 = item2;
      LLE = lle;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2Move(this, cloner);
    }
  }
}
