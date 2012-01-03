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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsEncoding", "Represents an Prins encoding of VRP solutions. It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableClass]
  public class PrinsEncoding : PermutationEncoding {
    [Storable]
    private int cities;

    [Storable]
    private DoubleMatrix coordinates;

    [Storable]
    private BoolValue useDistanceMatrix;

    [Storable]
    DoubleArray dueTimeArray;
    
    [Storable]
    DoubleArray serviceTimeArray;

    [Storable]
    DoubleArray readyTimeArray;
    
    [Storable]
    DoubleArray demandArray;

    [Storable]
    DoubleValue capacity;

    [Storable]
    DoubleValue fleetUsageFactor;

    [Storable]
    DoubleValue timeFactor;

    [Storable]
    DoubleValue distanceFactor;

    [Storable]
    DoubleValue overloadPenalty;

    [Storable]
    DoubleValue tardinessPenalty;

    #region IVRPEncoding Members
    public override List<Tour> GetTours(ILookupParameter<DoubleMatrix> distanceMatrix, int maxVehicles = int.MaxValue) {
      List<Tour> result = new List<Tour>();

      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, distanceMatrix, useDistanceMatrix);

      //Split permutation into vector P
      int[] P = new int[cities + 1];
      for (int i = 0; i <= cities; i++)
        P[i] = -1;

      double[] V = new double[cities + 1];
      V[0] = 0;
      for (int i = 1; i <= cities; i++) {
        V[i] = double.MaxValue;
      }

      for (int i = 1; i <= cities; i++) {
        int j = i;
        Tour tour = new Tour();
        bool feasible = true;

        do {
          tour.Cities.Add(this[j-1] + 1);

          TourEvaluation eval = VRPEvaluator.EvaluateTour(tour,
            dueTimeArray,
            serviceTimeArray,
            readyTimeArray,
            demandArray,
            capacity,
            fleetUsageFactor,
            timeFactor,
            distanceFactor,
            overloadPenalty,
            tardinessPenalty,
            distMatrix);

          double cost = eval.Quality;
          feasible = eval.Overload < double.Epsilon && eval.Tardiness < double.Epsilon;

          if (feasible) {
            if (V[i - 1] + cost < V[j]) {
              V[j] = V[i - 1] + cost;
              P[j] = i - 1;
            }
            j++;
          }

        } while (j <= cities && feasible);
      }

      //extract VRP solution from vector P
      int index = 0;
      int index2 = cities;
      Tour trip = null;
      do {
        index = P[index2];
        trip = new Tour();

        for (int k = index + 1; k <= index2; k++) {
          trip.Cities.Add(this[k - 1] + 1);
        }

        if (trip.Cities.Count > 0)
          result.Add(trip);

        index2 = index;
      } while (index != 0);

      //if there are too many vehicles - repair
      while (result.Count > maxVehicles) {
        Tour tour = result[result.Count - 1];

        //find predecessor / successor in permutation
        int predecessorIndex = Array.IndexOf(this.array, tour.Cities[0] - 1) - 1;
        if (predecessorIndex >= 0) {
          int predecessor = this[predecessorIndex] + 1;

          foreach (Tour t in result) {
            int insertPosition = t.Cities.IndexOf(predecessor) + 1;
            if (insertPosition != -1) {
              t.Cities.InsertRange(insertPosition, tour.Cities);
              break;
            }
          }
        } else {
          int successorIndex = Array.IndexOf(this.array,
            tour.Cities[tour.Cities.Count - 1] - 1) + 1;
          int successor = this[successorIndex] + 1;

          foreach (Tour t in result) {
            int insertPosition = t.Cities.IndexOf(successor);
            if (insertPosition != -1) {
              t.Cities.InsertRange(insertPosition, tour.Cities);
              break;
            }
          }
        }

        result.Remove(tour);
      }

      return result;
    }
    #endregion
    
    [StorableConstructor]
    protected PrinsEncoding(bool deserializing) : base(deserializing) { }
    protected PrinsEncoding(PrinsEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.cities = original.cities;
      this.dueTimeArray = original.dueTimeArray;
      this.serviceTimeArray = original.serviceTimeArray;
      this.readyTimeArray = original.readyTimeArray;
      this.demandArray = original.demandArray;
      this.capacity = original.capacity;
      this.fleetUsageFactor = original.fleetUsageFactor;
      this.timeFactor = original.timeFactor;
      this.distanceFactor = original.distanceFactor;
      this.overloadPenalty = original.overloadPenalty;
      this.tardinessPenalty = original.tardinessPenalty;
      this.coordinates = original.coordinates;
      this.useDistanceMatrix = original.useDistanceMatrix;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsEncoding(this, cloner);
    }
    public PrinsEncoding(Permutation permutation, int cities,
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, BoolValue useDistanceMatrix)
      : base(permutation) {
        this.cities = cities;
        this.dueTimeArray = dueTimeArray;
        this.serviceTimeArray = serviceTimeArray;
        this.readyTimeArray = readyTimeArray;
        this.demandArray = demandArray;
        this.capacity = capacity;
        this.coordinates = coordinates;
        this.useDistanceMatrix = useDistanceMatrix;
        this.fleetUsageFactor = fleetUsageFactor;
        this.timeFactor = timeFactor;
        this.distanceFactor = distanceFactor;
        this.overloadPenalty = overloadPenalty;
        this.tardinessPenalty = tardinessPenalty;
    }

    public static PrinsEncoding ConvertFrom(IVRPEncoding encoding, int cities,
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix) {
      List<Tour> tours = encoding.GetTours(distanceMatrix);
      List<int> route = new List<int>();

      foreach (Tour tour in tours) {
        foreach (int city in tour.Cities)
          route.Add(city - 1);
      }

      return new PrinsEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), cities,
        dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
        fleetUsageFactor, timeFactor, distanceFactor, overloadPenalty, tardinessPenalty,
        coordinates, useDistanceMatrix);
    }

    public static PrinsEncoding ConvertFrom(List<int> routeParam, int cities, 
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, BoolValue useDistanceMatrix) {
      List<int> route = new List<int>(routeParam);

      while (route.Remove(0)) { //remove all delimiters (0)
      }

      for (int i = 0; i < route.Count; i++)
        route[i]--;

      return new PrinsEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), cities, 
        dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
        fleetUsageFactor, timeFactor, distanceFactor, overloadPenalty, tardinessPenalty,
        coordinates, useDistanceMatrix);
    }
  }
}
