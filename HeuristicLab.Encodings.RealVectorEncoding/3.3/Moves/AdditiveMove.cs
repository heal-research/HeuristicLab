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
  [Item("AdditiveMove", "A move on a real vector that that represents an additive change in one dimension.")]
  [StorableType("D7E22627-2AB5-44A9-BD4D-9D1C5E45A150")]
  public class AdditiveMove : Item {
    [Storable]
    public int Dimension { get; protected set; }
    [Storable]
    public double MoveDistance { get; protected set; }
    [Storable]
    public RealVector RealVector { get; protected set; }

    [StorableConstructor]
    protected AdditiveMove(StorableConstructorFlag _) : base(_) { }
    protected AdditiveMove(AdditiveMove original, Cloner cloner)
      : base(original, cloner) {
      this.Dimension = original.Dimension;
      this.MoveDistance = original.MoveDistance;
      if (original.RealVector != null)
        this.RealVector = cloner.Clone(original.RealVector);
    }
    public AdditiveMove() : this(-1, 0, null) { }
    public AdditiveMove(int dimension, double moveDistance) : this(dimension, moveDistance, null) { }
    public AdditiveMove(int dimension, double moveDistance, RealVector realVector)
      : base() {
      Dimension = dimension;
      MoveDistance = moveDistance;
      RealVector = realVector;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMove(this, cloner);
    }
  }
}
