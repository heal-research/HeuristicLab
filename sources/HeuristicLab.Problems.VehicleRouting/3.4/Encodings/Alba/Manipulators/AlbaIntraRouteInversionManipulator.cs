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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System;
using System.Collections.Generic;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaIntraRouteInversionManipulator", "An operator which applies the SLS operation to a VRP representation. It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaIntraRouteInversionManipulator : AlbaManipulator {   
    [StorableConstructor]
    private AlbaIntraRouteInversionManipulator(bool deserializing) : base(deserializing) { }

    public AlbaIntraRouteInversionManipulator()
      : base() {
     }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaIntraRouteInversionManipulator(this, cloner);
    }

    private AlbaIntraRouteInversionManipulator(AlbaIntraRouteInversionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(AlbaEncoding individual, int index1, int index2) {
      if (index1 != -1 && index2 != -1) {
        int breakPoint1 = index1 + 1;
        int breakPoint2 = index2;
        for (int i = 0; i <= (breakPoint2 - breakPoint1) / 2; i++) {  // invert permutation between breakpoints
          int temp = individual[breakPoint1 + i];
          individual[breakPoint1 + i] = individual[breakPoint2 - i];
          individual[breakPoint2 - i] = temp;
        }
      }
    }

    protected override void Manipulate(IRandom rand, AlbaEncoding individual) {
      int index1 = -1;
      int index2 = -1;

      List<Tour> validTours = new List<Tour>();
      foreach (Tour tour in individual.GetTours()) {
        if (tour.Stops.Count >= 4)
          validTours.Add(tour);
      }

      if (validTours.Count > 0) {
        Tour chosenTour = validTours[rand.Next(validTours.Count)];
        int currentTourStart = -1;
        for (int i = 0; i < individual.Length; i++) {
          if (individual[i] + 1 == chosenTour.Stops[0]) {
            currentTourStart = i;
            break;
          }
        }

        int currentTourEnd = currentTourStart;
        while (currentTourEnd < individual.Length &&
          individual[currentTourEnd] < ProblemInstance.Cities.Value) {
          currentTourEnd++;
        }

        int tourLength = currentTourEnd - currentTourStart;
        int a = rand.Next(tourLength - 3);
        index1 = currentTourStart + a;
        index2 = currentTourStart + rand.Next(a + 2, tourLength - 1);
      }

      Apply(individual, index1, index2);
    }
  }
}
