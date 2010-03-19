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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneIndexMove", "A move on a binary vector that is specified by 1 index")]
  [StorableClass]
  public class OneIndexMove : Item {
    [Storable]
    public int Index { get; protected set; }
    [Storable]
    public BinaryVector BinaryVector { get; protected set; }

    public OneIndexMove()
      : base() {
      Index = -1;
      BinaryVector = null;
    }

    public OneIndexMove(int index, BinaryVector binaryVector)
      : base() {
      Index = index;
      BinaryVector = binaryVector;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OneIndexMove clone = (OneIndexMove)base.Clone(cloner);
      clone.Index = Index;
      if (BinaryVector != null)
        clone.BinaryVector = (BinaryVector)BinaryVector.Clone(cloner);
      return clone;
    }
  }
}