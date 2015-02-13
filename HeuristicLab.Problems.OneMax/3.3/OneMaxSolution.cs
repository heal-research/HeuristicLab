#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.OneMax {
  /// <summary>
  /// Represents a OneMax solution.
  /// </summary>
  [Item("OneMaxSolution", "Represents a OneMax solution.")]
  [StorableClass]
  public sealed class OneMaxSolution : Item {
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
    private OneMaxSolution(bool deserializing) : base(deserializing) { }
    private OneMaxSolution(OneMaxSolution original, Cloner cloner)
      : base(original, cloner) {
      binaryVector = cloner.Clone(original.binaryVector);
      quality = cloner.Clone(original.quality);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxSolution(this, cloner);
    }
    public OneMaxSolution() : base() { }
    public OneMaxSolution(BinaryVector binaryVector, DoubleValue quality)
      : base() {
      this.binaryVector = binaryVector;
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
    }

    #region Events
    public event EventHandler BinaryVectorChanged;
    private void OnBinaryVectorChanged() {
      var changed = BinaryVectorChanged;
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
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    #endregion
  }
}
