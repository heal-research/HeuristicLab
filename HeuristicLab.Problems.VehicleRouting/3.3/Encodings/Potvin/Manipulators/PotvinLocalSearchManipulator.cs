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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinLocalSearchManipulator", "The LSM operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinLocalSearchManipulator : PotvinManipulator {
    public IValueParameter<IntValue> Iterations {
      get { return (IValueParameter<IntValue>)Parameters["Iterations"]; }
    }

    [StorableConstructor]
    private PotvinLocalSearchManipulator(bool deserializing) : base(deserializing) { }
    private PotvinLocalSearchManipulator(PotvinLocalSearchManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinLocalSearchManipulator(this, cloner);
    }
    public PotvinLocalSearchManipulator()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Iterations", "The number of max iterations.", new IntValue(100)));
    }

    private bool FindBetterInsertionPlace(
      PotvinEncoding individual,  
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime, DoubleArray demand,
      DoubleValue capacity, DistanceMatrix distMatrix,
      int tour, int city, int length,
      out int insertionTour, out int insertionPlace) {
      bool insertionFound = false;
      insertionTour = -1;
      insertionPlace = 1;

      List<int> toBeDeleted = individual.Tours[tour].Cities.GetRange(city, length);
      double distance = individual.Tours[tour].GetLength(distMatrix);
      individual.Tours[tour].Cities.RemoveRange(city, length);
      double removalBenefit = distance - individual.Tours[tour].GetLength(distMatrix);

      int currentTour = 0;
      while (currentTour < individual.Tours.Count && !insertionFound) {
        int currentCity = 0;
        while (currentCity <= individual.Tours[currentTour].Cities.Count && !insertionFound) {
          distance = individual.Tours[currentTour].GetLength(distMatrix);
          individual.Tours[currentTour].Cities.InsertRange(currentCity, toBeDeleted);
          if (individual.Tours[currentTour].Feasible(dueTime, serviceTime, readyTime, demand, capacity, distMatrix)) {
            double lengthIncrease =
              individual.Tours[currentTour].GetLength(distMatrix) - distance;
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
      BoolValue useDistanceMatrix = UseDistanceMatrixParameter.ActualValue;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, DistanceMatrixParameter, useDistanceMatrix);
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;
      
      //only apply to feasible individuals
      bool feasible = true;

      foreach (Tour tour in individual.Tours) {
        if (!tour.Feasible(dueTime, serviceTime, readyTime, demand, capacity, distMatrix)) {
          feasible = false;
          break;
        }
      }

      if (feasible) {
        bool insertionFound;
        int iterations = 0;

        do {
          insertionFound = false;
          int length = 3;
          while (length > 0 && !insertionFound) {
            int tour = 0;
            while (tour < individual.Tours.Count && !insertionFound) {
              int city = 0;
              while (city <= individual.Tours[tour].Cities.Count - length && !insertionFound) {
                int insertionTour, insertionPlace;
                if (FindBetterInsertionPlace(individual, dueTime, readyTime, serviceTime, demand, capacity, distMatrix,
                  tour, city, length,
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
          iterations++;
        } while (insertionFound &&
          iterations < Iterations.Value.Value);

        IList<Tour> toBeRemoved = new List<Tour>();
        foreach (Tour tour in individual.Tours) {
          if (tour.Cities.Count == 0)
            toBeRemoved.Add(tour);
        }

        foreach (Tour tour in toBeRemoved) {
          individual.Tours.Remove(tour);
        }
      }
    }
  }
}
