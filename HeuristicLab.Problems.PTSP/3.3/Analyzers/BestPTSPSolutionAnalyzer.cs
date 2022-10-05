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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  /// <summary>
  /// An operator for analyzing the best solution of probabilistic traveling salesman problems given in path representation.
  /// </summary>
  [Item("BestPTSPSolutionAnalyzer", "An operator for analyzing the best solution of Probabilistic Traveling Salesman Problems given in path representation using city coordinates.")]
  [StorableType("459505F0-379B-4903-A388-8612FD732EE7")]
  public sealed class BestPTSPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {
    public bool EnabledByDefault {
      get { return true; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public IScopeTreeLookupParameter<Permutation> PermutationParameter {
      get { return (IScopeTreeLookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleArray> ProbabilitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Probabilities"]; }
    }
    public ILookupParameter<PathPTSPTour> BestSolutionParameter {
      get { return (ILookupParameter<PathPTSPTour>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ILookupParameter<Permutation> BestKnownSolutionParameter {
      get { return (ILookupParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    private BestPTSPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private BestPTSPSolutionAnalyzer(BestPTSPSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestPTSPSolutionAnalyzer(this, cloner);
    }
    public BestPTSPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Permutation", "The PTSP solutions given in path representation from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the PTSP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<DoubleArray>("Probabilities", "This list describes for each city the probability of appearing in a realized instance."));
      Parameters.Add(new LookupParameter<PathPTSPTour>("BestSolution", "The best PTSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best PTSP solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this PTSP instance."));
      Parameters.Add(new LookupParameter<Permutation>("BestKnownSolution", "The best known solution of this PTSP instance."));
    }

    public override IOperation Apply() {
      var coordinates = CoordinatesParameter.ActualValue;
      var permutations = PermutationParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var probabilities = ProbabilitiesParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var max = MaximizationParameter.ActualValue.Value;
      var bestKnownQuality = BestKnownQualityParameter.ActualValue;

      var i = !max ? qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index
                   : qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
      }

      var tour = BestSolutionParameter.ActualValue;
      if (tour == null) {
        tour = new PathPTSPTour(coordinates, probabilities, (Permutation)permutations[i].Clone(), new DoubleValue(qualities[i].Value));
        BestSolutionParameter.ActualValue = tour;
        results.Add(new Result("Best PTSP Solution", tour));
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
