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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinEncoding", "Represents a potvin encoding of VRP solutions. It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public class PotvinEncoding : TourEncoding {
    [Storable]
    public List<int> Unrouted { get; set; }

    [StorableConstructor]
    protected PotvinEncoding(bool deserializing) : base(deserializing) { }
    protected PotvinEncoding(PotvinEncoding original, Cloner cloner)
      : base(original, cloner) {
      Tours = cloner.Clone(original.Tours);
      Unrouted = new List<int>(original.Unrouted);
    }
    public PotvinEncoding()
      : base() {
      Unrouted = new List<int>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinEncoding(this, cloner);
    }

    public static PotvinEncoding ConvertFrom(IVRPEncoding encoding, ILookupParameter<DoubleMatrix> distanceMatrix) {
      PotvinEncoding solution = new PotvinEncoding();

      TourEncoding.ConvertFrom(encoding, solution, distanceMatrix);

      return solution;
    }

    public static PotvinEncoding ConvertFrom(List<int> route) {
      PotvinEncoding solution = new PotvinEncoding();

      TourEncoding.ConvertFrom(route, solution);

      return solution;
    }

    public bool FindInsertionPlace(
      DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DistanceMatrix distMatrix,
      int city, int routeToAvoid, bool allowInfeasible,
      out int route, out int place) {
      route = -1;
      place = -1;
      bool bestFeasible = false;
      double minDetour = double.MaxValue;

      for (int tour = 0; tour < Tours.Count; tour++) {
        if (tour != routeToAvoid) {
          for (int i = 0; i <= Tours[tour].Cities.Count; i++) {
            double length = Tours[tour].GetLength(distMatrix);

            Tours[tour].Cities.Insert(i, city);

            bool feasible = Tours[tour].Feasible(dueTimeArray, serviceTimeArray, readyTimeArray, demandArray,
              capacity, distMatrix);

            if (feasible || allowInfeasible && !bestFeasible) {
              double newLength = Tours[tour].GetLength(distMatrix);
              double detour = newLength - length;

              if (route < 0 || detour < minDetour || feasible && !bestFeasible) {
                route = tour;
                place = i;
                minDetour = detour;

                if (feasible)
                  bestFeasible = true;
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
