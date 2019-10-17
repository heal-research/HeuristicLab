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

using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.PTSP {
  [StorableType("dd2d0ecc-372e-46f1-846f-fb4ca2afa124")]
  public interface IProbabilisticTSPData : INamedItem {
    ITSPData TSPData { get; }

    double GetProbability(int city);
    IProbabilisticTSPSolution GetSolution(Permutation tspTour, double tourLength);
    PTSPData Export();
  }

  [Item("Probabilistic TSP Data", "An extension to the TSP where customers have to be served with a certain probability only.")]
  [StorableType("b79f0ad6-b7f7-49c5-9a62-8089db6af1aa")]
  public class ProbabilisticTSPData : NamedItem, IProbabilisticTSPData {
    [Storable] public ITSPData TSPData { get; protected set; }
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected ProbabilisticTSPData(StorableConstructorFlag _) : base(_) { }
    protected ProbabilisticTSPData(ProbabilisticTSPData original, Cloner cloner) : base(original, cloner) {
      TSPData = original.TSPData;
      Probabilities = original.Probabilities;
    }
    public ProbabilisticTSPData() {
      TSPData = new EuclideanTSPData();
      Name = TSPData.Name;
      Probabilities = new PercentArray(Enumerable.Repeat(0.5, TSPData.Cities).ToArray(), @readonly: true);
    }
    public ProbabilisticTSPData(ITSPData tspData, double[] probabilities)
      : base(tspData.Name, tspData.Description) {
      if (tspData.Cities != probabilities.Length) throw new ArgumentException("Unequal number of cities and probabilities.");
      TSPData = tspData;
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public ProbabilisticTSPData(ITSPData tspData, PercentArray probabilities)
      : base(tspData.Name, tspData.Description) {
      if (tspData.Cities != probabilities.Length) throw new ArgumentException("Unequal number of cities and probabilities.");
      TSPData = tspData;
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProbabilisticTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public IProbabilisticTSPSolution GetSolution(Permutation tspTour, double tourLength) {
      var tspSolution = TSPData.GetSolution(tspTour, tourLength);
      return new ProbabilisticTSPSolution(tspSolution, Probabilities);
    }

    public PTSPData Export() {
      var tspExport = TSPData.Export();
      return new PTSPData() {
        Name = tspExport.Name,
        Description = tspExport.Description,
        Coordinates = tspExport.Coordinates,
        Dimension = tspExport.Dimension,
        DistanceMeasure = tspExport.DistanceMeasure,
        Distances = tspExport.Distances,
        Probabilities = Probabilities.CloneAsArray()
      };
    }
  }
}
