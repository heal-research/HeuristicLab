#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Packing Position (2d)", "Represents a packing-position associated with a two dimensional packing-problem.")]
  [StorableType("7B0A0FD9-2D2C-4E03-BB66-674446A07587")]
  // PackingPosition is an immutable class (and handled as value type concerning Equals and GetHashCode())
  public class PackingPosition : BinPacking.PackingPosition, IComparable<PackingPosition> {
    [Storable]
    private readonly int x;
    public int X { get { return x; } }

    [Storable]
    private readonly int y;
    public int Y { get { return y; } }

    [StorableConstructor]
    protected PackingPosition(StorableConstructorFlag _) : base(_) { }
    protected PackingPosition(PackingPosition original, Cloner cloner)
      : base(original, cloner) {
      x = original.X;
      y = original.Y;
    }

    public PackingPosition(int assignedBin, int x, int y, bool rotated = false)
      : base(assignedBin, rotated) {
      this.x = x;
      this.y = y;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingPosition(this, cloner);
    }

    public override string ToString() {
      return string.Format("[AssignedBin: {0}; ({1},{2})]", AssignedBin, x, y);
    }

    public override bool Equals(object obj) {
      var other = obj as PackingPosition;
      if (other != null)
        return (other.X == this.X && other.Y == this.Y && base.Equals(other));
      else return false;
    }

    public override int GetHashCode() {
      return base.GetHashCode() + 13 * X + 17 * Y;
    }

    public static PackingPosition MoveLeft(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X - 1, original.Y, original.Rotated);
    }
    public static PackingPosition MoveDown(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y - 1, original.Rotated);
    }

    public static PackingPosition MoveRight(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X + 1, original.Y, original.Rotated);
    }
    public static PackingPosition MoveUp(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y + 1, original.Rotated);
    }


    #region IComparable<PackingPosition> Members

    public int CompareTo(PackingPosition other) {
      int result = X.CompareTo(other.X);
      if (result == 0)
        result = Y.CompareTo(other.Y);

      return result;
    }

    #endregion
  }
}
