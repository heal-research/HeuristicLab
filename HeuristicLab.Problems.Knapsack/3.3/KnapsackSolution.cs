#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// Represents a knapsack solution which can be visualized in the GUI.
  /// </summary>
  [Item("KnapsackSolution", "Represents a Knapsack solution which can be visualized in the GUI.")]
  [StorableClass]
  public class KnapsackSolution : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private BinaryVector binaryVector;
    public BinaryVector BinaryVector {
      get { return binaryVector; }
      set {
        if (binaryVector != value) {
          if (binaryVector != null) DeregisterBinaryVectorEvents();
          binaryVector = value;
          if (binaryVector != null) RegisterBinaryVectorEvents();
          OnBinaryVectorChanged();
        }
      }
    }

    [Storable]
    private IntValue capacity;
    public IntValue Capacity {
      get { return capacity; }
      set {
        if (capacity != value) {
          if (capacity != null) DeregisterCapacityEvents();
          capacity = value;
          if (capacity != null) RegisterCapacityEvents();
          OnCapacityChanged();
        }
      }
    }

    [Storable]
    private IntArray weights;
    public IntArray Weights {
      get { return weights; }
      set {
        if (weights != value) {
          if (weights != null) DeregisterWeightsEvents();
          weights = value;
          if (weights != null) RegisterWeightsEvents();
          OnWeightsChanged();
        }
      }
    }

    [Storable]
    private IntArray values;
    public IntArray Values {
      get { return values; }
      set {
        if (values != value) {
          if (values != null) DeregisterValuesEvents();
          values = value;
          if (values != null) RegisterValuesEvents();
          OnValuesChanged();
        }
      }
    }

    [Storable]
    private DoubleValue quality;
    public DoubleValue Quality {
      get { return quality; }
      set {
        if (quality != value) {
          if (quality != null) DeregisterQualityEvents();
          quality = value;
          if (quality != null) RegisterQualityEvents();
          OnQualityChanged();
        }
      }
    }

    [StorableConstructor]
    protected KnapsackSolution(bool deserializing) : base(deserializing) { }
    protected KnapsackSolution(KnapsackSolution original, Cloner cloner)
      : base(original, cloner) {
      this.binaryVector = cloner.Clone(original.binaryVector);
      this.quality = cloner.Clone(original.quality);
      this.capacity = cloner.Clone(original.capacity);
      this.weights = cloner.Clone(original.weights);
      this.values = cloner.Clone(original.values);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackSolution(this, cloner);
    }
    public KnapsackSolution() : base() { }
    public KnapsackSolution(BinaryVector binaryVector, DoubleValue quality, IntValue capacity, IntArray weights, IntArray values)
      : base() {
      this.binaryVector = binaryVector;
      this.capacity = capacity;
      this.weights = weights;
      this.values = values;
      this.quality = quality;
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      if (binaryVector != null) RegisterBinaryVectorEvents();
      if (quality != null) RegisterQualityEvents();
      if (capacity != null) RegisterCapacityEvents();
      if (weights != null) RegisterWeightsEvents();
      if (values != null) RegisterValuesEvents();
    }

    #region Events
    public event EventHandler BinaryVectorChanged;
    private void OnBinaryVectorChanged() {
      var changed = BinaryVectorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler CapacityChanged;
    private void OnCapacityChanged() {
      var changed = CapacityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler WeightsChanged;
    private void OnWeightsChanged() {
      var changed = WeightsChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler ValuesChanged;
    private void OnValuesChanged() {
      var changed = ValuesChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterBinaryVectorEvents() {
      BinaryVector.ItemChanged += new EventHandler<EventArgs<int>>(BinaryVector_ItemChanged);
      BinaryVector.Reset += new EventHandler(BinaryVector_Reset);
    }

    private void DeregisterBinaryVectorEvents() {
      BinaryVector.ItemChanged -= new EventHandler<EventArgs<int>>(BinaryVector_ItemChanged);
      BinaryVector.Reset -= new EventHandler(BinaryVector_Reset);
    }

    private void RegisterCapacityEvents() {
      Capacity.ValueChanged += new EventHandler(Capacity_ValueChanged);
    }

    private void DeregisterCapacityEvents() {
      Capacity.ValueChanged -= new EventHandler(Capacity_ValueChanged);
    }

    private void RegisterWeightsEvents() {
      Weights.ItemChanged += new EventHandler<EventArgs<int>>(Weights_ItemChanged);
      Weights.Reset += new EventHandler(Weights_Reset);
    }

    private void DeregisterWeightsEvents() {
      Weights.ItemChanged -= new EventHandler<EventArgs<int>>(Weights_ItemChanged);
      Weights.Reset -= new EventHandler(Weights_Reset);
    }

    private void RegisterValuesEvents() {
      Values.ItemChanged += new EventHandler<EventArgs<int>>(Values_ItemChanged);
      Values.Reset += new EventHandler(Values_Reset);
    }

    private void DeregisterValuesEvents() {
      Values.ItemChanged -= new EventHandler<EventArgs<int>>(Values_ItemChanged);
      Values.Reset -= new EventHandler(Values_Reset);
    }

    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }

    private void BinaryVector_ItemChanged(object sender, EventArgs<int> e) {
      OnBinaryVectorChanged();
    }
    private void BinaryVector_Reset(object sender, EventArgs e) {
      OnBinaryVectorChanged();
    }
    void Capacity_ValueChanged(object sender, EventArgs e) {
      OnCapacityChanged();
    }
    private void Weights_ItemChanged(object sender, EventArgs<int> e) {
      OnWeightsChanged();
    }
    private void Weights_Reset(object sender, EventArgs e) {
      OnWeightsChanged();
    }
    private void Values_ItemChanged(object sender, EventArgs<int> e) {
      OnValuesChanged();
    }
    private void Values_Reset(object sender, EventArgs e) {
      OnValuesChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    #endregion
  }
}
