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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMoveTabuAttribute", "Tabu attribute for additive moves.")]
  [StorableType("F53207BE-01C0-4057-AACE-8DF031C8EBC5")]
  public class AdditiveMoveTabuAttribute : Item {
    [Storable]
    public int Dimension { get; protected set; }
    [Storable]
    public double OriginalPosition { get; protected set; }
    [Storable]
    public double MovedPosition { get; protected set; }
    [Storable]
    public double MoveQuality { get; protected set; }

    [StorableConstructor]
    protected AdditiveMoveTabuAttribute(StorableConstructorFlag _) : base(_) { }
    protected AdditiveMoveTabuAttribute(AdditiveMoveTabuAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Dimension = original.Dimension;
      this.OriginalPosition = original.OriginalPosition;
      this.MovedPosition = original.MovedPosition;
      this.MoveQuality = original.MoveQuality;
    }
    public AdditiveMoveTabuAttribute() : this(-1, 0, 0, 0) { }
    public AdditiveMoveTabuAttribute(int dimension, double originalPosition, double movedPosition, double moveQuality)
      : base() {
      Dimension = dimension;
      OriginalPosition = originalPosition;
      MovedPosition = movedPosition;
      MoveQuality = moveQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMoveTabuAttribute(this, cloner);
    }
  }
}
