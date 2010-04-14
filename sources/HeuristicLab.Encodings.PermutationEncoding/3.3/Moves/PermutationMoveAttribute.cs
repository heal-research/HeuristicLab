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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("PermutationMoveAttribute", "Base class for specifying a move attribute")]
  [StorableClass]
  public class PermutationMoveAttribute : Item {
    [Storable]
    public double MoveQuality { get; protected set; }

    public PermutationMoveAttribute()
      : base() {
      MoveQuality = 0;
    }

    public PermutationMoveAttribute(double moveQuality)
      : base() {
      MoveQuality = moveQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      PermutationMoveAttribute clone = (PermutationMoveAttribute)base.Clone(cloner);
      clone.MoveQuality = MoveQuality;
      return clone;
    }
  }
}
