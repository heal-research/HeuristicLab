#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVREncoding", "Represents a potvin encoding of VRP solutions. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableClass]
  public class GVREncoding : TourEncoding {
    public override List<Tour> GetTours() {
      List<Tour> tours = new List<Tour>();

      foreach (Tour tour in base.Tours) {
        Tour newTour = new Tour();
        double currentDemand = 0;

        DoubleValue capacity = new DoubleValue(double.MaxValue);
        if (ProblemInstance is IHomogenousCapacitatedProblemInstance) {
          capacity.Value = (ProblemInstance as IHomogenousCapacitatedProblemInstance).Capacity.Value;
        }


        foreach (int city in tour.Stops) {
          currentDemand += ProblemInstance.GetDemand(city);

          if (currentDemand > capacity.Value) {
            if (newTour.Stops.Count > 0)
              tours.Add(newTour);

            newTour = new Tour();
            newTour.Stops.Add(city);
            currentDemand = ProblemInstance.GetDemand(city);
          } else {
            newTour.Stops.Add(city);
          }
        }

        if (newTour.Stops.Count > 0)
          tours.Add(newTour);
      }

      //repair if there are too many vehicles used
      while (tours.Count > ProblemInstance.Vehicles.Value) {
        Tour tour = tours[tours.Count - 1];
        tours[tours.Count - 2].Stops.AddRange(tour.Stops);

        tours.Remove(tour);
      }

      return tours;
    }

    public GVREncoding(IVRPProblemInstance problemInstance)
      : base(problemInstance) {
    }

    [StorableConstructor]
    protected GVREncoding(bool serializing)
      : base(serializing) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVREncoding(this, cloner);
    }

    protected GVREncoding(GVREncoding original, Cloner cloner)
      : base(original, cloner) {
    }

    public static GVREncoding ConvertFrom(IVRPEncoding encoding, IVRPProblemInstance problemInstance) {
      GVREncoding solution = new GVREncoding(problemInstance);

      TourEncoding.ConvertFrom(encoding, solution, problemInstance);

      return solution;
    }

    public static GVREncoding ConvertFrom(List<int> route, IVRPProblemInstance problemInstance) {
      GVREncoding solution = new GVREncoding(problemInstance);

      TourEncoding.ConvertFrom(route, solution);

      return solution;
    }

    internal void FindCustomer(int customer, out Tour tour, out int index) {
      tour = null;
      index = -1;

      int currentTour = 0;
      while (tour == null && currentTour < Tours.Count) {
        int currentCity = 0;
        while (tour == null && currentCity < Tours[currentTour].Stops.Count) {
          if (Tours[currentTour].Stops[currentCity] == customer) {
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
