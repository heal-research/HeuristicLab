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
using System.Drawing;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinEncoding", "Represents a potvin encoding of VRP solutions. It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public class PotvinEncoding : Item, IVRPEncoding {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }
    
    #region IVRPEncoding Members
    [Storable]
    public ItemList<Tour> Tours { get; set; }

    public int Cities {
      get 
      {
        int cities = 0;

        foreach (Tour tour in Tours) {
          cities += tour.Cities.Count;
        }

        return cities;
      }
    }
    #endregion

    [Storable]
    public List<int> Unrouted { get; set; }

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      PotvinEncoding clone = new PotvinEncoding();
      cloner.RegisterClonedObject(this, clone);
      clone.Tours = (ItemList<Tour>)cloner.Clone(this.Tours);
      clone.Unrouted = new List<int>(Unrouted);
      return clone;
    }

    public PotvinEncoding() {
      Tours = new ItemList<Tour>();
      Unrouted = new List<int>();
    }
    
    public static PotvinEncoding ConvertFrom(IVRPEncoding encoding) {
      PotvinEncoding solution = new PotvinEncoding();

      solution.Tours.AddRange(
        encoding.Tours);

      return solution;
    }

    public static PotvinEncoding ConvertFrom(List<int> route) {
      PotvinEncoding solution = new PotvinEncoding();

      Tour tour = new Tour();
      for (int i = 0; i < route.Count; i++) {
        if (route[i] == 0) {
          if (tour.Cities.Count > 0) {
            solution.Tours.Add(tour);
            tour = new Tour();
          }
        } else {
          tour.Cities.Add(route[i]);
        }
      }

      return solution;
    }

    public bool FindInsertionPlace(
      DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix,
      int city, int routeToAvoid, out int route, out int place) {
      route = -1;
      place = -1;
      double minDetour = 0;

      for (int tour = 0; tour < Tours.Count; tour++) {
        if (tour != routeToAvoid) {
          for (int i = 0; i <= Tours[tour].Cities.Count; i++) {
            double length = Tours[tour].GetLength(coordinates, distanceMatrix, useDistanceMatrix);

            Tours[tour].Cities.Insert(i, city);

            if (Tours[tour].Feasible(dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, 
              capacity, coordinates, distanceMatrix, useDistanceMatrix)) {
              double newLength = Tours[tour].GetLength(coordinates, distanceMatrix, useDistanceMatrix);

              double detour = newLength - length;

              if (route <= 0 || detour < minDetour) {
                route = tour;
                place = i;
                minDetour = detour;
              }
            }

            Tours[tour].Cities.RemoveAt(i);
          }
        }
      }

      return route >= 0 && place >= 0;
    }
  }
}
