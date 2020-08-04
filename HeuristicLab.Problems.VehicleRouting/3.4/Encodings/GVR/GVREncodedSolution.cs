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

using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVREncodedSolution", "Represents a genetic vehicle routing encoded solution of the VRP. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableType("27A8F267-9865-4AEA-9ECF-88D950D81D74")]
  public class GVREncodedSolution : TourEncodedSolutions {
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

    public GVREncodedSolution(IVRPProblemInstance problemInstance)
      : base(problemInstance) {
    }

    [StorableConstructor]
    protected GVREncodedSolution(StorableConstructorFlag _) : base(_) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVREncodedSolution(this, cloner);
    }

    protected GVREncodedSolution(GVREncodedSolution original, Cloner cloner)
      : base(original, cloner) {
    }

    public static GVREncodedSolution ConvertFrom(IVRPEncodedSolution encoding, IVRPProblemInstance problemInstance) {
      GVREncodedSolution solution = new GVREncodedSolution(problemInstance);

      TourEncodedSolutions.ConvertFrom(encoding, solution, problemInstance);

      return solution;
    }

    public static GVREncodedSolution ConvertFrom(List<int> route, IVRPProblemInstance problemInstance) {
      GVREncodedSolution solution = new GVREncodedSolution(problemInstance);

      TourEncodedSolutions.ConvertFrom(route, solution);

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
