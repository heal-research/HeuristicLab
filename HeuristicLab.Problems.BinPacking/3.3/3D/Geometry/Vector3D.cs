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

    public Vector3D() { }
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

    public bool IsInside(PackingPosition pos, Tuple<int, int, int> rs) {
      return X >= pos.X && X < pos.X + rs.Item1
        && Y >= pos.Y && Y < pos.Y + rs.Item2
        && Z >= pos.Z && Z < pos.Z + rs.Item3;
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
