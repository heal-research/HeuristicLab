#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Fossil;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("PackingShape (2d)", "Represents the rectangular measures (width, height) of a two-dimensional bin-packing object.")]
  [StorableType("11EF7917-826C-4154-A7E6-0869C1188B95")]
  public class PackingShape : PackingShape<PackingPosition>, IComparable<PackingShape> {
    public int Height {
      get { return ((IFixedValueParameter<IntValue>)Parameters["Height"]).Value.Value; }
      set { ((IFixedValueParameter<IntValue>)Parameters["Height"]).Value.Value = value; }
    }

    public int Width {
      get { return ((IFixedValueParameter<IntValue>)Parameters["Width"]).Value.Value; }
      set { ((IFixedValueParameter<IntValue>)Parameters["Width"]).Value.Value = value; }
    }

    [StorableConstructor]
    protected PackingShape(StorableConstructorFlag _) : base(_) { }
    protected PackingShape(PackingShape original, Cloner cloner)
      : base(original, cloner) {
    }
    public PackingShape()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("Width"));
      Parameters.Add(new FixedValueParameter<IntValue>("Height"));
    }
    public PackingShape(int width, int height)
      : this() {
      this.Height = height;
      this.Width = width;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingShape(this, cloner);
    }

    public override string ToString() {
      return String.Format("RectangularPackingShape ({0}, {1})", this.Width, this.Height);
    }

    #region IComparable<RectangularPackingShape> Members

    public int CompareTo(PackingShape other) {
      int result = this.Width.CompareTo(other.Width);
      if (result == 0)
        result = this.Height.CompareTo(other.Height);
      return result;
    }

    public override int CompareTo(object obj) {
      var other = obj as PackingShape;
      if (other != null) return CompareTo(other);
      else throw new ArgumentException(string.Format("Cannot compare to object {0}", obj), "obj");
    }

    #endregion


    #region Helpers
    public override PackingPosition Origin { get { return new PackingPosition(0, 0, 0); } }
    public override int Volume { get { return Height * Width; } }

    public override bool EnclosesPoint(PackingPosition myPosition, PackingPosition checkedPoint) {
      return (myPosition.X <= checkedPoint.X &&
                (myPosition.X + (myPosition.Rotated ? Height : Width) - 1) >= checkedPoint.X &&
                myPosition.Y <= checkedPoint.Y &&
                (myPosition.Y + (myPosition.Rotated ? Width : Height) - 1) >= checkedPoint.Y);
    }
    public override bool Encloses(PackingPosition checkedPosition, PackingShape<PackingPosition> checkedShape) {
      return Encloses(checkedPosition, (PackingShape)checkedShape);
    }
    private bool Encloses(PackingPosition checkedPosition, PackingShape checkedShape) {
      return Encloses(new RectangleDiagonal(this), new RectangleDiagonal(checkedPosition, checkedShape));
    }
    private bool Encloses(RectangleDiagonal r1, RectangleDiagonal r2) {
      return (r1.x1 <= r2.x1 &&
                r1.x2 >= r2.x2 &&
                r1.y1 <= r2.y1 &&
                r1.y2 >= r2.y2);
    }

    public override bool Overlaps(PackingPosition myPosition, PackingPosition checkedPosition, PackingShape<PackingPosition> checkedShape) {
      return Overlaps(myPosition, checkedPosition, (PackingShape)checkedShape);
    }
    private bool Overlaps(PackingPosition myPosition, PackingPosition checkedPosition, PackingShape checkedShape) {
      return Overlaps(new RectangleDiagonal(myPosition, this), new RectangleDiagonal(checkedPosition, checkedShape));
    }
    private bool Overlaps(RectangleDiagonal r1, RectangleDiagonal r2) {
      return !(r1.x1 > r2.x2 ||
               r1.y1 > r2.y2 ||
               r1.x2 < r2.x1 ||
               r1.y2 < r2.y1);
    }

    public override void ApplyHorizontalOrientation() {
      if (Width < Height) {
        var aux = Width;
        Width = Height;
        Height = aux;
      }
    }
    #endregion


    private struct RectangleDiagonal {
      public int x1;
      public int y1;
      public int x2;
      public int y2;
      public RectangleDiagonal(PackingShape myShape) : this(new PackingPosition(0, 0, 0), myShape) { }
      public RectangleDiagonal(PackingPosition myPosition, PackingShape myShape) {
        x1 = myPosition.X;
        y1 = myPosition.Y;
        x2 = myPosition.X + (myPosition.Rotated ? myShape.Height : myShape.Width) - 1;
        y2 = myPosition.Y + (myPosition.Rotated ? myShape.Width : myShape.Height) - 1;
      }
    }
  }
}
