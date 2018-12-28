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


using HEAL.Fossil;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("PackingItem (2d)", "Represents a rectangular packing-item for bin-packing problems.")]
  [StorableType("36B693B0-3CD9-4EE2-97EF-B8BEA8E4877A")]
  public class PackingItem : PackingShape, IPackingItem {

    public PackingShape TargetBin {
      get { return ((IValueParameter<PackingShape>)Parameters["TargetBin"]).Value; }
      set { ((IValueParameter<PackingShape>)Parameters["TargetBin"]).Value = value; }
    }

    public double Weight {
      get { return ((IFixedValueParameter<DoubleValue>)Parameters["Weight"]).Value.Value; }
      set { ((IFixedValueParameter<DoubleValue>)Parameters["Weight"]).Value.Value = value; }
    }

    public int Material {
      get { return ((IFixedValueParameter<IntValue>)Parameters["Material"]).Value.Value; }
      set { ((IFixedValueParameter<IntValue>)Parameters["Material"]).Value.Value = value; }
    }

    [StorableConstructor]
    protected PackingItem(StorableConstructorFlag _) : base(_) { }
    protected PackingItem(PackingItem original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingItem(this, cloner);
    }

    public PackingItem()
      : base() {
      Parameters.Add(new ValueParameter<PackingShape>("TargetBin"));
      Parameters.Add(new FixedValueParameter<DoubleValue>("Weight"));
      Parameters.Add(new FixedValueParameter<IntValue>("Material"));
    }

    public PackingItem(int width, int height, PackingShape targetBin)
      : this() {
      this.Width = width;
      this.Height = height;
      this.TargetBin = (PackingShape)targetBin.Clone();
    }

    public bool SupportsStacking(IPackingItem other) {
      return ((other.Material < this.Material) || (other.Material.Equals(this.Material) && other.Weight <= this.Weight));
    }
  }
}
