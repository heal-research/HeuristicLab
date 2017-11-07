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
  }
}
