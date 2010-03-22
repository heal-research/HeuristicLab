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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMove", "A move on a real vector that that represents an additive change in one dimension.")]
  [StorableClass]
  public class AdditiveMove : Item {
    [Storable]
    public int Dimension { get; protected set; }
    [Storable]
    public double MoveDistance { get; protected set; }
    [Storable]
    public RealVector RealVector { get; protected set; }

    public AdditiveMove()
      : this(-1, 0, null) {
    }

    public AdditiveMove(int dimension, double moveDistance)
      : this(dimension, moveDistance, null) {
    }

    public AdditiveMove(int dimension, double moveDistance, RealVector realVector)
      : base() {
      Dimension = dimension;
      MoveDistance = moveDistance;
      RealVector = realVector;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      AdditiveMove clone = (AdditiveMove)base.Clone(cloner);
      clone.Dimension = Dimension;
      clone.MoveDistance = MoveDistance;
      if (RealVector != null)
        clone.RealVector = (RealVector)RealVector.Clone(cloner);
      return clone;
    }
  }
}
