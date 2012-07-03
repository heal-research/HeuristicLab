#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("IterativeInsertionCreator", "Creates a randomly initialized VRP solution using the iterative insertion Heuristic.")]
  [StorableClass]
  public sealed class IterativeInsertionCreator : DefaultRepresentationCreator, IStochasticOperator {
    #region IStochasticOperator Members
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    [StorableConstructor]
    private IterativeInsertionCreator(bool deserializing) : base(deserializing) { }
    private IterativeInsertionCreator(IterativeInsertionCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IterativeInsertionCreator(this, cloner);
    }

    public IterativeInsertionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    private static double CalculateAngleToDepot(DoubleMatrix coordinates, int city) {
      double dx = coordinates[0, 0];
      double dy = coordinates[0, 1];

      double cx = coordinates[city, 0];
      double cy = coordinates[city, 1];

      double alpha = Math.Atan((cx - dx) / (dy - cy)) * (180.0 / Math.PI);
      if (cx > dx && cy > dy)
        alpha = (90.0 + alpha) + 90.0;
      else if (cx < dx && cy > dy)
        alpha = alpha + 180.0;
      else if (cx < dx && cy < dy)
        alpha = (90.0 + alpha) + 270.0;

      return alpha;
    }

    private int FindBestInsertionPlace(Tour tour, int city, DoubleArray dueTime,
      DoubleArray serviceTime, DoubleArray readyTime, DoubleArray demand, DoubleValue capacity,
      DistanceMatrix distMatrix) {
      int place = -1;
      double minQuality = -1;

      for (int i = 0; i <= tour.Cities.Count; i++) {
        tour.Cities.Insert(i, city);
        
        TourEvaluation evaluation = VRPEvaluator.EvaluateTour(tour,
          dueTime, serviceTime, readyTime, demand, capacity,
          new DoubleValue(0), new DoubleValue(0), new DoubleValue(1), new DoubleValue(100), new DoubleValue(100),
            distMatrix);
        double quality = evaluation.Quality;

        if (place < 0 || quality < minQuality) {
          place = i;
          minQuality = quality;
        }

        tour.Cities.RemoveAt(i);
      }

      if (place == -1)
        place = 0;

      return place;
    }

    protected override List<int> CreateSolution() {
      int cities = Cities;
      int vehicles = VehiclesParameter.ActualValue.Value;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      IRandom random = RandomParameter.ActualValue;
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;
      BoolValue useDistanceMatrix = UseDistanceMatrixParameter.ActualValue;

      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, DistanceMatrixParameter, useDistanceMatrix);

      PotvinEncoding solution = new PotvinEncoding();

      List<int> customers = new List<int>();
      for (int i = 0; i < Cities; i++)
        customers.Add(i + 1);
      customers.Sort(delegate(int city1, int city2) {
            double angle1 = CalculateAngleToDepot(coordinates, city1);
            double angle2 = CalculateAngleToDepot(coordinates, city2);

            return angle1.CompareTo(angle2);
          });

      Tour currentTour = new Tour();
      solution.Tours.Add(currentTour);
      int j = random.Next(customers.Count);
      for (int i = 0; i < customers.Count; i++) {
        int index = (i + j) % customers.Count;

        int stopIdx = FindBestInsertionPlace(currentTour, customers[index],
          dueTime, serviceTime, readyTime, demand, capacity,
            distMatrix);
        currentTour.Cities.Insert(stopIdx, customers[index]);

        TourEvaluation evaluation = VRPEvaluator.EvaluateTour(currentTour, 
          dueTime, serviceTime, readyTime, demand, capacity, 
          new DoubleValue(0), new DoubleValue(0), new DoubleValue(1), new DoubleValue(100), new DoubleValue(100),
            distMatrix);
        if (solution.Tours.Count < vehicles &&
          ((evaluation.Overload > 0 || evaluation.Tardiness > 0))) {
          currentTour.Cities.RemoveAt(stopIdx);

          currentTour = new Tour();
          solution.Tours.Add(currentTour);
          currentTour.Cities.Add(customers[index]);
        }
      }

      List<int> result = new List<int>();

      bool first = true;
      foreach (Tour tour in solution.GetTours()) {
        if (first)
          first = false;
        else
          result.Add(0);

        result.AddRange(tour.Cities);
      }

      while (result.Count < cities + vehicles)
        result.Add(0);

      return result;
    }
  }
}
