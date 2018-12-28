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
using HEAL.Fossil;
using HeuristicLab.Core;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Packing Position (3d)", "Represents a packing-position associated with a three dimensional packing-problem.")]
  [StorableType("68408986-2325-43D3-A53B-DC2F31FB690C")]
  // PackingPosition is immutable (and handled as value type concerning Equals and GetHashCode)
  public class PackingPosition : BinPacking.PackingPosition, IComparable<PackingPosition> {
    [Storable]
    private readonly int x;
    [Storable]
    private readonly int y;
    [Storable]
    private readonly int z;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return z; } }

    [StorableConstructor]
    protected PackingPosition(StorableConstructorFlag _) : base(_) { }
    protected PackingPosition(PackingPosition original, Cloner cloner)
      : base(original, cloner) {
      x = original.X;
      y = original.Y;
      z = original.Z;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingPosition(this, cloner);
    }

    public PackingPosition(int assignedBin, int x, int y, int z, bool rotated = false)
      : base(assignedBin, rotated) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public override string ToString() {
      return string.Format("[AssignedBin: {0}; ({1},{2},{3})]", AssignedBin, X, Y, Z);
    }


    public override bool Equals(object obj) {
      var tdp = obj as PackingPosition;
      if (tdp != null)
        return (tdp.X == this.X && tdp.Y == this.Y && tdp.Z == this.Z);
      else return false;
    }

    public override int GetHashCode() {
      return base.GetHashCode() + 13 * X + 17 * Y + 23 * Z;
    }

    public static PackingPosition MoveLeft(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X - 1, original.Y, original.Z, original.Rotated);
    }
    public static PackingPosition MoveDown(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y - 1, original.Z, original.Rotated);
    }
    public static PackingPosition MoveBack(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y, original.Z - 1, original.Rotated);
    }

    public static PackingPosition MoveRight(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X + 1, original.Y, original.Z, original.Rotated);
    }
    public static PackingPosition MoveUp(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y + 1, original.Z, original.Rotated);
    }
    public static PackingPosition MoveFront(PackingPosition original) {
      return new PackingPosition(original.AssignedBin, original.X, original.Y, original.Z + 1, original.Rotated);
    }

    #region IComparable<PackingPosition> Members

    public int CompareTo(PackingPosition other) {
      int result = Z.CompareTo(other.Z);
      if (result == 0)
        result = X.CompareTo(other.X);
      if (result == 0)
        result = Y.CompareTo(other.Y);

      return result;

    }

    #endregion
  }
}
