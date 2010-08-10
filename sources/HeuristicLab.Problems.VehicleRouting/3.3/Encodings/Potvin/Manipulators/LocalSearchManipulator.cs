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
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("LocalSearchManipulator", "The LSM operator which manipulates a Potvin VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class LocalSearchManipulator : PotvinManipulator {
    [StorableConstructor]
    private LocalSearchManipulator(bool deserializing) : base(deserializing) { }

    public LocalSearchManipulator() : base() { }

    private bool FindBetterInsertionPlace(
      PotvinEncoding individual, int tour, int city, int length,
      out int insertionTour, out int insertionPlace) {
      bool insertionFound = false;
      insertionTour = -1;
      insertionPlace = 1;

      List<int> toBeDeleted = individual.Tours[tour].Cities.GetRange(city, length);
      double distance = GetLength(individual.Tours[tour]);
      individual.Tours[tour].Cities.RemoveRange(city, length);
      double removalBenefit = distance - GetLength(individual.Tours[tour]);

      int currentTour = 0;
      while (currentTour < individual.Tours.Count && !insertionFound) {
        int currentCity = 0;
        while (currentCity <= individual.Tours[currentTour].Cities.Count && !insertionFound) {
          distance = GetLength(individual.Tours[currentTour]);
          individual.Tours[currentTour].Cities.InsertRange(currentCity, toBeDeleted);
          if (Feasible(individual.Tours[currentTour])) {
            double lengthIncrease =
              GetLength(individual.Tours[currentTour]) - distance;
            if (removalBenefit > lengthIncrease) {
              insertionTour = currentTour;
              insertionPlace = currentCity;

              insertionFound = true;
            }
          }
          individual.Tours[currentTour].Cities.RemoveRange(currentCity, length);
          
          currentCity++;
        }
        currentTour++;
      }

      individual.Tours[tour].Cities.InsertRange(city, toBeDeleted);

      return insertionFound;
    }

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      //only apply to feasible individuals
      if (Feasible(individual)) {
        bool insertionFound;

        do {
          insertionFound = false;
          int length = 3;
          while (length > 0 && !insertionFound) {
            int tour = 0;
            while (tour < individual.Tours.Count && !insertionFound) {
              int city = 0;
              while (city <= individual.Tours[tour].Cities.Count - length && !insertionFound) {
                int insertionTour, insertionPlace;
                if (FindBetterInsertionPlace(individual, tour, city, length,
                 out insertionTour, out insertionPlace)) {
                  insertionFound = true;

                  List<int> toBeInserted = individual.Tours[tour].Cities.GetRange(city, length);

                  individual.Tours[tour].Cities.RemoveRange(city, length);
                  individual.Tours[insertionTour].Cities.InsertRange(
                    insertionPlace,
                    toBeInserted);
                }
                city++;
              }
              tour++;
            }
            length--;
          }
        } while (insertionFound);
      }
    }
  }
}
