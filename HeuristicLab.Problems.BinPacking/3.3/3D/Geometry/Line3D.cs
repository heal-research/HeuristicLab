using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Geometry {
  /// <summary>
  /// A line is given as a point and a directing vector
  /// </summary>
  internal class Line3D {
    public Vector3D Point;
    public Vector3D Direction;

    public Line3D(Vector3D point, Vector3D direction) {
      Point = point;
      Direction = direction;
    }
    public Line3D(PackingPosition position, Vector3D direction) {
      Point = new Vector3D(position);
      Direction = direction;
    }

    public bool Intersects(Plane3D plane) {
      return plane.Intersects(this);
    }

    public Vector3D Intersect(Plane3D plane) {
      return plane.Intersect(this);
    }

    public Vector3D Intersect(Line3D line) {
      double r = 0;
      double s = 0;

      if (Direction.X != 0) {
        r = (line.Point.X - this.Point.X) / (double)Direction.X;
      } else if (Direction.Y != 0) {
        r = (line.Point.Y - this.Point.Y) / (double)Direction.Y;
      } else if (Direction.Z != 0) {
        r = (line.Point.Z - this.Point.Z) / (double)Direction.Z;
      }

      if (line.Direction.X != 0) {
        s = (this.Point.X - line.Point.X) / (double)line.Direction.X;
      } else if (line.Direction.Y != 0) {
        s = (this.Point.Y - line.Point.Y) / (double)line.Direction.Y;
      } else if (line.Direction.Z != 0) {
        s = (this.Point.Z - line.Point.Z) / (double)line.Direction.Z;
      }

      var a = r * this.Direction + this.Point;
      var b = s * line.Direction + line.Point;
      var c = a.Equals(b);
      if (s!=0 && r!=0 && a.Equals(b)) {
        
        return a;
      }

      return null;
    }
  }
}
