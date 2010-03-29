#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("InversionMoveAttribute", "Specifies the tabu attributes for an inversion move (2-opt).")]
  [StorableClass]
  public class InversionMoveAttribute : Item {
    [Storable]
    public int Edge1Source { get; private set; }
    [Storable]
    public int Edge1Target { get; private set; }
    [Storable]
    public int Edge2Source { get; private set; }
    [Storable]
    public int Edge2Target { get; private set; }

    [StorableConstructor]
    private InversionMoveAttribute(bool deserializing)
      : base() {
    }

    public InversionMoveAttribute()
      : this(-1, -1, -1, -1) { }

    public InversionMoveAttribute(int edge1Source, int edge1Target, int edge2Source, int edge2Target)
      : base() {
      Edge1Source = edge1Source;
      Edge1Target = edge1Target;
      Edge2Source = edge2Source;
      Edge2Target = edge2Target;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      InversionMoveAttribute clone = (InversionMoveAttribute)base.Clone(cloner);
      clone.Edge1Source = Edge1Source;
      clone.Edge1Target = Edge1Target;
      clone.Edge2Source = Edge2Source;
      clone.Edge2Target = Edge2Target;
      return clone;
    }
  }
}
