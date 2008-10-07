#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;

namespace HeuristicLab.Visualization {
  public struct PointD {
    public static readonly PointD Empty = new PointD();

    private double myX;

    public double X {
      get { return myX; }
      set { myX = value; }
    }

    private double myY;

    public double Y {
      get { return myY; }
      set { myY = value; }
    }

    public bool IsEmpty {
      get { return this == Empty; }
    }

    public PointD(double x, double y) {
      myX = x;
      myY = y;
    }

    public PointD(SizeD size) {
      myX = size.Width;
      myY = size.Height;
    }

    public PointD(Offset offset) {
      myX = offset.DX;
      myY = offset.DY;
    }

    public override bool Equals(object obj) {
      if (obj is PointD) {
        return ((PointD)obj) == this;
      } else {
        return base.Equals(obj);
      }
    }

    public override int GetHashCode() {
      return (int)Math.Pow(X, Y);
    }

    public override string ToString() {
      return string.Format("({0};{1})", X, Y);
    }

    public static PointD operator +(PointD point, Offset offset) {
      return new PointD(point.X + offset.DX, point.Y + offset.DY);
    }

    public static PointD operator -(PointD point, Offset offset) {
      return new PointD(point.X - offset.DX, point.Y - offset.DY);
    }

    public static Offset operator +(PointD point1, PointD point2) {
      return new Offset(point1.X + point2.X, point1.Y + point2.Y);
    }

    public static Offset operator -(PointD point1, PointD point2) {
      return new Offset(point1.X - point2.X, point1.Y - point2.Y);
    }

    public static bool operator ==(PointD point1, PointD point2) {
      return (point1.X == point2.X) && (point1.Y == point2.Y);
    }

    public static bool operator !=(PointD point1, PointD point2) {
      return (point1.X != point2.X) && (point1.Y != point2.Y);
    }

    public static explicit operator SizeD(PointD point) {
      return new SizeD(point.X, point.Y);
    }

    public static explicit operator Offset(PointD point) {
      return new Offset(point.X, point.Y);
    }

    public static implicit operator PointD(Point point) {
      return new PointD(point.X, point.Y);
    }

    public static implicit operator PointD(PointF point) {
      return new PointD(point.X, point.Y);
    }

    public static PointD Add(PointD point, Offset offset) {
      return new PointD(point.X + offset.DX, point.Y + offset.DY);
    }

    public static PointD Subtract(PointD point, Offset offset) {
      return new PointD(point.X - offset.DX, point.Y - offset.DY);
    }
  }
}