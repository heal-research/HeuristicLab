using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Geometry {
  public enum Side { Top, Left, Bottom, Right, Front, Back }

  /// <summary>
  /// A plane is given as a point and two directing vectors
  /// </summary>
  public class Plane3D {
    public Vector3D Point { get; private set; }
    public Vector3D Direction1 { get; private set; }
    public Vector3D Direction2 { get; private set; }
    public Vector3D Normal { get; private set; }

    private int rhs;

    public Plane3D(PackingShape bin, Side side) {
      switch (side) {
        case Side.Top: // ZX plane
          Point = new Vector3D(0, bin.Height, 0);
          Direction1 = new Vector3D(0, 0, bin.Depth);
          Direction2 = new Vector3D(bin.Width, 0, 0);
          break;
        case Side.Left: // ZY plane
          Point = new Vector3D(0, 0, 0);
          Direction1 = new Vector3D(0, 0, bin.Depth);
          Direction2 = new Vector3D(0, bin.Height, 0);
          break;
        case Side.Bottom: // XZ plane
          Point = new Vector3D(0, 0, 0);
          Direction1 = new Vector3D(bin.Width, 0, 0);
          Direction2 = new Vector3D(0, 0, bin.Depth);
          break;
        case Side.Right: // YZ plane
          Point = new Vector3D(bin.Width, 0, 0);
          Direction1 = new Vector3D(0, bin.Height, 0);
          Direction2 = new Vector3D(0, 0, bin.Depth);
          break;
        case Side.Front: // XY plane
          Point = new Vector3D(0, 0, bin.Depth);
          Direction1 = new Vector3D(bin.Width, 0, 0);
          Direction2 = new Vector3D(0, bin.Height, 0);
          break;
        case Side.Back: // YX plane
          Point = new Vector3D(0, 0, 0);
          Direction1 = new Vector3D(0, bin.Height, 0);
          Direction2 = new Vector3D(bin.Width, 0, 0);
          break;
      }
      Normal = Direction1.Cross(Direction2);
      rhs = 0;
    }

    public Plane3D(PackingPosition pos, PackingItem item, Side side) {
      // the directing vectors are chosen such that normal always points outside the packing item
      switch (side) {
        case Side.Top: // ZX plane
          Point = new Vector3D(pos.X, pos.Y + item.Height, pos.Z);
          Direction1 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
          Direction2 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
          break;
        case Side.Left: // ZY plane
          Point = new Vector3D(pos.X, pos.Y, pos.Z);
          Direction1 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
          Direction2 = new Vector3D(0, item.Height, 0);
          break;
        case Side.Bottom: // XZ plane
          Point = new Vector3D(pos.X, pos.Y, pos.Z);
          Direction1 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
          Direction2 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
          break;
        case Side.Right: // YZ plane
          Point = new Vector3D(pos.X + (pos.Rotated ? item.Depth : item.Width), pos.Y, pos.Z);
          Direction1 = new Vector3D(0, item.Height, 0);
          Direction2 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
          break;
        case Side.Front: // XY plane
          Point = new Vector3D(pos.X, pos.Y, pos.Z + (pos.Rotated ? item.Width : item.Depth));
          Direction1 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
          Direction2 = new Vector3D(0, item.Height, 0);
          break;
        case Side.Back: // YX plane
          Point = new Vector3D(pos.X, pos.Y, pos.Z);
          Direction1 = new Vector3D(0, item.Height, 0);
          Direction2 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
          break;
      }
      Normal = Direction1.Cross(Direction2);
      rhs = Normal.X * Point.X + Normal.Y * Point.Y + Normal.Z * Point.Z;
    }

    /// <summary>
    /// Returns true if the given point is an element of the current plane
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsElementOf(Vector3D point) {
      return Normal.X * point.X + Normal.Y * point.Y + Normal.Z * point.Z == rhs;
    }

    /// <summary>
    /// Returns true, if the given line intersects with the current plane
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public bool Intersects(Line3D line) {
      return Intersect(line) != null;
    }

    /// <summary>
    /// Returns the point of intersection of a given line and the current plane.
    /// It returns null if there is no intersection.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public Vector3D Intersect(Line3D line) {
      var denom = Normal * line.Direction;
      if (denom == 0) {
        // dir is perpendicular to the normal vector of the plane
        // this means they intersect if p1 is element of the plane
        // also the plane does not stretch infinitely, but is bound
        // to the parallelogram spanned by the point and the two
        // directing vectors
        if (IsElementOf(line.Point) && IsWithinDirectionalVectors(line.Point))
          return line.Point;
        else
          return null;
      }
      var intersect = line.Point + ((Normal * (Point - line.Point)) / denom) * line.Direction;
      if (IsWithinDirectionalVectors(intersect))
        return intersect;
      return null;
    }

    public bool IsWithinDirectionalVectors(Vector3D point) {
      return point.X >= Point.X && (Direction1.X + Direction2.X == 0 || (point.X < Point.X + Direction1.X || point.X < Point.X + Direction2.X))
          && point.Y >= Point.Y && (Direction1.Y + Direction2.Y == 0 || (point.Y < Point.Y + Direction1.Y || point.Y < Point.Y + Direction2.Y))
          && point.Z >= Point.Z && (Direction1.Z + Direction2.Z == 0 || (point.Z < Point.Z + Direction1.Z || point.Z < Point.Z + Direction2.Z));
    }
  }
}
