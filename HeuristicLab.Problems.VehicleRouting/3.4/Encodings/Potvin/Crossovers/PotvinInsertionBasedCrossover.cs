#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

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
        probabilities[i] = 1.0 / ((double)individual.Tours[i].Stops.Count / (double)individual.Cities);
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

    private double CalculateCentroidDistance(Tour t1, Tour t2, IVRPProblemInstance instance) {
      double xSum = 0;
      double ySum = 0;
      double c1X, c1Y, c2X, c2Y;

      for (int i = 0; i < t1.Stops.Count; i++) {
        xSum += instance.GetCoordinates(t1.Stops[i])[0];
        ySum += instance.GetCoordinates(t1.Stops[i])[1];
      }
      c1X = xSum / t1.Stops.Count;
      c1Y = ySum / t1.Stops.Count;

      for (int i = 0; i < t2.Stops.Count; i++) {
        xSum += instance.GetCoordinates(t2.Stops[i])[0];
        ySum += instance.GetCoordinates(t2.Stops[i])[1];
      }
      c2X = xSum / t1.Stops.Count;
      c2Y = ySum / t1.Stops.Count;

      return Math.Sqrt(
           (c1X - c2X) * (c1X - c2X) +
           (c1Y - c2Y) * (c1Y - c2Y));
    }

    private double CalculateMeanCentroidDistance(Tour t1, IList<Tour> tours, IVRPProblemInstance instance) {
      double sum = 0;

      for (int i = 0; i < tours.Count; i++) {
        sum += CalculateCentroidDistance(t1, tours[i], instance);
      }

      return sum / tours.Count;
    }

    private int SelectCityBiasedByNeighborDistance(IRandom random, Tour tour, IVRPEncoding solution) {
      int cityIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[tour.Stops.Count];
      for (int i = 0; i < tour.Stops.Count; i++) {
        int next;
        if (i + 1 >= tour.Stops.Count)
          next = 0;
        else
          next = tour.Stops[i + 1];
        double distance = ProblemInstance.GetDistance(
          tour.Stops[i], next, solution);

        int prev;
        if (i - 1 < 0)
          prev = 0;
        else
          prev = tour.Stops[i - 1];
        distance += ProblemInstance.GetDistance(
          tour.Stops[i], prev, solution);

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
      PotvinEncoding individual,
      Tour tour,
      int city, bool allowInfeasible, out int place) {
      place = -1;

      if (tour.Stops.Contains(city))
        return false;

      if (tour.Stops.Count == 0) {
        place = 0;
        return true;
      }

      double minDetour = 0;
      VRPEvaluation eval = ProblemInstance.EvaluateTour(tour, individual);
      bool originalFeasible = ProblemInstance.Feasible(eval);

      for (int i = 0; i <= tour.Stops.Count; i++) {
        bool feasible;
        double detour = ProblemInstance.GetInsertionCosts(eval, individual, city, 0, i, out feasible);
        if (feasible || allowInfeasible) {
          if (place < 0 || detour < minDetour) {
            place = i;
            minDetour = detour;
          }
        }
      }

      return place >= 0;
    }

    private ICollection<int> GetUnrouted(PotvinEncoding solution, int cities) {
      HashSet<int> undiscovered = new HashSet<int>();
      for (int i = 1; i <= cities; i++) {
        undiscovered.Add(i);
      }

      foreach (Tour tour in solution.Tours) {
        foreach (int city in tour.Stops)
          undiscovered.Remove(city);
      }

      return undiscovered;
    }

    protected override PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2) {
      PotvinEncoding child = parent1.Clone() as PotvinEncoding;
      child.Tours.Clear();

      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;

      List<Tour> R1 = new List<Tour>();
      PotvinEncoding p1Clone = parent1.Clone() as PotvinEncoding;

      int length = Math.Min(Length.Value.Value, parent1.Tours.Count) + 1;
      int k = 1;
      if(length > 1)
        k = random.Next(1, length);
      for (int i = 0; i < k; i++) {
        int index = SelectRandomTourBiasedByLength(random, p1Clone);
        R1.Add(p1Clone.Tours[index]);
        p1Clone.Tours.RemoveAt(index);
      }

      foreach (Tour r1 in R1) {
        List<int> R2 = new List<int>();

        double r = CalculateMeanCentroidDistance(r1, parent2.Tours, ProblemInstance);
        foreach (Tour tour in parent2.Tours) {
          if (CalculateCentroidDistance(r1, tour, ProblemInstance) <= r) {
            R2.AddRange(tour.Stops);
          }
        }

        Tour childTour = new Tour();
        child.Tours.Add(childTour);
        childTour.Stops.AddRange(r1.Stops);

        //DESTROY - remove cities from r1
        int removed = 1;
        if(r1.Stops.Count > 1)
          removed = random.Next(1, r1.Stops.Count + 1);
        for (int i = 0; i < removed; i++) {
          childTour.Stops.RemoveAt(SelectCityBiasedByNeighborDistance(random, childTour, child));
        }

        //REPAIR - add cities from R2
        int maxCount = 1;
        if(R2.Count > 1)
          maxCount = random.Next(1, Math.Min(5, R2.Count));
        int count = 0;

        while (count < maxCount && R2.Count != 0) {
          int index = random.Next(R2.Count);
          int city = R2[index];
          R2.RemoveAt(index);

          int place = -1;
          bool found = FindRouteInsertionPlace(child, childTour, city, allowInfeasible, out place); 
          if (found) {
            childTour.Stops.Insert(place, city);

            if (!Repair(random, child, childTour, ProblemInstance, allowInfeasible)) {
              childTour.Stops.RemoveAt(place);
            } else {
              count++;
            }
          }
        }

        Repair(random, child, childTour, ProblemInstance, allowInfeasible);
      }

      for (int i = 0; i < p1Clone.Tours.Count; i++) {
        Tour childTour = p1Clone.Tours[i].Clone() as Tour;
        child.Tours.Add(childTour);
        Repair(random, child, childTour, ProblemInstance, allowInfeasible);
      }

      //route unrouted customers
      child.Unrouted.AddRange(GetUnrouted(child, ProblemInstance.Cities.Value));
      bool success = RouteUnrouted(child, allowInfeasible);

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
