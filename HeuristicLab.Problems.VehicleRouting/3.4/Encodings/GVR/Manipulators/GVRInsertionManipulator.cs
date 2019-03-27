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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRInsertionManipulator", "An operator which manipulates a GVR representation by inserting a customer at another location. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableType("2DDACE2B-562A-40A5-86A4-B0BE34993869")]
  public sealed class GVRInsertionManipulator : GVRManipulator {
    [StorableConstructor]
    private GVRInsertionManipulator(StorableConstructorFlag _) : base(_) { }

    public GVRInsertionManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVRInsertionManipulator(this, cloner);
    }

    private GVRInsertionManipulator(GVRInsertionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, GVREncoding individual) {
      int customer = random.Next(1, individual.Cities + 1);
      Tour tour;
      int position;
      individual.FindCustomer(customer, out tour, out position);

      tour.Stops.RemoveAt(position);

      //with a probability of 1/(2*V) create a new tour, else insert at another position
      if (individual.GetTours().Count > 0 &&
        individual.GetTours().Count < ProblemInstance.Vehicles.Value &&
        random.Next(individual.GetTours().Count * 2) == 0) {
        Tour newTour = new Tour();
        newTour.Stops.Add(customer);

        individual.Tours.Add(newTour);
      } else {
        Tour newTour = individual.Tours[random.Next(individual.Tours.Count)];
        int newPosition = random.Next(newTour.Stops.Count + 1);

        newTour.Stops.Insert(newPosition, customer);
      }

      if (tour.Stops.Count == 0)
        individual.Tours.Remove(tour);
    }
  }
}
