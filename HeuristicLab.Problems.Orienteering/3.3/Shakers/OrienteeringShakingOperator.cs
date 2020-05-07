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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Orienteering {
  /// <summary>
  /// The used neighborhood operator is based on a two point exchange move. A move in
  /// the k-th neighborhood consists of removing k consecutive vertices from the tour, starting
  /// at a randomly selected position. Afterwards, a sorted list of all vertices not yet included
  /// in the current tour is built. The vertices are sorted in descending order with respect to the
  /// objective value increase using the current weights. Out of the first three entries with the
  /// highest ranking in this list, one randomly selected vertex is reinserted into the current tour
  /// at the same position as the removed vertices. This way, l new vertices are inserted into the
  /// tour. The largest neighborhood is a complete exchange of all vertices on the tour. 
  /// The shaking procedure does not guarantee that the new tour does not exceed the cost
  /// limit Tmax. Therefore, in a repair step, a sorted list of all vertices in the tour is created. The
  /// vertices are sorted in descending order with respect to costs saved when removing the vertex
  /// from the tour. Vertices are removed as long as the cost limit is violated.
  /// (Schilde et. al. 2009)
  /// </summary>
  [Item("OrienteeringShakingOperator", @"Implements the shaking procedure described in Schilde M., Doerner K.F., Hartl R.F., Kiechle G. 2009. Metaheuristics for the bi-objective orienteering problem. Swarm Intelligence, Volume 3, Issue 3, pp 179-201.")]
  [StorableType("D6654BD1-63CD-4057-89C8-36D1EE6EA7DF")]
  public sealed class OrienteeringShakingOperator : SingleSuccessorOperator, IMultiNeighborhoodShakingOperator, IStochasticOperator {

    #region Shaking Parameter Properties
    public IValueLookupParameter<IntValue> CurrentNeighborhoodIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["CurrentNeighborhoodIndex"]; }
    }
    public ILookupParameter<IntValue> NeighborhoodCountParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NeighborhoodCount"]; }
    }
    #endregion

    #region Parameter Properties
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    public ILookupParameter<IOrienteeringProblemData> OrienteeringProblemDataParameter {
      get { return (ILookupParameter<IOrienteeringProblemData>)Parameters["OrienteeringProblemData"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    [StorableConstructor]
    private OrienteeringShakingOperator(StorableConstructorFlag _) : base(_) { }
    private OrienteeringShakingOperator(OrienteeringShakingOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public OrienteeringShakingOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("CurrentNeighborhoodIndex", "The index of the operator that should be applied (k)."));
      Parameters.Add(new LookupParameter<IntValue>("NeighborhoodCount", "The number of operators that are available."));

      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The Orienteering Solution given in path representation."));
      Parameters.Add(new LookupParameter<IOrienteeringProblemData>("OrienteeringProblemData", "The main data that comprises the orienteering problem."));

      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringShakingOperator(this, cloner);
    }

    public override IOperation Apply() {
      var initialTour = IntegerVectorParameter.ActualValue;
      var data = OrienteeringProblemDataParameter.ActualValue;
      int numPoints = data.RoutingData.Cities;

      if (NeighborhoodCountParameter.ActualValue == null)
        NeighborhoodCountParameter.ActualValue = new IntValue(initialTour.Length);
      else NeighborhoodCountParameter.ActualValue.Value = initialTour.Length;

      var random = RandomParameter.ActualValue;

      if (initialTour.Length > 2) {
        // Limit the neighborhood to the tour length
        int currentNeighborhood = CurrentNeighborhoodIndexParameter.ActualValue.Value + 1;
        int maximumNeighborhood = initialTour.Length - 2; // neighborhood limit within [0, changable tour length - 1)
        maximumNeighborhood = (maximumNeighborhood > currentNeighborhood) ? currentNeighborhood : maximumNeighborhood;
        int neighborhood = maximumNeighborhood;

        // Find all points that are not yet included in the tour and are
        // within the maximum distance allowed (ellipse)
        // and sort them with regard to their utility 
        var visitablePoints = (
          from point in Enumerable.Range(0, numPoints)
          // Calculate the distance when going from the starting point to this point and then to the end point
          let distance = data.RoutingData.GetDistance(data.StartingPoint, point) + data.RoutingData.GetDistance(point, data.TerminalPoint) + data.PointVisitingCosts
          // If this distance is feasible and the point is neither starting nor ending point, check the point
          where distance < data.MaximumTravelCosts && point != data.StartingPoint && point != data.TerminalPoint
          // The point was not yet visited, so add it to the candidate list
          where !initialTour.Contains(point)
          // Calculate the utility of the point at this position
          let utility = data.GetScore(point)
          orderby utility
          select point
          ).ToList();

        // Initialize the new tour
        var actualTour = new List<int> { data.StartingPoint };

        // Perform the insertions according to the utility sorting
        InsertPoints(actualTour, initialTour, neighborhood, visitablePoints, random);

        // Bring the tour back to be feasible 
        CleanupTour(actualTour, data);

        // Set new Tour
        IntegerVectorParameter.ActualValue = new IntegerVector(actualTour.ToArray());
      }

      return base.Apply();
    }

    private void InsertPoints(List<int> actualTour, IntegerVector initialTour,
      int neighborhood, List<int> visitablePoints, IRandom random) {

      // Elect the starting index of the part to be replaced
      int tourSize = initialTour.Length;
      int randomPosition = random.Next(tourSize - neighborhood - 1) + 1;

      for (int position = 1; position < tourSize; position++) {
        if ((position < randomPosition) || (position > (randomPosition + neighborhood - 1))) {
          // Copy from initial tour when outside shaking range
          actualTour.Add(initialTour[position]);

          // Delete this point from the candidate list
          visitablePoints.Remove(initialTour[position]);
        } else {
          // Point from within shaking range
          if (visitablePoints.Count > 0) {
            // Add the point with the highest utility from the candidate list
            int randomFactor = random.Next(3);
            int insertionIndex = visitablePoints.Count - 1;
            if (visitablePoints.Count > 4) insertionIndex -= randomFactor;

            actualTour.Add(visitablePoints[insertionIndex]);

            // Delete this point from the candidate list
            visitablePoints.RemoveAt(insertionIndex);
          } else {
            // We don't have any points left that could be inserted so we can only re-insert
            // the removed and not already re-inserted points in a random order
            for (int reinsertPosition = randomPosition; reinsertPosition < randomPosition + neighborhood; reinsertPosition++) {
              bool alreadyReinserted = actualTour.Contains(initialTour[reinsertPosition]);

              if (!alreadyReinserted)
                visitablePoints.Add(initialTour[reinsertPosition]);
            }

            int randomIndex = random.Next(visitablePoints.Count);
            actualTour.Add(visitablePoints[randomIndex]);
            visitablePoints.Clear();
          }
        }
      }
    }

    private void CleanupTour(List<int> actualTour, IOrienteeringProblemData data) {
      // Sort the points on the tour according to their costs savings when removed
      var distanceSavings = (
        from removePosition in Enumerable.Range(1, actualTour.Count - 2)
        let saving = OrienteeringProblem.CalculateRemovementSaving(data, actualTour, removePosition)
        orderby saving descending
        select new SavingInfo { Index = removePosition, Saving = saving }
      ).ToList();

      double tourLength = OrienteeringProblem.CalculateTravelCosts(data, actualTour);

      // As long as the created path is infeasible, remove elements
      while (tourLength > data.MaximumTravelCosts) {
        // Remove the point that frees the largest distance
        // Note, distance savings are not updated after removal
        tourLength -= OrienteeringProblem.CalculateRemovementSaving(data, actualTour, distanceSavings[0].Index);
        actualTour.RemoveAt(distanceSavings[0].Index);

        // Shift indices due to removal of a point in the tour
        for (int i = 1; i < distanceSavings.Count; i++) {
          if (distanceSavings[i].Index > distanceSavings[0].Index) {
            distanceSavings[i].Index--;
          }
        }
        distanceSavings.RemoveAt(0);
      }
    }

    [StorableType("8E380F4E-D9C2-4671-ABF8-A28E14B22BA2")]
    private class SavingInfo {
      public int Index;
      public double Saving;
    }
  }
}