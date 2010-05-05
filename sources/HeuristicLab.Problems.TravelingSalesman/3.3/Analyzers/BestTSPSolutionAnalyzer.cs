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
  public sealed class BestTSPSolutionAnalyzer : SingleSuccessorOperator, IBestTSPSolutionAnalyzer, IAnalyzer {
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    ILookupParameter IBestTSPSolutionAnalyzer.PermutationParameter {
      get { return PermutationParameter; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
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

    public BestTSPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be analyzed."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the TSP solution which should be analyzed."));
      Parameters.Add(new LookupParameter<PathTSPTour>("BestSolution", "The best TSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the TSP solution should be stored."));
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      PathTSPTour tour = BestSolutionParameter.ActualValue;
      if (tour == null) {
        tour = new PathTSPTour(coordinates, permutation, quality);
        BestSolutionParameter.ActualValue = tour;
        results.Add(new Result("Best TSP Solution", tour));
      } else {
        if (tour.Quality.Value > quality.Value) {
          tour.Coordinates = coordinates;
          tour.Permutation = permutation;
          tour.Quality = quality;
          results["Best TSP Solution"].Value = tour;
        }
      }

      return base.Apply();
    }
  }
}
