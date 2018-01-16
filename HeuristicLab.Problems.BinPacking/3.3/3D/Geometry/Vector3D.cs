﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Geometry {
  public class Vector3D {

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Vector3D() {
      X = 0;
      Y = 0;
      Z = 0;
    }
    public Vector3D(int x, int y, int z) {
      X = x;
      Y = y;
      Z = z;
    }
    public Vector3D(PackingPosition pos) {
      X = pos.X;
      Y = pos.Y;
      Z = pos.Z;
    }

    public PackingPosition ToPackingPosition(int assignedBin) {
      return new PackingPosition(assignedBin, X, Y, Z);
    }

    public static Vector3D AlongX(Vector3D pos, PackingItem item) {
      return new Vector3D(
        pos.X + item.Width,
        pos.Y,
        pos.Z
      );
    }
    public static Vector3D AlongY(Vector3D pos, PackingItem item) {
      return new Vector3D(
        pos.X,
        pos.Y + item.Height,
        pos.Z
      );
    }
    public static Vector3D AlongZ(Vector3D pos, PackingItem item) {
      return new Vector3D(
        pos.X,
        pos.Y,
        pos.Z + item.Depth
      );
    }
    public static Vector3D AlongX(PackingPosition pos, PackingItem item) {
      return new Vector3D(
        pos.X + item.Width,
        pos.Y,
        pos.Z
      );
    }
    public static Vector3D AlongY(PackingPosition pos, PackingItem item) {
      return new Vector3D(
        pos.X,
        pos.Y + item.Height,
        pos.Z
      );
    }
    public static Vector3D AlongZ(PackingPosition pos, PackingItem item) {
      return new Vector3D(
        pos.X,
        pos.Y,
        pos.Z + item.Depth
      );
    }

    public Vector3D Cross(Vector3D b) {
      return new Vector3D(
        Y * b.Z - Z * b.Y,
        -X * b.Z + Z * b.X,
        X * b.Y - Y * b.X
      );
    }

    public bool IsInside(PackingPosition pos, ResidualSpace rs) {
      return X >= pos.X && X < pos.X + rs.Width
        && Y >= pos.Y && Y < pos.Y + rs.Height
        && Z >= pos.Z && Z < pos.Z + rs.Depth;
    }

    public bool IsInside(PackingPosition pos, IEnumerable<ResidualSpace> rs) {
      return rs.Any(x => IsInside(pos, x));
    }

    public static int operator *(Vector3D a, Vector3D b) {
      return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
    public static Vector3D operator *(int a, Vector3D b) {
      return new Vector3D(a * b.X, a * b.Y, a * b.Z);
    }

    public static Vector3D operator *(double a, Vector3D b) {
      return new Vector3D((int)(a * b.X), (int)(a * b.Y), (int)(a * b.Z));
    }

    public static Vector3D operator *(Vector3D a, int b) {
      return new Vector3D(a.X * b, a.Y * b, a.Z * b);
    }

    public static Vector3D operator *(Vector3D a, double b) {
      return new Vector3D((int)(b * a.X), (int)(b * a.Y), (int)(b * a.Z));
    }

    public static Vector3D operator +(Vector3D a, Vector3D b) {
      return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    public static Vector3D operator -(Vector3D a, Vector3D b) {
      return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public override bool Equals(object obj) {
      var packPos = obj as PackingPosition;
      if (packPos != null) {
        return X == packPos.X && Y == packPos.Y && Z == packPos.Z;
      }
      var vec = obj as Vector3D;
      if (vec != null) {
        return X == vec.X && Y == vec.Y && Z == vec.Z;
      }
      return false;
    }
  }
}
