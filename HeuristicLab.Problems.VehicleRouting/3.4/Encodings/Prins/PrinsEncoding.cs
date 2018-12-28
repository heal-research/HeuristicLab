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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsEncoding", "Represents an Prins encoding of VRP solutions. It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableType("A0E1EBC1-C0F5-4CD5-9279-1A669C6633CD")]
  public class PrinsEncoding : General.PermutationEncoding {
    #region IVRPEncoding Members
    public override int GetTourIndex(Tour tour) {
      return 0;
    }

    public override List<Tour> GetTours() {
      List<Tour> result = new List<Tour>();

      int cities = ProblemInstance.Cities.Value;

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
          tour.Stops.Add(this[j - 1] + 1);

          VRPEvaluation eval =
            ProblemInstance.EvaluateTour(tour, this);

          double cost = eval.Quality;
          feasible = ProblemInstance.Feasible(eval);

          if (feasible || j == i) {
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
          trip.Stops.Add(this[k - 1] + 1);
        }

        if (trip.Stops.Count > 0)
          result.Add(trip);

        index2 = index;
      } while (index != 0);

      //if there are too many vehicles - repair
      while (result.Count > ProblemInstance.Vehicles.Value) {
        Tour tour = result[result.Count - 1];

        //find predecessor / successor in permutation
        int predecessorIndex = Array.IndexOf(this.array, tour.Stops[0] - 1) - 1;
        if (predecessorIndex >= 0) {
          int predecessor = this[predecessorIndex] + 1;

          foreach (Tour t in result) {
            int insertPosition = t.Stops.IndexOf(predecessor) + 1;
            if (insertPosition != -1) {
              t.Stops.InsertRange(insertPosition, tour.Stops);
              break;
            }
          }
        } else {
          int successorIndex = Array.IndexOf(this.array,
            tour.Stops[tour.Stops.Count - 1] - 1) + 1;
          int successor = this[successorIndex] + 1;

          foreach (Tour t in result) {
            int insertPosition = t.Stops.IndexOf(successor);
            if (insertPosition != -1) {
              t.Stops.InsertRange(insertPosition, tour.Stops);
              break;
            }
          }
        }

        result.Remove(tour);
      }

      return result;
    }
    #endregion
    public PrinsEncoding(Permutation permutation, IVRPProblemInstance problemInstance)
      : base(permutation, problemInstance) {
    }

    [StorableConstructor]
    protected PrinsEncoding(StorableConstructorFlag _) : base(_) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsEncoding(this, cloner);
    }

    protected PrinsEncoding(PrinsEncoding original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PrinsEncoding ConvertFrom(IVRPEncoding encoding, IVRPProblemInstance problemInstance) {
      List<Tour> tours = encoding.GetTours();
      List<int> route = new List<int>();

      foreach (Tour tour in tours) {
        foreach (int city in tour.Stops)
          route.Add(city - 1);
      }

      return new PrinsEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), problemInstance);
    }

    public static PrinsEncoding ConvertFrom(List<int> routeParam, IVRPProblemInstance problemInstance) {
      List<int> route = new List<int>(routeParam);

      while (route.Remove(0)) { //remove all delimiters (0)
      }

      for (int i = 0; i < route.Count; i++)
        route[i]--;

      return new PrinsEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), problemInstance);
    }
  }
}
