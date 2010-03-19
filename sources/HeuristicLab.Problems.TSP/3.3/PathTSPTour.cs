#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.
  /// </summary>
  [Item("PathTSPTour", "Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class PathTSPTour : Item {
    private DoubleMatrix coordinates;
    [Storable]
    public DoubleMatrix Coordinates {
      get { return coordinates; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (coordinates != value) {
          if (coordinates != null) DeregisterCoordinatesEvents();
          coordinates = value;
          if (coordinates != null) RegisterCoordinatesEvents();
          OnCoordinatesChanged();
        }
      }
    }
    private Permutation permutation;
    [Storable]
    public Permutation Permutation {
      get { return permutation; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (permutation != value) {
          if (permutation != null) DeregisterPermutationEvents();
          permutation = value;
          if (permutation != null) RegisterCoordinatesEvents();
          OnPermutationChanged();
        }
      }
    }

    private PathTSPTour() : base() { }
    public PathTSPTour(DoubleMatrix coordinates, Permutation permutation)
      : base() {
      Coordinates = coordinates;
      Permutation = permutation;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      PathTSPTour clone = new PathTSPTour();
      cloner.RegisterClonedObject(this, clone);
      clone.Coordinates = (DoubleMatrix)cloner.Clone(coordinates);
      clone.Permutation = (Permutation)cloner.Clone(permutation);
      return clone;
    }

    #region Events
    public event EventHandler CoordinatesChanged;
    private void OnCoordinatesChanged() {
      if (CoordinatesChanged != null)
        CoordinatesChanged(this, EventArgs.Empty);
    }
    public event EventHandler PermutationChanged;
    private void OnPermutationChanged() {
      if (PermutationChanged != null)
        PermutationChanged(this, EventArgs.Empty);
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
    #endregion
  }
}
