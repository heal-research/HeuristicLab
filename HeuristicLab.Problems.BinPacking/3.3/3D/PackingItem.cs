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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.BinPacking;
using HeuristicLab.Problems.BinPacking3D.Material;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("PackingItem (3d)", "Represents a cuboidic packing-item for bin-packing problems.")]
  [StorableClass]
  public class PackingItem : PackingShape, IPackingItem {
    #region Properties


    public IValueParameter<PackingShape> TargetBinParameter {
      get { return (IValueParameter<PackingShape>)Parameters["TargetBin"]; }
    }
    public IFixedValueParameter<DoubleValue> WeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Weight"]; }
    }
    public IFixedValueParameter<IntValue> LayerParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Layer"]; }
    }

    public IFixedValueParameter<IntValue> SequenceGroupParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["SequenceGroup"]; }
    }

    public IFixedValueParameter<IntValue> IdParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Id"]; }
    }

    public PackingShape TargetBin {
      get { return TargetBinParameter.Value; }
      set { TargetBinParameter.Value = value; }
    }

    public double Weight {
      get { return WeightParameter.Value.Value; }
      set { WeightParameter.Value.Value = value; }
    }

    public int Layer {
      get { return LayerParameter.Value.Value; }
      set { LayerParameter.Value.Value = value; }
    }

    public int SequenceGroup {
      get { return SequenceGroupParameter.Value.Value; }
      set { SequenceGroupParameter.Value.Value = value; }
    }
    
    public int Id {
      get { return IdParameter.Value.Value; }
      set { IdParameter.Value.Value = value; }
    }

    #region Material    

    public IFixedValueParameter<EnumValue<MaterialType>> MaterialTopParameter {
      get { return (IFixedValueParameter<EnumValue<MaterialType>>)Parameters["MaterialTop"]; }
    }
    public MaterialType MaterialTop {
      get { return MaterialTopParameter.Value.Value; }
      set { MaterialTopParameter.Value.Value = value; }
    }
    #endregion

    public IFixedValueParameter<DoubleValue> SupportedWeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["SupportedWeight"]; }
    }
    public double SupportedWeight {
      get { return SupportedWeightParameter.Value.Value; }
      set { SupportedWeightParameter.Value.Value = value; }
    }

    public double SupportedWeightPerSquareMeter {
      get {
        return SupportedWeight / (Width * Depth);
      }
    }

    public IValueParameter<BoolValue> IsStackableParameter {
      get { return (IValueParameter<BoolValue>)Parameters["IsStackable"]; }
    }

    /// <summary>
    /// Indicates that another item can be stacked on the current one.
    /// </summary>
    public bool IsStackabel {
      get { return IsStackableParameter.Value.Value; }
      set { IsStackableParameter.Value.Value = value; }
    }

    public IValueParameter<BoolValue> RotateEnabledParameter {
      get { return (IValueParameter<BoolValue>)Parameters["RotateEnabled"]; }
    }

    public IValueParameter<BoolValue> RotatedParameter {
      get { return (IValueParameter<BoolValue>)Parameters["Rotated"]; }
    }

    public IValueParameter<BoolValue> TiltEnabledParameter {
      get { return (IValueParameter<BoolValue>)Parameters["TiltEnabled"]; }
    }

    public IValueParameter<BoolValue> TiltedParameter {
      get { return (IValueParameter<BoolValue>)Parameters["Tilted"]; }
    }

    /// <summary>
    /// Enables that the current item can be rotated.
    /// </summary>
    public bool RotateEnabled {
      get { return RotateEnabledParameter.Value.Value; }
      set { RotateEnabledParameter.Value.Value = value; }
    }

    /// <summary>
    /// Indicates that the current item is rotated.
    /// If the item is also tilted it will be tilted first.
    /// </summary>
    public bool Rotated {
      get { return RotatedParameter.Value.Value; }
      set { RotatedParameter.Value.Value = value; }
    }

    /// <summary>
    /// Enables that the current item can be tilted.
    /// </summary>
    public bool TiltEnabled {
      get { return TiltEnabledParameter.Value.Value; }
      set { TiltEnabledParameter.Value.Value = value; }
    }

    /// <summary>
    /// Indicates that the current item is tilted.
    /// Tilted means that the item is tilted sidewards.
    /// If the item is also rotated it will be tilted first.
    /// </summary>
    public bool Tilted {
      get { return TiltedParameter.Value.Value; }
      set { TiltedParameter.Value.Value = value; }
    }

    public IFixedValueParameter<IntValue> LoadSecuringHeightParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["LoadSecuringHeight"]; }
    }

    public IFixedValueParameter<IntValue> LoadSecuringWidthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["LoadSecuringWidth"]; }
    }

    public IFixedValueParameter<IntValue> LoadSecuringDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["LoadSecuringDepth"]; }
    }

    public int LoadSecuringHeight {
      get { return LoadSecuringHeightParameter.Value.Value; }
      set { LoadSecuringHeightParameter.Value.Value = value; }
    }

    public int LoadSecuringWidth {
      get { return LoadSecuringWidthParameter.Value.Value; }
      set { LoadSecuringWidthParameter.Value.Value = value; }
    }

    public int LoadSecuringDepth {
      get { return LoadSecuringDepthParameter.Value.Value; }
      set { LoadSecuringDepthParameter.Value.Value = value; }
    }

    /// <summary>
    /// This property represents the height as needed in the bin packing.
    /// </summary>
    public new int Height {
      get {
        if (!Tilted) {
          return HeightParameter.Value.Value + LoadSecuringHeightParameter.Value.Value;
        } else {
          return WidthParameter.Value.Value + LoadSecuringHeightParameter.Value.Value;
        }
      }
    }

    /// <summary>
    /// This property represents the width as needed in the bin packing.
    /// </summary>
    public new int Width {
      get {
        if (Rotated) {
          return DepthParameter.Value.Value + LoadSecuringWidthParameter.Value.Value;
        } else {
          if (!Tilted) {
            return WidthParameter.Value.Value + LoadSecuringWidthParameter.Value.Value;
          } else {
            return HeightParameter.Value.Value + LoadSecuringWidthParameter.Value.Value;
          }
        }
      }
    }

    /// <summary>
    /// This property represents the depth as needed in the bin packing.
    /// </summary>
    public new int Depth {
      get {
        if (!Rotated) {
          return DepthParameter.Value.Value + LoadSecuringDepthParameter.Value.Value;
        } else {
          if (!Tilted) {
            return WidthParameter.Value.Value + LoadSecuringDepthParameter.Value.Value;
          } else {
            return HeightParameter.Value.Value + LoadSecuringDepthParameter.Value.Value;
          }
        }
      }
    }

    /// <summary>
    /// This property represents the height as it is seen in the view of the bin packing.
    /// </summary>
    public int HeightInView {
      get {
        if (!Tilted) {
          return HeightParameter.Value.Value;
        } else {
          return WidthParameter.Value.Value;
        }
      }
    }

    /// <summary>
    /// This property represents the width as it is seen in the view of the bin packing.
    /// </summary>
    public int WidthInView {
      get {
        if (Rotated) {
          return DepthParameter.Value.Value;
        } else {
          if (!Tilted) {
            return WidthParameter.Value.Value;
          } else {
            return HeightParameter.Value.Value;
          }
        }
      }
    }

    /// <summary>
    /// This property represents the depth as it is seen in the view of the bin packing.
    /// </summary>
    public int DepthInView {
      get {
        if (!Rotated) {
          return DepthParameter.Value.Value;
        } else {
          if (!Tilted) {
            return WidthParameter.Value.Value;
          } else {
            return HeightParameter.Value.Value;
          }
        }
      }
    }

    /// <summary>
    /// This property represents the original height.
    /// </summary>
    public int OriginalHeight {
      get { return HeightParameter.Value.Value; }
      set { HeightParameter.Value.Value = value; }
    }

    /// <summary>
    /// This property represents the original width.
    /// </summary>
    public int OriginalWidth {
      get { return WidthParameter.Value.Value; }
      set { WidthParameter.Value.Value = value; }
    }

    /// <summary>
    /// This property represents the original depth.
    /// </summary>
    public int OriginalDepth {
      get { return DepthParameter.Value.Value; }
      set { DepthParameter.Value.Value = value; }
    }

    public bool SupportsStacking(IPackingItem other) {
      return other.Layer <= this.Layer && SupportedWeight > 0;
    }
        
    public bool SupportWeight(double weigth) {
      return SupportedWeight >= weigth;
    }



    #endregion


    [StorableConstructor]
    protected PackingItem(bool deserializing) : base(deserializing) { }
    protected PackingItem(PackingItem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public PackingItem()
      : base() {
      Parameters.Add(new ValueParameter<PackingShape>("TargetBin"));
      Parameters.Add(new FixedValueParameter<DoubleValue>("Weight"));      
      Parameters.Add(new FixedValueParameter<IntValue>("Layer"));
      Parameters.Add(new FixedValueParameter<IntValue>("SequenceGroup"));
      Parameters.Add(new FixedValueParameter<IntValue>("Id"));


      Parameters.Add(new FixedValueParameter<EnumValue<MaterialType>>("MaterialTop"));

      Parameters.Add(new FixedValueParameter<DoubleValue>("SupportedWeight"));

      Parameters.Add(new FixedValueParameter<BoolValue>("RotateEnabled"));
      Parameters.Add(new FixedValueParameter<BoolValue>("Rotated"));
      Parameters.Add(new FixedValueParameter<BoolValue>("TiltEnabled"));
      Parameters.Add(new FixedValueParameter<BoolValue>("Tilted"));
      Parameters.Add(new FixedValueParameter<BoolValue>("IsStackable"));

      Parameters.Add(new FixedValueParameter<IntValue>("LoadSecuringHeight"));
      Parameters.Add(new FixedValueParameter<IntValue>("LoadSecuringWidth"));
      Parameters.Add(new FixedValueParameter<IntValue>("LoadSecuringDepth"));

      IsStackabel = true;

      RegisterEvents();
    }

    public PackingItem(int width, int height, int depth, PackingShape targetBin)
      : this() {
      this.OriginalWidth = width;
      this.OriginalHeight = height;
      this.OriginalDepth = depth;
      this.TargetBin = (PackingShape)targetBin.Clone();
    }

    public PackingItem(int width, int height, int depth, PackingShape targetBin, double weight, int sequenceNumber, int layer)
      : this(width, height, depth, targetBin) {
      this.Weight = weight;
      this.Layer = layer;
      this.SequenceGroup = sequenceNumber;
    }

    public PackingItem(PackingItem packingItem) : this() {
      OriginalWidth = packingItem.OriginalWidth;
      OriginalHeight = packingItem.OriginalHeight;
      OriginalDepth = packingItem.OriginalDepth;
      TargetBin = (PackingShape)packingItem.TargetBin.Clone();
      Weight = packingItem.Weight;
      Layer = packingItem.Layer;
      SequenceGroup = packingItem.SequenceGroup;
      Id = packingItem.Id;
      Rotated = packingItem.Rotated;
      Tilted = packingItem.Tilted;
      IsStackabel = packingItem.IsStackabel;
      MaterialTop = packingItem.MaterialTop;
      MaterialBottom = packingItem.MaterialBottom;

      LoadSecuringDepth = packingItem.LoadSecuringDepth;
      LoadSecuringHeight = packingItem.LoadSecuringHeight;
      LoadSecuringWidth = packingItem.LoadSecuringWidth;
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
      LayerParameter.Value.ValueChanged += (sender, args) => OnToStringChanged();

      // target bin does not occur in ToString()
    }

    public override string ToString() {
      return string.Format("CuboidPackingItem ({0}, {1}, {2}; weight={3}, layer={4})", this.Width, this.Height, this.Depth, this.Weight, this.Layer);
    }

  }
}
