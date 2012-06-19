#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRDisplacementManipulator", "An operator which manipulates a GVR representation by applying a displacement operation. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableClass]
  public sealed class GVRDisplacementManipulator : GVRManipulator {
    [StorableConstructor]
    private GVRDisplacementManipulator(bool deserializing) : base(deserializing) { }

    public GVRDisplacementManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVRDisplacementManipulator(this, cloner);
    }

    private GVRDisplacementManipulator(GVRDisplacementManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, GVREncoding individual) {
      Tour tour = individual.Tours[random.Next(individual.Tours.Count)];
      int breakPoint1 = random.Next(tour.Stops.Count);
      int length = random.Next(1, tour.Stops.Count - breakPoint1 + 1);

      List<int> displaced = tour.Stops.GetRange(breakPoint1, length);
      tour.Stops.RemoveRange(breakPoint1, length);
      //with a probability of 1/(2*V) create a new tour, else insert at another position
      if (individual.GetTours().Count > 0 &&
        individual.GetTours().Count < ProblemInstance.Vehicles.Value &&
        random.Next(individual.GetTours().Count * 2) == 0) {
        Tour newTour = new Tour();
        newTour.Stops.InsertRange(0, displaced);

        individual.Tours.Add(newTour);
      } else {
        Tour newTour = individual.Tours[random.Next(individual.Tours.Count)];
        int newPosition = newTour.Stops.Count;

        newTour.Stops.InsertRange(newPosition, displaced);
      }

      if (tour.Stops.Count == 0)
        individual.Tours.Remove(tour);
    }
  }
}
