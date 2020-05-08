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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.Orienteering {
  public interface IOrienteeringSolution : ITSPSolution {
    new IOrienteeringProblemData Data { get; }
    new IntegerVector Tour { get; }
    DoubleValue Quality { get; }
    DoubleValue Score { get; }
    DoubleValue TravelCosts { get; }
  }

  [Item("OrienteeringSolution", "Represents a Orienteering solution which can be visualized in the GUI.")]
  [StorableType("BC58ED08-B9A7-40F3-B8E0-A6B33AA993F4")]
  public sealed class OrienteeringSolution : Item, IOrienteeringSolution {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable] private Permutation routeAsPermutation;
    [Storable] private IntegerVector route;
    public IntegerVector Tour {
      get { return route; }
      set {
        if (route == value) return;
        route = value;
        routeAsPermutation = new Permutation(PermutationTypes.RelativeDirected, value);
        OnPropertyChanged(nameof(Tour));
      }
    }
    [Storable] private IOrienteeringProblemData data;
    public IOrienteeringProblemData Data {
      get { return data; }
      set {
        if (data == value) return;
        data = value;
        OnPropertyChanged(nameof(Data));
      }
    }
    [Storable]
    private DoubleValue quality;
    public DoubleValue Quality {
      get { return quality; }
      set {
        if (quality == value) return;
        quality = value;
        OnPropertyChanged(nameof(Quality));
      }
    }
    [Storable]
    private DoubleValue score;
    public DoubleValue Score {
      get { return score; }
      set {
        if (score == value) return;
        score = value;
        OnPropertyChanged(nameof(Score));
      }
    }
    [Storable]
    private DoubleValue travelCosts;
    public DoubleValue TravelCosts {
      get { return travelCosts; }
      set {
        if (travelCosts == value) return;
        travelCosts = value;
        OnPropertyChanged(nameof(TravelCosts));
        OnPropertyChanged(nameof(ITSPSolution.TourLength));
      }
    }

    ITSPData ITSPSolution.Data => Data;
    Permutation ITSPSolution.Tour => routeAsPermutation;
    DoubleValue ITSPSolution.TourLength => TravelCosts;

    [StorableConstructor]
    private OrienteeringSolution(StorableConstructorFlag _) : base(_) { }
    private OrienteeringSolution(OrienteeringSolution original, Cloner cloner)
      : base(original, cloner) {
      this.route = cloner.Clone(original.route);
      this.routeAsPermutation = cloner.Clone(original.routeAsPermutation);
      this.data = cloner.Clone(original.data);
      this.quality = cloner.Clone(original.quality);
      this.score = cloner.Clone(original.score);
    }
    public OrienteeringSolution(IntegerVector route, IOrienteeringProblemData opData, double quality, double score, double travelCosts)
      : this(route, opData, new DoubleValue(quality), new DoubleValue(score), new DoubleValue(travelCosts)) { }
    public OrienteeringSolution(IntegerVector route, IOrienteeringProblemData opData, DoubleValue quality = null, DoubleValue score = null, DoubleValue distance = null)
      : base() {
      this.route = route;
      this.routeAsPermutation = new Permutation(PermutationTypes.RelativeDirected, route);
      this.data = opData;
      this.quality = quality;
      this.score = score;
      this.travelCosts = distance;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringSolution(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}