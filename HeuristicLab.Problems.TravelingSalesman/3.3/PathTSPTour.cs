#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.
  /// </summary>
  [Item("PathTSPTour", "Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class PathTSPTour : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private DoubleMatrix coordinates;
    public DoubleMatrix Coordinates {
      get { return coordinates; }
      set {
        if (coordinates != value) {
          if (coordinates != null) DeregisterCoordinatesEvents();
          coordinates = value;
          if (coordinates != null) RegisterCoordinatesEvents();
          OnCoordinatesChanged();
        }
      }
    }
    [Storable]
    private Permutation permutation;
    public Permutation Permutation {
      get { return permutation; }
      set {
        if (permutation != value) {
          if (permutation != null) DeregisterPermutationEvents();
          permutation = value;
          if (permutation != null) RegisterPermutationEvents();
          OnPermutationChanged();
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
    private PathTSPTour(bool deserializing) : base(deserializing) { }
    private PathTSPTour(PathTSPTour original, Cloner cloner)
      : base(original, cloner) {
      this.coordinates = cloner.Clone(original.coordinates);
      this.permutation = cloner.Clone(original.permutation);
      this.quality = cloner.Clone(original.quality);
      Initialize();
    }
    public PathTSPTour() : base() { }
    public PathTSPTour(DoubleMatrix coordinates)
      : base() {
      this.coordinates = coordinates;
      Initialize();
    }
    public PathTSPTour(DoubleMatrix coordinates, Permutation permutation)
      : base() {
      this.coordinates = coordinates;
      this.permutation = permutation;
      Initialize();
    }
    public PathTSPTour(DoubleMatrix coordinates, Permutation permutation, DoubleValue quality)
      : base() {
      this.coordinates = coordinates;
      this.permutation = permutation;
      this.quality = quality;
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PathTSPTour(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      if (coordinates != null) RegisterCoordinatesEvents();
      if (permutation != null) RegisterPermutationEvents();
      if (quality != null) RegisterQualityEvents();
    }

    #region Events
    public event EventHandler CoordinatesChanged;
    private void OnCoordinatesChanged() {
      var changed = CoordinatesChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler PermutationChanged;
    private void OnPermutationChanged() {
      var changed = PermutationChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterCoordinatesEvents() {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
    }
    private void DeregisterCoordinatesEvents() {
      Coordinates.ItemChanged -= new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset -= new EventHandler(Coordinates_Reset);
    }
    private void RegisterPermutationEvents() {
      Permutation.ItemChanged += new EventHandler<EventArgs<int>>(Permutation_ItemChanged);
      Permutation.Reset += new EventHandler(Permutation_Reset);
    }
    private void DeregisterPermutationEvents() {
      Permutation.ItemChanged -= new EventHandler<EventArgs<int>>(Permutation_ItemChanged);
      Permutation.Reset -= new EventHandler(Permutation_Reset);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }

    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      OnCoordinatesChanged();
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      OnCoordinatesChanged();
    }
    private void Permutation_ItemChanged(object sender, EventArgs<int> e) {
      OnPermutationChanged();
    }
    private void Permutation_Reset(object sender, EventArgs e) {
      OnPermutationChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    #endregion
  }
}
