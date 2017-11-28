using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Geometry {
  public class Edge3D {
    public Edge3D() { }

    public Edge3D(PackingPosition start, PackingPosition end) {
      Start = new Vector3D(start);
      End = new Vector3D(end);
    }

    public Edge3D(Vector3D start, Vector3D end) {
      Start = start;
      End = end;
    }

    public Vector3D Start { get; set; }
    public Vector3D End { get; set; }


    

    /// <summary>
    /// Returns true if a given point lies on the edge.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Returns true if a given point lies on the edge.</returns>
    public bool LiesOn(Vector3D point) {
      if (point == null) {
        return false;
      }
      var x = (Start.X <= point.X && point.X <= End.X) || (Start.X >= point.X && point.X >= End.X);
      var y = (Start.Y <= point.Y && point.Y <= End.Y) || (Start.Y >= point.Y && point.Y >= End.Y);
      var z = (Start.Z <= point.Z && point.Z <= End.Z) || (Start.Z >= point.Z && point.Z >= End.Z);
      return x && y && z;
    }


    /// <summary>
    /// Returns a point where the two edges intersects.
    /// It returns null if they don't intersect.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>Returns a point where the two edges intersects. Null = no intersection.</returns>
    public Vector3D Intersects(Edge3D edge) {
      return Intersects(this, edge);
    }

    /// <summary>
    /// Returns a point where the two edges intersects.
    /// It returns null if they don't intersect.
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    /// <returns>Returns a point where the two edges intersects. Null = no intersection.</returns>
    public static Vector3D Intersects(Edge3D e1, Edge3D e2) {
      Line3D l1 = new Line3D(e1.Start, new Vector3D() {
        X = e1.Start.X - e1.End.X,
        Y = e1.Start.Y - e1.End.Y,
        Z = e1.Start.Z - e1.End.Z
      });
      Line3D l2 = new Line3D(e2.Start, new Vector3D() {
        X = e2.Start.X - e2.End.X,
        Y = e2.Start.Y - e2.End.Y,
        Z = e2.Start.Z - e2.End.Z
      });
      Vector3D point = l1.Intersect(l2);
      if (e1.LiesOn(point) && e2.LiesOn(point)) {
        return point;
      }
      return null;
    }
  }
}
