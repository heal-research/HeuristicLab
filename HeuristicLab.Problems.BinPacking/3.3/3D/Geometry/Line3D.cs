#region License Information
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
  /// <summary>
  /// A line is given as a point and a directing vector
  /// </summary>
  public class Line3D {
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

    /// <summary>
    /// Returns the intersection point of two lines.
    /// It the lines doesn't intersect it returns null.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public Vector3D Intersect(Line3D line) {
      double r = 0;
      double s = 0;

      // if they have the same source point, this point can be returned.
      if (this.Point.Equals(line.Point)) {
        return this.Point;
      }

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
      var p1 = r * this.Direction + this.Point;
      var p2 = s * line.Direction + line.Point;
      var c = p1.Equals(p2);
      if (s!=0 && r!=0 && p1.Equals(p2)) {        
        return p1;
      }

      return null;
    }
  }
}
