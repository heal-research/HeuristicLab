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
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.PTSP {
  [StorableType("596f52b5-b2c8-45a0-a7bd-e5b9c787c960")]
  public interface IProbabilisticTSPSolution : IItem, INotifyPropertyChanged {
    ITSPSolution TSPSolution { get; }
    DoubleArray Probabilities { get; }
  }

  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.
  /// </summary>
  [Item("pTSP Solution", "Represents a tour of a Probabilistic Traveling Salesman Problem given in path representation which can be visualized in the GUI.")]
  [StorableType("ea784622-41e5-493e-a1f3-4c38fed99216")]
  public sealed class ProbabilisticTSPSolution : Item, IProbabilisticTSPSolution {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private ITSPSolution tspSolution;
    public ITSPSolution TSPSolution {
      get { return tspSolution; }
      set {
        if (tspSolution == value) return;
        tspSolution = value;
        OnPropertyChanged(nameof(TSPSolution));
      }
    }

    [Storable]
    private DoubleArray probabilities;
    public DoubleArray Probabilities {
      get { return probabilities; }
      set {
        if (probabilities == value) return;
        probabilities = value;
        OnPropertyChanged(nameof(Probabilities));
      }
    }

    [StorableConstructor]
    private ProbabilisticTSPSolution(StorableConstructorFlag _) : base(_) { }
    private ProbabilisticTSPSolution(ProbabilisticTSPSolution original, Cloner cloner)
      : base(original, cloner) {
      this.tspSolution = cloner.Clone(original.tspSolution);
      this.probabilities = cloner.Clone(original.probabilities);
    }
    public ProbabilisticTSPSolution() : base() { }
    public ProbabilisticTSPSolution(ITSPSolution tspSolution, PercentArray probabilities)
      : base() {
      this.tspSolution = tspSolution;
      this.probabilities = probabilities;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProbabilisticTSPSolution(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
