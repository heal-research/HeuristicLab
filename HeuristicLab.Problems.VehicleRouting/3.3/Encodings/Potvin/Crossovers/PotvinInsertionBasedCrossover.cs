#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
using HeuristicLab.Data;
using System;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinInsertionBasedCrossover", "The IBX crossover for VRP representations. It is implemented as described in Berger, J and Solois, M and Begin, R (1998). A hybrid genetic algorithm for the vehicle routing problem with time windows. LNCS 1418. Springer, London 114-127.")]
  [StorableClass]
  public sealed class PotvinInsertionBasedCrossover : PotvinCrossover {
    public IValueParameter<IntValue> Length {
      get { return (IValueParameter<IntValue>)Parameters["Length"]; }
    }

    [StorableConstructor]
    private PotvinInsertionBasedCrossover(bool deserializing) : base(deserializing) { }
    private PotvinInsertionBasedCrossover(PotvinInsertionBasedCrossover original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinInsertionBasedCrossover(this, cloner);
    }
    public PotvinInsertionBasedCrossover()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Length", "The maximum length of the replaced route.", new IntValue(1)));
    }

    private static int SelectRandomTourBiasedByLength(IRandom random, PotvinEncoding individual) {
      int tourIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[individual.Tours.Count];
      for (int i = 0; i < individual.Tours.Count; i++) {
        probabilities[i] = 1.0 / ((double)individual.Tours[i].Cities.Count / (double)individual.Cities);
        sum += probabilities[i];
      }

      double rand = random.NextDouble() * sum;
      double cumulatedProbabilities = 0.0;
      int index = 0;
      while (tourIndex == -1 && index < probabilities.Length) {
        if (cumulatedProbabilities <= rand && rand <= cumulatedProbabilities + probabilities[index])
          tourIndex = index;

        cumulatedProbabilities += probabilities[index];
        index++;
      }

      return tourIndex;
    }

    private double CalculateCentroidDistance(Tour t1, Tour t2, DoubleMatrix coordinates) {
      double xSum = 0;
      double ySum = 0;
      double c1X, c1Y, c2X, c2Y;

      for (int i = 0; i < t1.Cities.Count; i++) {
        xSum += coordinates[t1.Cities[i], 0];
        ySum += coordinates[t1.Cities[i], 1];
      }
      c1X = xSum / t1.Cities.Count;
      c1Y = ySum / t1.Cities.Count;

      for (int i = 0; i < t2.Cities.Count; i++) {
        xSum += coordinates[t2.Cities[i], 0];
        ySum += coordinates[t2.Cities[i], 1];
      }
      c2X = xSum / t1.Cities.Count;
      c2Y = ySum / t1.Cities.Count;

      return Math.Sqrt(
           (c1X - c2X) * (c1X - c2X) +
           (c1Y - c2Y) * (c1Y - c2Y));
    }

    private double CalculateMeanCentroidDistance(Tour t1, IList<Tour> tours, DoubleMatrix coordinates) {
      double sum = 0;

      for (int i = 0; i < tours.Count; i++) {
        sum += CalculateCentroidDistance(t1, tours[i], coordinates);
      }

      return sum / tours.Count;
    }

    private int SelectCityBiasedByNeighborDistance(IRandom random, Tour tour, DistanceMatrix distMatrix) {
      int cityIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[tour.Cities.Count];
      for (int i = 0; i < tour.Cities.Count; i++) {
        int next;
        if (i + 1 >= tour.Cities.Count)
          next = 0;
        else
          next = tour.Cities[i + 1];
        double distance = VRPUtilities.GetDistance(
          tour.Cities[i], next, distMatrix);

        int prev;
        if (i - 1 < 0)
          prev = 0;
        else
          prev = tour.Cities[i - 1];
        distance += VRPUtilities.GetDistance(
          tour.Cities[i], prev, distMatrix);

        probabilities[i] = distance;
        sum += probabilities[i];
      }

      double rand = random.NextDouble() * sum;
      double cumulatedProbabilities = 0.0;
      int index = 0;
      while (cityIndex == -1 && index < probabilities.Length) {
        if (cumulatedProbabilities <= rand && rand <= cumulatedProbabilities + probabilities[index])
          cityIndex = index;

        cumulatedProbabilities += probabilities[index];
        index++;
      }

      return cityIndex;
    }

    private bool FindRouteInsertionPlace(
      Tour tour,
      DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DistanceMatrix distMatrix, int city, bool allowInfeasible, out int place) {
      place = -1;
      bool bestFeasible = false;
      double minDetour = 0;

      for (int i = 0; i <= tour.Cities.Count; i++) {
        double length = tour.GetLength(distMatrix);

        tour.Cities.Insert(i, city);

        bool feasible = tour.Feasible(dueTimeArray, serviceTimeArray, readyTimeArray, demandArray,
          capacity, distMatrix);

        if (feasible || allowInfeasible && !bestFeasible) {
          double newLength = tour.GetLength(distMatrix);
          double detour = newLength - length;

          if (place < 0 || detour < minDetour || feasible && !bestFeasible) {
            place = i;
            minDetour = detour;

            if (feasible)
              bestFeasible = true;
          }
        }

        tour.Cities.RemoveAt(i);
      }

      return place >= 0;
    }

    private ICollection<int> GetUnrouted(PotvinEncoding solution, int cities) {
      HashSet<int> undiscovered = new HashSet<int>();
      for (int i = 1; i <= cities; i++) {
        undiscovered.Add(i);
      }

      foreach (Tour tour in solution.Tours) {
        foreach (int city in tour.Cities)
          undiscovered.Remove(city);
      }

      return undiscovered;
    }

    protected override PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2) {
      PotvinEncoding child = new PotvinEncoding();

      BoolValue useDistanceMatrix = UseDistanceMatrixParameter.ActualValue;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, DistanceMatrixParameter, useDistanceMatrix);
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;

      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;

      List<Tour> R1 = new List<Tour>();
      PotvinEncoding p1Clone = parent1.Clone() as PotvinEncoding;

      int length = Math.Min(Length.Value.Value, parent1.Tours.Count) + 1;
      int k = random.Next(1, length);
      for (int i = 0; i < k; i++) {
        int index = SelectRandomTourBiasedByLength(random, p1Clone);
        R1.Add(p1Clone.Tours[index]);
        p1Clone.Tours.RemoveAt(index);
      }

      foreach (Tour r1 in R1) {
        List<int> R2 = new List<int>();

        double r = CalculateMeanCentroidDistance(r1, parent2.Tours, coordinates);
        foreach (Tour tour in parent2.Tours) {
          if (CalculateCentroidDistance(r1, tour, coordinates) <= r) {
            R2.AddRange(tour.Cities);
          }
        }

        Tour childTour = new Tour();
        childTour.Cities.AddRange(r1.Cities);

        //DESTROY - remove cities from r1
        int removed = random.Next(1, r1.Cities.Count + 1);
        for (int i = 0; i < removed; i++) {
          childTour.Cities.RemoveAt(SelectCityBiasedByNeighborDistance(random, childTour, distMatrix));
        }

        //REPAIR - add cities from R2
        int maxCount = random.Next(1, Math.Min(5, R2.Count));
        int count = 0;

        while (count < maxCount && R2.Count != 0) {
          PotvinEncoding newChild = child.Clone() as PotvinEncoding;
          newChild.Tours.Add(childTour);

          int index = random.Next(R2.Count);
          int city = R2[index];
          R2.RemoveAt(index);

          int place = -1;
          if (FindRouteInsertionPlace(childTour, dueTime, serviceTime, readyTime,
            demand, capacity, distMatrix, city, allowInfeasible, out place)) {
            childTour.Cities.Insert(place, city);

            if (!Repair(random, child, childTour, distMatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible)) {
              childTour.Cities.RemoveAt(place);
            } else {
              count++;
            }
          }
        }

        child.Tours.Add(childTour);
        Repair(random, child, childTour, distMatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible);
      }

      for (int i = 0; i < p1Clone.Tours.Count; i++) {
        Tour childTour = p1Clone.Tours[i].Clone() as Tour;
        child.Tours.Add(childTour);
        Repair(random, child, childTour, distMatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible);
      }

      //route unrouted customers
      child.Unrouted.AddRange(GetUnrouted(child, Cities));
      bool success = RouteUnrouted(child, distMatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible);

      if (success || allowInfeasible)
        return child;
      else {
        if (random.NextDouble() < 0.5)
          return parent1.Clone() as PotvinEncoding;
        else
          return parent2.Clone() as PotvinEncoding;
      }
    }
  }
}
