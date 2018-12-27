#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Orienteering {
  [StorableClass]
  public sealed class BestOrienteeringSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public bool EnabledByDefault {
      get { return true; }
    }

    public IScopeTreeLookupParameter<IntegerVector> IntegerVector {
      get { return (IScopeTreeLookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<IntValue> StartingPointParameter {
      get { return (ILookupParameter<IntValue>)Parameters["StartingPoint"]; }
    }
    public ILookupParameter<IntValue> TerminalPointParameter {
      get { return (ILookupParameter<IntValue>)Parameters["TerminalPoint"]; }
    }
    public ILookupParameter<DoubleArray> ScoresParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Scores"]; }
    }
    public ILookupParameter<DoubleValue> PointVisitingCostsParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PointVisitingCosts"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> PenaltyParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public ILookupParameter<OrienteeringSolution> BestSolutionParameter {
      get { return (ILookupParameter<OrienteeringSolution>)Parameters["BestSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ILookupParameter<IntegerVector> BestKnownSolutionParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    private BestOrienteeringSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestOrienteeringSolutionAnalyzer(BestOrienteeringSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestOrienteeringSolutionAnalyzer(this, cloner);
    }
    public BestOrienteeringSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("IntegerVector", "The Orienteering solutions which should be analysed."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the points."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the points."));
      Parameters.Add(new LookupParameter<IntValue>("StartingPoint", "Index of the starting point."));
      Parameters.Add(new LookupParameter<IntValue>("TerminalPoint", "Index of the ending point."));
      Parameters.Add(new LookupParameter<DoubleArray>("Scores", "The scores of the points."));
      Parameters.Add(new LookupParameter<DoubleValue>("PointVisitingCosts", "The costs for visiting a point."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the Orienteering solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Penalty", "The applied penalty of the Orienteering solutions."));
      Parameters.Add(new LookupParameter<OrienteeringSolution>("BestSolution", "The best Orienteering solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best Orienteering solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this Orienteering instance."));
      Parameters.Add(new LookupParameter<IntegerVector>("BestKnownSolution", "The best known solution of this Orienteering instance."));
    }

    public override IOperation Apply() {
      var solutions = IntegerVector.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var penalties = PenaltyParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int bestIndex = qualities.Select((quality, index) => new { index, quality.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null || qualities[bestIndex].Value > bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[bestIndex].Value);
        BestKnownSolutionParameter.ActualValue = (IntegerVector)solutions[bestIndex].Clone();
      }

      var solution = BestSolutionParameter.ActualValue;
      var coordinates = CoordinatesParameter.ActualValue;
      var startingPoint = StartingPointParameter.ActualValue;
      var terminalPoint = TerminalPointParameter.ActualValue;
      var scores = ScoresParameter.ActualValue;
      var pointVisitingCosts = PointVisitingCostsParameter.ActualValue;
      var distances = DistanceMatrixParameter.ActualValue;
      double distance = distances.CalculateTourLength(solutions[bestIndex].ToList(), pointVisitingCosts.Value);

      if (solution == null) {
        solution = new OrienteeringSolution(
          (IntegerVector)solutions[bestIndex].Clone(),
          coordinates,
          startingPoint,
          terminalPoint,
          scores,
          new DoubleValue(qualities[bestIndex].Value),
          new DoubleValue(penalties[bestIndex].Value),
          new DoubleValue(distance));
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best Orienteering Solution", solution));
      } else {
        if (solution.Quality.Value < qualities[bestIndex].Value) {
          solution.Coordinates = coordinates;
          solution.Scores = scores;
          solution.IntegerVector = (IntegerVector)solutions[bestIndex].Clone();
          solution.Quality.Value = qualities[bestIndex].Value;
          solution.Penalty.Value = penalties[bestIndex].Value;
          solution.Distance.Value = distance;
        }
      }

      return base.Apply();
    }
  }
}