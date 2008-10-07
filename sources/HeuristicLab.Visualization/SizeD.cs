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
  public struct SizeD {
    public static readonly SizeD Empty = new SizeD();

    private double myWidth;

    public double Width {
      get { return myWidth; }
      set { myWidth = value; }
    }

    private double myHeight;

    public double Height {
      get { return myHeight; }
      set { myHeight = value; }
    }

    public bool IsEmpty {
      get { return this == Empty; }
    }

    public SizeD(double width, double height) {
      myWidth = width;
      myHeight = height;
    }

    public SizeD(PointD point) {
      myWidth = point.X;
      myHeight = point.Y;
    }

    public SizeD(Offset offset) {
      myWidth = offset.DX;
      myHeight = offset.DY;
    }

    public override bool Equals(object obj) {
      if (obj is SizeD) {
        return ((SizeD)obj) == this;
      } else {
        return base.Equals(obj);
      }
    }

    public override int GetHashCode() {
      return (int)Math.Pow(Width, Height);
    }

    public override string ToString() {
      return string.Format("({0};{1})", Width, Height);
    }

    public static SizeD operator +(SizeD size1, SizeD size2) {
      return new SizeD(size1.Width + size2.Width, size1.Height + size2.Height);
    }

    public static SizeD operator -(SizeD size1, SizeD size2) {
      return new SizeD(size1.Width - size2.Width, size1.Height - size2.Height);
    }

    public static bool operator ==(SizeD size1, SizeD size2) {
      return (size1.Width == size2.Width) && (size1.Height == size2.Height);
    }

    public static bool operator !=(SizeD size1, SizeD size2) {
      return (size1.Width != size2.Width) && (size1.Height != size2.Height);
    }

    public static explicit operator PointD(SizeD size) {
      return new PointD(size.Width, size.Height);
    }

    public static explicit operator Offset(SizeD size) {
      return new Offset(size.Width, size.Height);
    }

    public static implicit operator SizeD(Size size) {
      return new SizeD(size.Width, size.Height);
    }

    public static implicit operator SizeD(SizeF size) {
      return new SizeD(size.Width, size.Height);
    }

    public static SizeD Add(SizeD size1, SizeD size2) {
      return new SizeD(size1.Width + size2.Width, size1.Height + size2.Height);
    }

    public static SizeD Subtract(SizeD size1, SizeD size2) {
      return new SizeD(size1.Width - size2.Width, size1.Height - size2.Height);
    }
  }
}