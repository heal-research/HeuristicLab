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

using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("PackingItem (3d)", "Represents a cuboidic packing-item for bin-packing problems.")]
  [StorableType("413E8254-9600-42F9-B057-E57B502881FD")]
  public class PackingItem : PackingShape, IPackingItem {
    public IValueParameter<PackingShape> TargetBinParameter {
      get { return (IValueParameter<PackingShape>)Parameters["TargetBin"]; }
    }
    public IFixedValueParameter<DoubleValue> WeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Weight"]; }
    }
    public IFixedValueParameter<IntValue> MaterialParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Material"]; }
    }

    public PackingShape TargetBin {
      get { return TargetBinParameter.Value; }
      set { TargetBinParameter.Value = value; }
    }

    public double Weight {
      get { return WeightParameter.Value.Value; }
      set { WeightParameter.Value.Value = value; }
    }

    public int Material {
      get { return MaterialParameter.Value.Value; }
      set { MaterialParameter.Value.Value = value; }
    }

    public bool SupportsStacking(IPackingItem other) {
      return ((other.Material < this.Material) || (other.Material.Equals(this.Material) && other.Weight <= this.Weight));
    }

    [StorableConstructor]
    protected PackingItem(StorableConstructorFlag _) : base(_) { }
    protected PackingItem(PackingItem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public PackingItem()
      : base() {
      Parameters.Add(new ValueParameter<PackingShape>("TargetBin"));
      Parameters.Add(new FixedValueParameter<DoubleValue>("Weight"));
      Parameters.Add(new FixedValueParameter<IntValue>("Material"));

      RegisterEvents();
    }

    public PackingItem(int width, int height, int depth, PackingShape targetBin, double weight, int material)
      : this() {
      this.Width = width;
      this.Height = height;
      this.Depth = depth;
      this.Weight = weight;
      this.Material = material;
      this.TargetBin = (PackingShape)targetBin.Clone();
    }

    public PackingItem(int width, int height, int depth, PackingShape targetBin)
      : this() {
      this.Width = width;
      this.Height = height;
      this.Depth = depth;
      this.TargetBin = (PackingShape)targetBin.Clone();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingItem(this, cloner);
    }

    private void RegisterEvents() {
      // NOTE: only because of ToString override
      WeightParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();
      MaterialParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();

      // target bin does not occur in ToString()
    }


    public override string ToString() {
      return string.Format("CuboidPackingItem ({0}, {1}, {2}; weight={3}, mat={4})", this.Width, this.Height, this.Depth, this.Weight, this.Material);
    }
  }
}
