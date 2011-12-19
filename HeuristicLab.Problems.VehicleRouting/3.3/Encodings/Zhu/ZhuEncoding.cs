#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuEncoding", "Represents a Zhu encoding of VRP solutions. It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableClass]
  public class ZhuEncoding : PermutationEncoding {
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

    #region IVRPEncoding Members
    public override List<Tour> GetTours(ILookupParameter<DoubleMatrix> distanceMatrix, int maxVehicles = int.MaxValue) {
      List<Tour> result = new List<Tour>();

      Tour newTour = new Tour();

      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, distanceMatrix, useDistanceMatrix);

      for (int i = 0; i < this.Length; i++) {
        int city = this[i] + 1;
        newTour.Cities.Add(city);
        if (!newTour.Feasible(
          dueTimeArray,
          serviceTimeArray,
          readyTimeArray,
          demandArray,
          capacity,
          distMatrix)) {
          newTour.Cities.Remove(city);
          if (newTour.Cities.Count > 0)
            result.Add(newTour);

          newTour = new Tour();
          newTour.Cities.Add(city);
        }
      }

      if (newTour.Cities.Count > 0)
        result.Add(newTour);

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
    protected ZhuEncoding(bool deserializing) : base(deserializing) { }
    protected ZhuEncoding(ZhuEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.cities = original.cities;
      this.dueTimeArray = original.dueTimeArray;
      this.serviceTimeArray = original.serviceTimeArray;
      this.readyTimeArray = original.readyTimeArray;
      this.demandArray = original.demandArray;
      this.capacity = original.capacity;
      this.coordinates = original.coordinates;
      this.useDistanceMatrix = original.useDistanceMatrix;
    }
    public ZhuEncoding(Permutation permutation, int cities,
     DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
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
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuEncoding(this, cloner);
    }

    public static ZhuEncoding ConvertFrom(IVRPEncoding encoding, int cities,
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix) {
      List<Tour> tours = encoding.GetTours(distanceMatrix);
      List<int> route = new List<int>();

      foreach (Tour tour in tours) {
        foreach (int city in tour.Cities)
          route.Add(city - 1);
      }

      return new ZhuEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), cities,
        dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
        coordinates, useDistanceMatrix);
    }

    public static ZhuEncoding ConvertFrom(List<int> routeParam, int cities,
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleMatrix coordinates, BoolValue useDistanceMatrix) {
      List<int> route = new List<int>(routeParam);

      while (route.Remove(0)) { //remove all delimiters (0)
      }

      for (int i = 0; i < route.Count; i++)
        route[i]--;

      return new ZhuEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), cities,
        dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
        coordinates, useDistanceMatrix);
    }
  }
}
