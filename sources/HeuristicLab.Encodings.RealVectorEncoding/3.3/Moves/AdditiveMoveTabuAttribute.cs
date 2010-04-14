﻿#region License Information
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
  [Item("AdditiveMoveTabuAttribute", "Tabu attribute for additive moves.")]
  [StorableClass]
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
    private AdditiveMoveTabuAttribute(bool deserializing)
      : base() {
    }
    public AdditiveMoveTabuAttribute()
      : base() {
      Dimension = -1;
      OriginalPosition = 0;
      MovedPosition = 0;
      MoveQuality = 0;
    }

    public AdditiveMoveTabuAttribute(int dimension, double originalPosition, double movedPosition, double moveQuality)
      : base() {
      Dimension = dimension;
      OriginalPosition = originalPosition;
      MovedPosition = movedPosition;
      MoveQuality = moveQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      AdditiveMoveTabuAttribute clone = (AdditiveMoveTabuAttribute)base.Clone(cloner);
      clone.Dimension = Dimension;
      clone.OriginalPosition = OriginalPosition;
      clone.MovedPosition = MovedPosition;
      clone.MoveQuality = MoveQuality;
      return clone;
    }
  }
}
