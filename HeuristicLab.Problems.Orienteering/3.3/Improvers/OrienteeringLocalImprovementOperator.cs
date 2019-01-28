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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.Orienteering {
  /// <summary>
  /// Iterative improvement consists of three basic operators: shortening, vertex insert and vertex
  /// exchange. The shortening operator tries to rearrange the vertices within a tour in order to
  /// minimize the cost of the tour. As shortening operator a 2-opt is applied. (Schilde et. al. 2009)
  /// </summary>
  [Item("OrienteeringLocalImprovementOperator", @"Implements the iterative improvement procedure described in Schilde M., Doerner K.F., Hartl R.F., Kiechle G. 2009. Metaheuristics for the bi-objective orienteering problem. Swarm Intelligence, Volume 3, Issue 3, pp 179-201.")]
  [StorableType("92FA69B3-F243-4D12-A67A-AA1D7EBCD302")]
  public sealed class OrienteeringLocalImprovementOperator : SingleSuccessorOperator, ILocalImprovementOperator {

    #region Parameter Properties
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["OrienteeringSolution"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<DoubleArray> ScoresParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Scores"]; }
    }
    public ILookupParameter<DoubleValue> MaximumDistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MaximumDistance"]; }
    }
    public ILookupParameter<IntValue> StartingPointParameter {
      get { return (ILookupParameter<IntValue>)Parameters["StartingPoint"]; }
    }
    public ILookupParameter<IntValue> TerminalPointParameter {
      get { return (ILookupParameter<IntValue>)Parameters["TerminalPoint"]; }
    }
    public ILookupParameter<DoubleValue> PointVisitingCostsParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PointVisitingCosts"]; }
    }
    #region ILocalImprovementOperator Parameters
    public IValueLookupParameter<IntValue> LocalIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["LocalIterations"]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    #endregion
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueParameter<IntValue> MaximumBlockLengthParmeter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumBlockLength"]; }
    }
    public IValueParameter<BoolValue> UseMaximumBlockLengthParmeter {
      get { return (IValueParameter<BoolValue>)Parameters["UseMaximumBlockLength"]; }
    }
    #endregion

    [StorableConstructor]
    private OrienteeringLocalImprovementOperator(StorableConstructorFlag _) : base(_) { }
    private OrienteeringLocalImprovementOperator(OrienteeringLocalImprovementOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public OrienteeringLocalImprovementOperator()
      : base() {
      Parameters.Add(new LookupParameter<IntegerVector>("OrienteeringSolution", "The Orienteering Solution given in path representation."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the points."));
      Parameters.Add(new LookupParameter<DoubleArray>("Scores", "The scores of the points."));
      Parameters.Add(new LookupParameter<DoubleValue>("MaximumDistance", "The maximum distance constraint for a Orienteering solution."));
      Parameters.Add(new LookupParameter<IntValue>("StartingPoint", "Index of the starting point."));
      Parameters.Add(new LookupParameter<IntValue>("TerminalPoint", "Index of the ending point."));
      Parameters.Add(new LookupParameter<DoubleValue>("PointVisitingCosts", "The costs for visiting a point."));

      Parameters.Add(new ValueLookupParameter<IntValue>("LocalIterations", "The number of iterations that have already been performed.", new IntValue(0)));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(150)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated moves."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The name of the collection where the results are stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the solution."));

      Parameters.Add(new ValueParameter<IntValue>("MaximumBlockLength", "The maximum length of the 2-opt shortening.", new IntValue(30)));
      Parameters.Add(new ValueParameter<BoolValue>("UseMaximumBlockLength", "Use a limitation of the length for the 2-opt shortening.", new BoolValue(false)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringLocalImprovementOperator(this, cloner);
    }

    public override IOperation Apply() {
      int numPoints = ScoresParameter.ActualValue.Length;
      var distances = DistanceMatrixParameter.ActualValue;
      var scores = ScoresParameter.ActualValue;
      double pointVisitingCosts = PointVisitingCostsParameter.ActualValue.Value;
      double maxLength = MaximumDistanceParameter.ActualValue.Value;
      int maxIterations = MaximumIterationsParameter.ActualValue.Value;
      int maxBlockLength = MaximumBlockLengthParmeter.Value.Value;
      bool useMaxBlockLength = UseMaximumBlockLengthParmeter.Value.Value;

      bool solutionChanged = true;

      var tour = IntegerVectorParameter.ActualValue.ToList();

      double tourLength = 0;
      double tourScore = tour.Sum(point => scores[point]);

      var localIterations = LocalIterationsParameter.ActualValue;
      var evaluatedSolutions = EvaluatedSolutionsParameter.ActualValue;
      int evaluations = 0;

      // Check if the tour can be improved by adding or replacing points
      while (solutionChanged && localIterations.Value < maxIterations) {
        solutionChanged = false;

        if (localIterations.Value == 0)
          tourLength = distances.CalculateTourLength(tour, pointVisitingCosts);

        // Try to shorten the path
        ShortenPath(tour, distances, maxBlockLength, useMaxBlockLength, ref tourLength, ref evaluations);

        // Determine all points that have not yet been visited by this tour
        var visitablePoints = Enumerable.Range(0, numPoints).Except(tour).ToList();

        // Determine if any of the visitable points can be included at any position within the tour
        IncludeNewPoints(tour, visitablePoints,
          distances, pointVisitingCosts, maxLength, scores,
          ref tourLength, ref tourScore, ref evaluations, ref solutionChanged);

        // Determine if any of the visitable points can take the place of an already visited point in the tour to improve the scores
        ReplacePoints(tour, visitablePoints,
          distances, maxLength, scores,
          ref tourLength, ref tourScore, ref evaluations, ref solutionChanged);

        localIterations.Value++;
      }

      localIterations.Value = 0;
      evaluatedSolutions.Value += evaluations;

      // Set new tour
      IntegerVectorParameter.ActualValue = new IntegerVector(tour.ToArray());
      QualityParameter.ActualValue.Value = tourScore;

      return base.Apply();
    }

    private void ShortenPath(List<int> tour, DistanceMatrix distances, int maxBlockLength, bool useMaxBlockLength, ref double tourLength, ref int evaluations) {
      bool solutionChanged;
      int pathSize = tour.Count;
      maxBlockLength = (useMaxBlockLength && (pathSize > maxBlockLength + 1)) ? maxBlockLength : (pathSize - 2);

      // Perform a 2-opt
      do {
        solutionChanged = false;

        for (int blockLength = 2; blockLength < maxBlockLength; blockLength++) {
          // If an optimization has been done, start from the beginning
          if (solutionChanged) break;

          for (int position = 1; position < (pathSize - blockLength); position++) {
            // If an optimization has been done, start from the beginning
            if (solutionChanged) break;

            evaluations++;

            double newLength = tourLength;
            // Recalculate length of whole swapped part, in case distances are not symmetric
            for (int index = position - 1; index < position + blockLength; index++) newLength -= distances[tour[index], tour[index + 1]];
            for (int index = position + blockLength - 1; index > position; index--) newLength += distances[tour[index], tour[index - 1]];
            newLength += distances[tour[position - 1], tour[position + blockLength - 1]];
            newLength += distances[tour[position], tour[position + blockLength]];

            if (newLength < tourLength - 0.00001) {
              // Avoid cycling caused by precision
              var reversePart = tour.GetRange(position, blockLength).AsEnumerable().Reverse();

              tour.RemoveRange(position, blockLength);
              tour.InsertRange(position, reversePart);

              tourLength = newLength;

              // Re-run the optimization
              solutionChanged = true;
            }
          }
        }
      } while (solutionChanged);
    }

    private void IncludeNewPoints(List<int> tour, List<int> visitablePoints,
      DistanceMatrix distances, double pointVisitingCosts, double maxLength, DoubleArray scores,
      ref double tourLength, ref double tourScore, ref int evaluations, ref bool solutionChanged) {

      for (int tourPosition = 1; tourPosition < tour.Count; tourPosition++) {
        // If an optimization has been done, start from the beginning
        if (solutionChanged) break;

        for (int i = 0; i < visitablePoints.Count; i++) {
          // If an optimization has been done, start from the beginning
          if (solutionChanged) break;

          evaluations++;

          double detour = distances.CalculateInsertionCosts(tour, tourPosition, visitablePoints[i], pointVisitingCosts);

          // Determine if including the point does not violate any constraint
          if (tourLength + detour <= maxLength) {
            // Insert the new point at this position
            tour.Insert(tourPosition, visitablePoints[i]);

            // Update the overall tour tourLength and score
            tourLength += detour;
            tourScore += scores[visitablePoints[i]];

            // Re-run this optimization
            solutionChanged = true;
          }
        }
      }
    }

    private void ReplacePoints(List<int> tour, List<int> visitablePoints,
      DistanceMatrix distances, double maxLength, DoubleArray scores,
      ref double tourLength, ref double tourScore, ref int evaluations, ref bool solutionChanged) {

      for (int tourPosition = 1; tourPosition < tour.Count - 1; tourPosition++) {
        // If an optimization has been done, start from the beginning
        if (solutionChanged) break;

        for (int i = 0; i < visitablePoints.Count; i++) {
          // If an optimization has been done, start from the beginning
          if (solutionChanged) break;

          evaluations++;

          double detour = distances.CalculateReplacementCosts(tour, tourPosition, visitablePoints[i]);

          double oldPointScore = scores[tour[tourPosition]];
          double newPointScore = scores[visitablePoints[i]];

          if ((tourLength + detour <= maxLength) && (newPointScore > oldPointScore)) {
            // Replace the old point by the new one
            tour[tourPosition] = visitablePoints[i];

            // Update the overall tour tourLength
            tourLength += detour;

            // Update the scores achieved by visiting this point
            tourScore += newPointScore - oldPointScore;

            // Re-run this optimization
            solutionChanged = true;
          }
        }
      }
    }
  }
}