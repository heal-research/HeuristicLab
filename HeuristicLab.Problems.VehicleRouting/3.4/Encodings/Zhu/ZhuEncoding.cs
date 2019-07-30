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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuEncoding", "Represents a Zhu encoding of VRP solutions. It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableType("1A5F5A1D-E4F5-4477-887E-45FC488BC459")]
  public class ZhuEncoding : General.PermutationEncoding {
    #region IVRPEncoding Members
    public override int GetTourIndex(Tour tour) {
      return 0;
    }

    public override List<Tour> GetTours() {
      List<Tour> result = new List<Tour>();

      Tour newTour = new Tour();

      for (int i = 0; i < this.Length; i++) {
        int city = this[i] + 1;
        newTour.Stops.Add(city);
        if (!ProblemInstance.TourFeasible(newTour, this)) {
          newTour.Stops.Remove(city);
          if (newTour.Stops.Count > 0)
            result.Add(newTour);

          newTour = new Tour();
          newTour.Stops.Add(city);
        }
      }

      if (newTour.Stops.Count > 0)
        result.Add(newTour);

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

    public ZhuEncoding(Permutation permutation, IVRPProblemInstance problemInstance)
      : base(permutation, problemInstance) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuEncoding(this, cloner);
    }

    protected ZhuEncoding(ZhuEncoding original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected ZhuEncoding(StorableConstructorFlag _) : base(_) {
    }

    public static ZhuEncoding ConvertFrom(IVRPEncoding encoding, IVRPProblemInstance problemInstance) {
      List<Tour> tours = encoding.GetTours();
      List<int> route = new List<int>();

      foreach (Tour tour in tours) {
        foreach (int city in tour.Stops)
          route.Add(city - 1);
      }

      return new ZhuEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), problemInstance);
    }

    public static ZhuEncoding ConvertFrom(List<int> routeParam, IVRPProblemInstance problemInstance) {
      List<int> route = new List<int>(routeParam);

      while (route.Remove(0)) { //remove all delimiters (0)
      }

      for (int i = 0; i < route.Count; i++)
        route[i]--;

      return new ZhuEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()), problemInstance);
    }
  }
}
