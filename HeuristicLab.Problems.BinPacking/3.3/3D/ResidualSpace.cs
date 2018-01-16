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

using HeuristicLab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D.Geometry;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("ResidualSpace", "Represents a residual space für a 3D bin-packing problem.")]
  [StorableClass]
  public class ResidualSpace : Item {

    public static ResidualSpace Create(int width, int height, int depth) {
      return new ResidualSpace(width, height, depth);
    }    

    public static ResidualSpace Create(ResidualSpace other) {
      return new ResidualSpace() {
        Width = other.Width,
        Height = other.Height,
        Depth = other.Depth
      };
    }

    public ResidualSpace() {
      SetZero();
    }

    public ResidualSpace(int width, int height, int depth) {
      Width = width;
      Height = height;
      Depth = depth;
    }

    public ResidualSpace(Tuple<int, int, int> rs) {
      Width = rs.Item1;
      Height = rs.Item2;
      Depth = rs.Item3;

      if (IsZero()) {
        SetZero();
      }
    }

    public ResidualSpace(BinPacking3D binPacking, Vector3D point) {
      Width = binPacking.BinShape.Width - point.X;
      Height = binPacking.BinShape.Height - point.Y;
      Depth = binPacking.BinShape.Depth - point.Z;

      if (IsZero()) {
        SetZero();
      }
    }

    [StorableConstructor]
    protected ResidualSpace(bool deserializing) : base(deserializing) { }


    protected ResidualSpace(ResidualSpace original, Cloner cloner) : base(original, cloner) {
      this.Width = original.Width;
      this.Height = original.Height;
      this.Depth = original.Depth;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResidualSpace(this, cloner);
    }

    [Storable]
    public int Width { get; set; }

    [Storable]
    public int Height { get; set; }

    [Storable]
    public int Depth { get; set; }
    
    public override bool Equals(object obj) {

      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }
      ResidualSpace rs = obj as ResidualSpace;            
      return this.Width == rs.Width && this.Height == rs.Height && this.Depth == rs.Depth;
    }

    public override string ToString() {
      return $"({Width},{Height},{Depth})";
    }

    /*************************************************************
     * 
     * ***********************************************************/

    public Tuple<int, int, int> ToTuple() {
      return new Tuple<int, int, int>(Width, Height, Depth);
    }

    public PackingItem ToPackingItem() {
      return new PackingItem() {
        OriginalWidth = this.Width,
        OriginalHeight = this.Height,
        OriginalDepth = this.Depth
      };
    }

    public bool IsZero() {
      return Width == 0 || Height == 0 || Depth == 0;
    }
    
    public void SetZero() {
      Width = 0;
      Height = 0;
      Depth = 0;
    }
  }
}
