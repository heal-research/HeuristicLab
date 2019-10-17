#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ComponentModel;
using System.Drawing;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("f08a63d9-0b83-4944-9251-42925baeb872")]
  public interface ITSPSolution : IItem, INotifyPropertyChanged {
    ITSPData TSPData { get; }
    Permutation Tour { get; }
    DoubleValue TourLength { get; }
  }

  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.
  /// </summary>
  [Item("TSP Solution", "Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.")]
  [StorableType("38d1aac3-3047-40d9-bcf9-4b3ca0b9f95c")]
  public sealed class TSPSolution : Item, ITSPSolution {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private ITSPData tspData;
    public ITSPData TSPData {
      get { return tspData; }
      set {
        if (tspData == value) return;
        tspData = value;
        OnPropertyChanged(nameof(TSPData));
      }
    }

    [Storable]
    private Permutation tour;
    public Permutation Tour {
      get { return tour; }
      set {
        if (tour == value) return;
        tour = value;
        OnPropertyChanged(nameof(Tour));
      }
    }
    [Storable]
    private DoubleValue tourLength;
    public DoubleValue TourLength {
      get { return tourLength; }
      set {
        if (tourLength == value) return;
        tourLength = value;
        OnPropertyChanged(nameof(TourLength));
      }
    }

    [StorableConstructor]
    private TSPSolution(StorableConstructorFlag _) : base(_) { }
    private TSPSolution(TSPSolution original, Cloner cloner)
      : base(original, cloner) {
      this.tspData = cloner.Clone(original.tspData);
      this.tour = cloner.Clone(original.tour);
      this.tourLength = cloner.Clone(original.tourLength);
    }
    public TSPSolution() : base() { }
    public TSPSolution(ITSPData data)
      : base() {
      this.tspData = data;
    }
    public TSPSolution(ITSPData data, Permutation permutation)
      : base() {
      this.tspData = data;
      this.tour = permutation;
    }
    public TSPSolution(ITSPData data, Permutation permutation, DoubleValue quality)
      : base() {
      this.tspData = data;
      this.tour = permutation;
      this.tourLength = quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPSolution(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
