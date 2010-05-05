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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator for analyzing the best solution of Traveling Salesman Problems given in path representation using city coordinates.
  /// </summary>
  [Item("MultiPopulationBestTSPSolutionAnalyzer", "An operator for analyzing the best solution of Traveling Salesman Problems given in path representation using city coordinates.")]
  [StorableClass]
  public sealed class MultiPopulationBestTSPSolutionAnalyzer : SingleSuccessorOperator, IBestTSPSolutionAnalyzer, IAnalyzer {
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<ItemArray<ItemArray<Permutation>>> PermutationParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<Permutation>>>)Parameters["Permutation"]; }
    }
    ILookupParameter IBestTSPSolutionAnalyzer.PermutationParameter {
      get { return PermutationParameter; }
    }
    public ILookupParameter<ItemArray<ItemArray<DoubleValue>>> QualityParameter {
      get { return (ILookupParameter<ItemArray<ItemArray<DoubleValue>>>)Parameters["Quality"]; }
    }
    ILookupParameter IBestTSPSolutionAnalyzer.QualityParameter {
      get { return QualityParameter; }
    }
    public ILookupParameter<PathTSPTour> BestSolutionParameter {
      get { return (ILookupParameter<PathTSPTour>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public MultiPopulationBestTSPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Permutation", "The TSP solutions given in path representation from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the TSP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<PathTSPTour>("BestSolution", "The best TSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best TSP solution should be stored."));
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      ItemArray<ItemArray<Permutation>> permutations = PermutationParameter.ActualValue;
      ItemArray<ItemArray<DoubleValue>> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      DoubleValue bestQuality = new DoubleValue(double.MaxValue);
      Permutation bestPermutation = null;

      for (int i = 0; i < qualities.Length; i++) {
        for (int j = 0; j < qualities[i].Length; j++) {
          if (qualities[i][j].Value < bestQuality.Value) {
            bestQuality = qualities[i][j];
            bestPermutation = permutations[i][j];
          }
        }
      }

      PathTSPTour tour = BestSolutionParameter.ActualValue;
      if (tour == null) {
        tour = new PathTSPTour(coordinates, bestPermutation, bestQuality);
        BestSolutionParameter.ActualValue = tour;
        results.Add(new Result("Best TSP Solution", tour));
      } else {
        if (tour.Quality.Value > bestQuality.Value) {
          tour.Coordinates = coordinates;
          tour.Permutation = bestPermutation;
          tour.Quality = bestQuality;
          results["Best TSP Solution"].Value = tour;
        }
      }

      return base.Apply();
    }
  }
}
