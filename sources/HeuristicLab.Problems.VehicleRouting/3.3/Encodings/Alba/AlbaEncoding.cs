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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaEncoding", "Represents an Alba encoding of VRP solutions. It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaEncoding : Permutation, IVRPEncoding {
    [Storable]
    private int cities;
    
    #region IVRPEncoding Members
    public ItemList<Tour> Tours {
      get {
        ItemList<Tour> result = new ItemList<Tour>();
        
        Tour tour = new Tour();
        for (int i = 0; i < this.array.Length; i++) {
          if (this.array[i] >= cities) {
            if (tour.Cities.Count > 0) {
              result.Add(tour);

              tour = new Tour();
            }
          } else {
            tour.Cities.Add(this.array[i] + 1);
          }
        }

        if (tour.Cities.Count > 0) {
          result.Add(tour);
        }

        return result;
      }
    }

    public int Cities {
      get { return cities; }
    }

    public int MaxVehicles {
      get { return Length - Cities;  }
    }

    #endregion

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      AlbaEncoding clone = new AlbaEncoding(
        new Permutation(this.PermutationType, this.array), cities);
      cloner.RegisterClonedObject(this, clone);
      clone.readOnly = readOnly;
      return clone;
    }

    public AlbaEncoding(Permutation permutation, int cities)
      : base(PermutationTypes.RelativeUndirected) {
      this.array = new int[permutation.Length];
      for (int i = 0; i < array.Length; i++)
        this.array[i] = permutation[i];

      this.cities = cities;
    }

    [StorableConstructor]
    private AlbaEncoding(bool serializing)
      : base() {
    }

    public static AlbaEncoding ConvertFrom(IVRPEncoding encoding, int vehicles) {
      ItemList<Tour> tours = encoding.Tours;

      int cities = 0;
      foreach (Tour tour in tours) {
        cities += tour.Cities.Count;
      }

      int emptyVehicles = vehicles - tours.Count;

      int[] array = new int[cities + tours.Count + emptyVehicles - 1];
      int delimiter = cities;
      int arrayIndex = 0;

      foreach (Tour tour in tours) {
        foreach (int city in tour.Cities) {
            array[arrayIndex] = city - 1;
            arrayIndex++;
        }

        if (arrayIndex != array.Length) {
          array[arrayIndex] = delimiter;
          delimiter++;
          arrayIndex++;
        }
      }

      for (int i = 0; i < emptyVehicles - 1; i++) {
        array[arrayIndex] = delimiter;
        delimiter++;
        arrayIndex++;
      }
            
      AlbaEncoding solution = new AlbaEncoding(new Permutation(PermutationTypes.RelativeUndirected, new IntArray(array)), cities);

      return solution;
    }

    public static AlbaEncoding ConvertFrom(List<int> routeParam) {
      List<int> route = new List<int>(routeParam);
      
      int cities = 0;
      for (int i = 0; i < route.Count; i++) {
        if (route[i] != 0) {
          cities++;
        }
      }

      int vehicle = cities;
      for (int i = 0; i < route.Count; i++) {
        if (route[i] == 0) {
          route[i] = vehicle;
          vehicle++;
        } else {
          route[i] = route[i] - 1;
        }
      }

      return new AlbaEncoding(
        new Permutation(PermutationTypes.RelativeUndirected, route.ToArray()),
        cities);
    }

    internal static void RemoveUnusedParameters(ParameterCollection parameters) {
      parameters.Remove("DistanceMatrix");
      parameters.Remove("UseDistanceMatrix");
      parameters.Remove("Capacity");
      parameters.Remove("Demand");
      parameters.Remove("ReadyTime");
      parameters.Remove("DueTime");
      parameters.Remove("ServiceTime");
    }
  }
}
