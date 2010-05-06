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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator for analyzing the best solution of Traveling Salesman Problems given in path representation using city coordinates.
  /// </summary>
  [Item("BestTSPSolutionAnalyzer", "An operator for analyzing the best solution of Traveling Salesman Problems given in path representation using city coordinates.")]
  [StorableClass]
  public sealed class BestTSPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public LookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ScopeTreeLookupParameter<Permutation> PermutationParameter {
      get { return (ScopeTreeLookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<PathTSPTour> BestSolutionParameter {
      get { return (LookupParameter<PathTSPTour>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public BestTSPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Permutation", "The TSP solutions given in path representation from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the TSP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<PathTSPTour>("BestSolution", "The best TSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best TSP solution should be stored."));
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      ItemArray<Permutation> permutations = PermutationParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      PathTSPTour tour = BestSolutionParameter.ActualValue;
      if (tour == null) {
        tour = new PathTSPTour(coordinates, permutations[i], qualities[i]);
        BestSolutionParameter.ActualValue = tour;
        results.Add(new Result("Best TSP Solution", tour));
      } else {
        if (tour.Quality.Value > qualities[i].Value) {
          tour.Coordinates = coordinates;
          tour.Permutation = permutations[i];
          tour.Quality = qualities[i];
          results["Best TSP Solution"].Value = tour;
        }
      }

      return base.Apply();
    }
  }
}
