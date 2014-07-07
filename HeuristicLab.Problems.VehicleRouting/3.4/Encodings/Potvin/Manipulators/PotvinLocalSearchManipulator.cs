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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinLocalSearchManipulator", "The LSM operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinLocalSearchManipulator : PotvinManipulator, IVRPLocalSearchManipulator {
    public IValueParameter<IntValue> Iterations {
      get { return (IValueParameter<IntValue>)Parameters["Iterations"]; }
    }

    [StorableConstructor]
    private PotvinLocalSearchManipulator(bool deserializing) : base(deserializing) { }

    public PotvinLocalSearchManipulator()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Iterations", "The number of max iterations.", new IntValue(100)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinLocalSearchManipulator(this, cloner);
    }

    private PotvinLocalSearchManipulator(PotvinLocalSearchManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    private static bool FindBetterInsertionPlace(
      PotvinEncoding individual, IVRPProblemInstance instance, int tour, int city, int length,
      out int insertionTour, out int insertionPlace) {
      bool insertionFound = false;
      insertionTour = -1;
      insertionPlace = 1;

      List<int> toBeDeleted = individual.Tours[tour].Stops.GetRange(city, length);
      double distance = individual.GetTourLength(individual.Tours[tour]);
      individual.Tours[tour].Stops.RemoveRange(city, length);
      double removalBenefit = distance - individual.GetTourLength(individual.Tours[tour]);

      int currentTour = 0;
      while (currentTour < individual.Tours.Count && !insertionFound) {
        int currentCity = 0;
        while (currentCity <= individual.Tours[currentTour].Stops.Count && !insertionFound) {
          distance = individual.GetTourLength(individual.Tours[currentTour]);
          individual.Tours[currentTour].Stops.InsertRange(currentCity, toBeDeleted);
          if (instance.TourFeasible(individual.Tours[currentTour], individual)) {
            double lengthIncrease =
              individual.GetTourLength(individual.Tours[currentTour]) - distance;
            if (removalBenefit > lengthIncrease) {
              insertionTour = currentTour;
              insertionPlace = currentCity;

              insertionFound = true;
            }
          }
          individual.Tours[currentTour].Stops.RemoveRange(currentCity, length);

          currentCity++;
        }
        currentTour++;
      }

      individual.Tours[tour].Stops.InsertRange(city, toBeDeleted);

      return insertionFound;
    }

    public static void ApplyManipulation(IRandom random, PotvinEncoding individual, IVRPProblemInstance instance, int maxIterations) {
      //only apply to feasible individuals
      if (instance.Feasible(individual)) {
        bool insertionFound;
        int iterations = 0;

        do {
          insertionFound = false;
          int length = 3;
          while (length > 0 && !insertionFound) {
            int tour = 0;
            while (tour < individual.Tours.Count && !insertionFound) {
              int city = 0;
              while (city <= individual.Tours[tour].Stops.Count - length && !insertionFound) {
                int insertionTour, insertionPlace;
                if (FindBetterInsertionPlace(individual, instance, tour, city, length,
                 out insertionTour, out insertionPlace)) {
                  insertionFound = true;

                  List<int> toBeInserted = individual.Tours[tour].Stops.GetRange(city, length);

                  individual.Tours[tour].Stops.RemoveRange(city, length);
                  individual.Tours[insertionTour].Stops.InsertRange(
                    insertionPlace,
                    toBeInserted);
                }
                city++;
              }
              tour++;
            }
            length--;
          }
          iterations++;
        } while (insertionFound &&
          iterations < maxIterations);

        IList<Tour> toBeRemoved = new List<Tour>();
        foreach (Tour tour in individual.Tours) {
          if (tour.Stops.Count == 0)
            toBeRemoved.Add(tour);
        }

        foreach (Tour tour in toBeRemoved) {
          individual.Tours.Remove(tour);
        }
      }
    }
 

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      ApplyManipulation(random, individual, ProblemInstance, Iterations.Value.Value);     
    }
  }
}
