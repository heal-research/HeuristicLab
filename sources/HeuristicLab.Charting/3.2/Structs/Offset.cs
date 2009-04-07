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
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace HeuristicLab.Charting {
  public struct Offset {
    public static readonly Offset Empty;

    private double myDX;
    public double DX {
      get { return myDX; }
      set { myDX = value; }
    }
    private double myDY;
    public double DY {
      get { return myDY; }
      set { myDY = value; }
    }
    public bool IsEmpty {
      get { return this == Empty; }
    }
    public double Length {
      get { return Math.Sqrt(DX * DX + DY * DY); }
    }

    public Offset(double dx, double dy) {
      myDX = dx;
      myDY = dy;
    }
    public Offset(PointD point) {
      myDX = point.X;
      myDY = point.Y;
    }
    public Offset(SizeD size) {
      myDX = size.Width;
      myDY = size.Height;
    }

    public override bool Equals(object obj) {
      if (obj is Offset) {
        return ((Offset)obj) == this;
      } else {
        return base.Equals(obj);
      }
    }
    public override int GetHashCode() {
      return (int)Math.Pow(DX, DY);
    }
    public override string ToString() {
      return "(" + DX.ToString() + ";" + DY.ToString() + ")";
    }

    public static Offset operator +(Offset offset1, Offset offset2) {
      return new Offset(offset1.DX + offset2.DX, offset1.DY + offset2.DY);
    }
    public static Offset operator -(Offset offset1, Offset offset2) {
      return new Offset(offset1.DX - offset2.DX, offset1.DY - offset2.DY);
    }
    public static bool operator ==(Offset offset1, Offset offset2) {
      return (offset1.DX == offset2.DX) && (offset1.DY == offset2.DY);
    }
    public static bool operator !=(Offset offset1, Offset offset2) {
      return (offset1.DX != offset2.DX) && (offset1.DY != offset2.DY);
    }
    public static explicit operator PointD(Offset offset) {
      return new PointD(offset.DX, offset.DY);
    }
    public static explicit operator SizeD(Offset offset) {
      return new SizeD(offset.DX, offset.DY);
    }

    public static Offset Add(Offset offset1, Offset offset2) {
      return new Offset(offset1.DX + offset2.DX, offset1.DY + offset2.DY);
    }
    public static Offset Subtract(Offset offset1, Offset offset2) {
      return new Offset(offset1.DX - offset2.DX, offset1.DY - offset2.DY);
    }
  }
}
