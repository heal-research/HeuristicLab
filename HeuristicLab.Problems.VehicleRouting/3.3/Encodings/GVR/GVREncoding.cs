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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVREncoding", "Represents a potvin encoding of VRP solutions. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableClass]
  public class GVREncoding : TourEncoding {
    [Storable]
    private DoubleValue capacity { get; set; }

    [Storable]
    private DoubleArray demand { get; set; }

    public override List<Tour> GetTours(ILookupParameter<DoubleMatrix> distanceMatrix = null, int maxVehicles = int.MaxValue) {
      List<Tour> tours = new List<Tour>();    

      foreach (Tour tour in base.Tours) {
        Tour newTour = new Tour();
        double currentDemand = 0;

        foreach (int city in tour.Cities) {
          currentDemand += demand[city];

          if (currentDemand > capacity.Value) {
            if(newTour.Cities.Count > 0)
              tours.Add(newTour);

            newTour = new Tour();
            newTour.Cities.Add(city);
            currentDemand = demand[city];
          } else {
            newTour.Cities.Add(city);
          }
        }

        if (newTour.Cities.Count > 0)
          tours.Add(newTour);
      }

      //repair if there are too many vehicles used
      while (tours.Count > maxVehicles) {
        Tour tour = tours[tours.Count - 1];
        tours[tours.Count - 2].Cities.AddRange(tour.Cities);

        tours.Remove(tour);
      } 

      return tours;
    }

    [StorableConstructor]
    protected GVREncoding(bool deserializing) : base(deserializing) { }
    protected GVREncoding(GVREncoding original, Cloner cloner)
      : base(original, cloner) {
      this.capacity = original.capacity;
      this.demand = original.demand;
      this.Tours = cloner.Clone(original.Tours);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVREncoding(this, cloner);
    }
    public GVREncoding(DoubleValue capacity, DoubleArray demand)
      : base() {
        this.capacity = capacity;
        this.demand = demand;
    }

    public static GVREncoding ConvertFrom(IVRPEncoding encoding, DoubleValue capacity, DoubleArray demand, 
      ILookupParameter<DoubleMatrix> distanceMatrix) {
      GVREncoding solution = new GVREncoding(capacity, demand);

      TourEncoding.ConvertFrom(encoding, solution, distanceMatrix);

      return solution;
    }

    public static GVREncoding ConvertFrom(List<int> route, DoubleValue capacity, DoubleArray demand) {
      GVREncoding solution = new GVREncoding(capacity, demand);

      TourEncoding.ConvertFrom(route, solution);

      return solution;
    }

    internal void FindCustomer(int customer, out Tour tour, out int index) {
      tour = null;
      index = -1;

      int currentTour = 0;
      while (tour == null && currentTour < Tours.Count) {
        int currentCity = 0;
        while (tour == null && currentCity < Tours[currentTour].Cities.Count) {
          if (Tours[currentTour].Cities[currentCity] == customer) {
            tour = Tours[currentTour];
            index = currentCity;
          }

          currentCity++;
        }

        currentTour++;
      }
    }
  }
}
