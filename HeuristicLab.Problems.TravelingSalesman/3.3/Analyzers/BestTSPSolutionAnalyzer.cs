#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class BestTSPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {
    public bool EnabledByDefault {
      get { return true; }
    }

    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public LookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ScopeTreeLookupParameter<Permutation> PermutationParameter {
      get { return (ScopeTreeLookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<PathTSPTour> BestTSPSolutionParameter {
      get { return (LookupParameter<PathTSPTour>)Parameters["BestTSPSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<Permutation> BestKnownSolutionParameter {
      get { return (LookupParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    private BestTSPSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestTSPSolutionAnalyzer(BestTSPSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestTSPSolutionAnalyzer(this, cloner);
    }
    public BestTSPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Permutation", "The TSP solutions given in path representation from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the TSP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<PathTSPTour>("BestTSPSolution", "The best TSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best TSP solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this TSP instance."));
      Parameters.Add(new LookupParameter<Permutation>("BestKnownSolution", "The best known solution of this TSP instance."));

      MaximizationParameter.Hidden = true;
      CoordinatesParameter.Hidden = true;
      PermutationParameter.Hidden = true;
      QualityParameter.Hidden = true;
      BestTSPSolutionParameter.Hidden = true;
      ResultsParameter.Hidden = true;
      BestKnownQualityParameter.Hidden = true;
      BestKnownSolutionParameter.Hidden = true;
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      ItemArray<Permutation> permutations = PermutationParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
      }

      PathTSPTour tour = BestTSPSolutionParameter.ActualValue;
      if (tour == null) {
        tour = new PathTSPTour(coordinates, (Permutation)permutations[i].Clone(), new DoubleValue(qualities[i].Value));
        BestTSPSolutionParameter.ActualValue = tour;
        results.Add(new Result("Best TSP Solution", tour));
      } else {
        if (max && tour.Quality.Value < qualities[i].Value ||
          !max && tour.Quality.Value > qualities[i].Value) {
          tour.Coordinates = coordinates;
          tour.Permutation = (Permutation)permutations[i].Clone();
          tour.Quality.Value = qualities[i].Value;
        }
      }

      return base.Apply();
    }
  }
}
