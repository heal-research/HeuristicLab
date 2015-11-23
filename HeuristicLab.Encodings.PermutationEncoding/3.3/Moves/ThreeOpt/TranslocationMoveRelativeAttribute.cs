#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("TranslocationMoveRelativeAttribute", "Specifies the tabu attributes for a translocation and insertion move (3-opt) on relative permutation encodings.")]
  [StorableClass]
  public class TranslocationMoveRelativeAttribute : PermutationMoveAttribute {
    [Storable]
    public int Edge1Source { get; private set; }
    [Storable]
    public int Edge1Target { get; private set; }
    [Storable]
    public int Edge2Source { get; private set; }
    [Storable]
    public int Edge2Target { get; private set; }
    [Storable]
    public int Edge3Source { get; private set; }
    [Storable]
    public int Edge3Target { get; private set; }

    [StorableConstructor]
    protected TranslocationMoveRelativeAttribute(bool deserializing) : base(deserializing) { }
    protected TranslocationMoveRelativeAttribute(TranslocationMoveRelativeAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Edge1Source = original.Edge1Source;
      this.Edge1Target = original.Edge1Target;
      this.Edge2Source = original.Edge2Source;
      this.Edge2Target = original.Edge2Target;
      this.Edge3Source = original.Edge3Source;
      this.Edge3Target = original.Edge3Target;
    }
    public TranslocationMoveRelativeAttribute() : this(-1, -1, -1, -1, -1, -1, -1) { }
    public TranslocationMoveRelativeAttribute(int edge1Source, int edge1Target, int edge2Source, int edge2Target, int edge3Source, int edge3Target, double moveQuality)
      : base(moveQuality) {
      Edge1Source = edge1Source;
      Edge1Target = edge1Target;
      Edge2Source = edge2Source;
      Edge2Target = edge2Target;
      Edge3Source = edge3Source;
      Edge3Target = edge3Target;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationMoveRelativeAttribute(this, cloner);
    }
  }
}
