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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem which can be visualized in the GUI.
  /// </summary>
  [Item("TSPTour", "Represents a tour of a Traveling Salesman Problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class TSPTour : Item {
    private DoubleMatrix coordinates;
    [Storable]
    public DoubleMatrix Coordinates {
      get { return coordinates; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (coordinates != value) {
          coordinates = value;
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
          permutation = value;
          OnPermutationChanged();
        }
      }
    }

    private TSPTour() : base() { }
    public TSPTour(DoubleMatrix coordinates, Permutation permutation)
      : base() {
      if ((coordinates == null) || (permutation == null)) throw new ArgumentNullException();
      this.coordinates = coordinates;
      this.permutation = permutation;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      TSPTour clone = new TSPTour((DoubleMatrix)cloner.Clone(coordinates), (Permutation)cloner.Clone(permutation));
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

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
  }
}
