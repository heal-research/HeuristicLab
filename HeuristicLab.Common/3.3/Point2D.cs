#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Globalization;

namespace HeuristicLab.Common {
  [Serializable]
  public struct Point2D<T> where T : struct, IEquatable<T> {
    public static readonly Point2D<T> Empty = new Point2D<T>();

    private T x;
    public T X {
      get { return x; }
    }
    private T y;
    public T Y {
      get { return y; }
    }

    private object tag;
    public object Tag {
      get { return tag; }
    }

    [Browsable(false)]
    public bool IsEmpty {
      get { return Equals(Empty); }
    }

    public Point2D(T x, T y, object tag = null) {
      this.x = x;
      this.y = y;
      this.tag = tag;
    }

    public static Point2D<T> Create(T x, T y, object tag = null) {
      return new Point2D<T>(x, y, tag);
    }

    public static bool operator ==(Point2D<T> left, Point2D<T> right) {
      return left.x.Equals(right.x) && left.y.Equals(right.y) && left.tag == right.tag;
    }
    public static bool operator !=(Point2D<T> left, Point2D<T> right) {
      return !(left == right);
    }

    public override bool Equals(object obj) {
      if (!(obj is Point2D<T>))
        return false;
      Point2D<T> point = (Point2D<T>)obj;
      return GetType() == point.GetType() && x.Equals(point.x) && y.Equals(point.y) &&
             ((tag != null && tag.Equals(point.tag)) || tag == point.tag);
    }
    public override int GetHashCode() {
      return base.GetHashCode();
    }

    public override string ToString() {
      return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[2] { x, y });
    }
  }
}
