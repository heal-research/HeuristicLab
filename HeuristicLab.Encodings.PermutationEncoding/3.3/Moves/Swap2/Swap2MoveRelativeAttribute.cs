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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Swap2MoveRelativeAttribute", "Specifies the tabu attributes for a swap-2 move on a relative position permutation.")]
  [StorableType("826E9F84-344E-48F3-ADC8-E1AFF52AADBF")]
  public class Swap2MoveRelativeAttribute : PermutationMoveAttribute {
    [Storable]
    public ItemList<Edge> DeletedEdges { get; private set; }
    [Storable]
    public ItemList<Edge> AddedEdges { get; private set; }

    [StorableConstructor]
    protected Swap2MoveRelativeAttribute(StorableConstructorFlag _) : base(_) { }
    protected Swap2MoveRelativeAttribute(Swap2MoveRelativeAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.DeletedEdges = cloner.Clone(original.DeletedEdges);
      this.AddedEdges = cloner.Clone(original.AddedEdges);
    }
    public Swap2MoveRelativeAttribute(double moveQuality)
      : base(moveQuality) {
      DeletedEdges = new ItemList<Edge>();
      AddedEdges = new ItemList<Edge>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveRelativeAttribute(this, cloner);
    }
  }
}
