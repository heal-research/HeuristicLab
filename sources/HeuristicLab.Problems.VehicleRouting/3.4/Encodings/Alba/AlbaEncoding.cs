#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaEncoding", "Represents an Alba encoding of VRP solutions. It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaEncoding : PermutationEncoding  {   
    #region IVRPEncoding Members
    public override List<Tour> GetTours() {
      Repair();
      List<Tour> result = new List<Tour>();

      int cities = ProblemInstance.Cities.Value;
      
      Tour tour = new Tour();
      for (int i = 0; i < this.array.Length; i++) {
        if (this.array[i] >= cities) {
          if (tour.Stops.Count > 0) {
            result.Add(tour);

            tour = new Tour();
          }
        } else {
          tour.Stops.Add(this.array[i] + 1);
        }
      }

      if (tour.Stops.Count > 0) {
        result.Add(tour);
      }

      return result;
    }

    public override int GetVehicleAssignment(int tour) {
      int vehicleAssignment = -1;
      Tour currentTour = GetTours()[tour];

      int lastStop = currentTour.Stops[
        currentTour.Stops.Count - 1] - 1;

      int lastStopIndex = this.IndexOf(lastStop);

      if (lastStopIndex == this.Length - 1) {
        int i = this.Length - 1;

        while (vehicleAssignment == -1) {
          if (this.array[i] >= ProblemInstance.Cities.Value) {
            vehicleAssignment = this.array[i] - ProblemInstance.Cities.Value;
          }
          
          i--;
        }
      }
      else
        vehicleAssignment = this[lastStopIndex + 1] - this.ProblemInstance.Cities.Value;

      return vehicleAssignment;
    }
    #endregion

    public void Repair() {
      int cities = ProblemInstance.Cities.Value;

      if (this[this.Length - 1] < cities) {
        int index = this.Length - 2;
        while (this[index] < cities) {
          index--;
        }

        int vehicle = this[index];
        for (int i = index; i < this.Length - 1; i++)
          this[i] = this[i + 1];
        this[Length - 1] = vehicle;
      }
    }

    public AlbaEncoding(Permutation permutation, IVRPProblemInstance instance)
      : base(permutation, instance) {
    }

    [StorableConstructor]
    protected AlbaEncoding(bool serializing)
      : base(serializing) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaEncoding(this, cloner);
    }

    protected AlbaEncoding(AlbaEncoding original, Cloner cloner)
      : base(original, cloner) {
    }

    public static AlbaEncoding ConvertFrom(List<int> routeParam, IVRPProblemInstance instance) {
      List<int> route = new List<int>(routeParam);
      
      int cities = instance.Cities.Value;

      for (int i = 0; i < route.Count; i++) {
        if (route[i] <= 0) {
          int vehicle = -route[i];
          route[i] = cities + vehicle;
        } else {
          route[i] = route[i] - 1;
        }
      }

      return new AlbaEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()),
        instance);
    }

    public static AlbaEncoding ConvertFrom(IVRPEncoding encoding, IVRPProblemInstance instance) {
      List<Tour> tours = encoding.GetTours();

      int cities = 0;
      foreach (Tour tour in tours) {
        cities += tour.Stops.Count;
      }

      int emptyVehicles = instance.Vehicles.Value - tours.Count;

      int[] array = new int[cities + tours.Count + emptyVehicles];
      int delimiter = 0;
      int arrayIndex = 0;
      
      foreach (Tour tour in tours) {
        foreach (int city in tour.Stops) {
          array[arrayIndex] = city - 1;
          arrayIndex++;
        }

        if (arrayIndex != array.Length) {
          array[arrayIndex] = cities + encoding.GetVehicleAssignment(delimiter);
          
          delimiter++;
          arrayIndex++;
        }
      }

      for (int i = 0; i < emptyVehicles; i++) {
        array[arrayIndex] = cities + encoding.GetVehicleAssignment(delimiter);

        delimiter++;
        arrayIndex++;
      }

      AlbaEncoding solution = new AlbaEncoding(new Permutation(PermutationTypes.RelativeUndirected, new IntArray(array)), instance);

      return solution;
    }
  }
}
