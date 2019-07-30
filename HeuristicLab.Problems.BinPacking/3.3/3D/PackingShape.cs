#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("PackingShape (3d)", "Represents the cuboid measures (width, height, depth) of a three-dimensional cuboidic bin-packing object.")]
  [StorableType("87C5DC4E-A7E3-4853-B6C7-690B3F47DB57")]
  public class PackingShape : PackingShape<PackingPosition>, IComparable<PackingShape> {
    public IFixedValueParameter<IntValue> HeightParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Height"]; }
    }
    public IFixedValueParameter<IntValue> WidthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Width"]; }
    }
    public IFixedValueParameter<IntValue> DepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Depth"]; }
    }

    public int Height {
      get { return HeightParameter.Value.Value; }
      set { HeightParameter.Value.Value = value; }
    }

    public int Width {
      get { return WidthParameter.Value.Value; }
      set { WidthParameter.Value.Value = value; }
    }

    public int Depth {
      get { return DepthParameter.Value.Value; }
      set { DepthParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected PackingShape(StorableConstructorFlag _) : base(_) { }
    protected PackingShape(PackingShape original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public PackingShape()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("Width"));
      Parameters.Add(new FixedValueParameter<IntValue>("Height"));
      Parameters.Add(new FixedValueParameter<IntValue>("Depth"));

      RegisterEvents();
    }

    public PackingShape(int width, int height, int depth)
      : this() {
      this.Width = width;
      this.Height = height;
      this.Depth = depth;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingShape(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      // only because of ToString override
      HeightParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();
      WidthParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();
      DepthParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();
    }

    public override string ToString() {
      return String.Format("CuboidPackingShape ({0}, {1}, {2})", this.Width, this.Height, this.Depth);
    }

    #region IComparable Members

    public int CompareTo(PackingShape other) {
      //Using "Clustered-Area-Height"-comparison as descr

      int result = (this.Width * this.Depth).CompareTo(other.Width * other.Depth);

      if (result == 0)
        result = this.Volume.CompareTo(other.Volume);
      if (result == 0)
        result = this.Height.CompareTo(other.Height);
      return result;
    }

    public override int CompareTo(object obj) {
      var other = (PackingShape)obj;
      if (other != null) return CompareTo(other);
      else throw new ArgumentException(string.Format("Cannot compare with object {0}", obj), "obj");
    }
    #endregion

    [StorableType(StorableMemberSelection.AllFields, "6dc0b0e2-e165-44e0-a342-71974f0494e3")]
    private struct CuboidDiagonal {
      public int x1;
      public int y1;
      public int z1;
      public int x2;
      public int y2;
      public int z2;
      public CuboidDiagonal(PackingShape myShape) : this(new PackingPosition(0, 0, 0, 0), myShape) { }
      public CuboidDiagonal(PackingPosition myPosition, PackingShape myShape) {
        x1 = myPosition.X;
        y1 = myPosition.Y;
        z1 = myPosition.Z;
        x2 = myPosition.X + (myPosition.Rotated ? myShape.Depth : myShape.Width) - 1;
        y2 = myPosition.Y + myShape.Height - 1;
        z2 = myPosition.Z + (myPosition.Rotated ? myShape.Width : myShape.Depth) - 1;
      }
    }


    #region Helpers
    public override PackingPosition Origin { get { return new PackingPosition(0, 0, 0, 0); } }
    public override int Volume { get { return Width * Height * Depth; } }

    public override bool EnclosesPoint(PackingPosition myPosition, PackingPosition checkedPoint) {
      return (myPosition.X <= checkedPoint.X &&
                (myPosition.X + (myPosition.Rotated ? Depth : Width) - 1) >= checkedPoint.X &&
                myPosition.Y <= checkedPoint.Y &&
                (myPosition.Y + Height - 1) >= checkedPoint.Y &&
                myPosition.Z <= checkedPoint.Z &&
                (myPosition.Z + (myPosition.Rotated ? Width : Depth) - 1) >= checkedPoint.Z);
    }
    public override bool Encloses(PackingPosition checkedPosition, PackingShape<PackingPosition> checkedShape) {
      return Encloses(checkedPosition, (PackingShape)checkedShape);
    }
    private bool Encloses(PackingPosition checkedPosition, PackingShape checkedShape) {
      return Encloses(new CuboidDiagonal(this), new CuboidDiagonal(checkedPosition, checkedShape));
    }
    private bool Encloses(CuboidDiagonal c1, CuboidDiagonal c2) {
      return (c1.x1 <= c2.x1 &&
                c1.x2 >= c2.x2 &&
                c1.y1 <= c2.y1 &&
                c1.y2 >= c2.y2 &&
                c1.z1 <= c2.z1 &&
                c1.z2 >= c2.z2);
    }

    public override bool Overlaps(PackingPosition myPosition, PackingPosition checkedPosition, PackingShape<PackingPosition> checkedShape) {
      return Overlaps(myPosition, checkedPosition, (PackingShape)checkedShape);
    }
    private bool Overlaps(PackingPosition myPosition, PackingPosition checkedPosition, PackingShape checkedShape) {
      return Overlaps(new CuboidDiagonal(myPosition, this), new CuboidDiagonal(checkedPosition, checkedShape));
    }
    private bool Overlaps(CuboidDiagonal c1, CuboidDiagonal c2) {
      return !(c1.x1 > c2.x2 ||
               c1.y1 > c2.y2 ||
               c1.z1 > c2.z2 ||
               c1.x2 < c2.x1 ||
               c1.y2 < c2.y1 ||
               c1.z2 < c2.z1);
    }

    public override void ApplyHorizontalOrientation() {
      if (Width > Depth) {
        var aux = Width;
        Width = Depth;
        Depth = aux;
      }
    }
    #endregion

  }
}
