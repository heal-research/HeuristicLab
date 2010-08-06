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
using HeuristicLab.Optimization;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [StorableClass]
  public abstract class PotvinCrossover : VRPCrossover, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public PotvinCrossover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected abstract PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2);

    protected bool FindInsertionPlace(PotvinEncoding individual, int city, out int route, out int place) {
      return individual.FindInsertionPlace(
        DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue, CapacityParameter.ActualValue, CoordinatesParameter.ActualValue,
        DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue,
        city, -1, out route, out place);
    }

    protected bool Repair(IRandom random, PotvinEncoding solution, Tour newTour) {
      bool success = true;
      
      //remove duplicates from new tour      
      for (int i = 0; i < newTour.Cities.Count; i++) {
        for (int j = 0; j < newTour.Cities.Count; j++) {
          if (newTour.Cities[i] == newTour.Cities[j] && i != j) {
            if (random.NextDouble() < 0.5)
              newTour.Cities[i] = 0;
            else
              newTour.Cities[j] = 0;
          }
        }
      }
      while(newTour.Cities.Contains(0))
        newTour.Cities.Remove(0);

      //remove duplicates from old tours
      for (int i = 0; i < newTour.Cities.Count; i++) {
        foreach (Tour tour in solution.Tours) {
          if (tour != newTour && tour.Cities.Contains(newTour.Cities[i])) {
            tour.Cities.Remove(newTour.Cities[i]);
          }
        }
      }

      //remove empty tours
      List<Tour> toBeDeleted = new List<Tour>();
      foreach (Tour tour in solution.Tours) {
        if (tour.Cities.Count == 0)
          toBeDeleted.Add(tour);
      }
      foreach (Tour tour in toBeDeleted) {
        solution.Tours.Remove(tour);
      }

      //route unrouted vehicles
      int index = 0;
      while (index < solution.Unrouted.Count && success) {
        int unrouted = solution.Unrouted[index];

        int route, place;
        if(FindInsertionPlace(solution, unrouted, out route, out place)) {
          solution.Tours[route].Cities.Insert(place, unrouted);
        } else {
          success = false;
        }

        index++;
      }

      for (int i = 0; i < index; i++)
        solution.Unrouted.RemoveAt(0);

      return success;
    }

    public override IOperation Apply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is PotvinEncoding)) {
          parents[i] = PotvinEncoding.ConvertFrom(solution);
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      ChildParameter.ActualValue = Crossover(RandomParameter.ActualValue, parents[0] as PotvinEncoding, parents[1] as PotvinEncoding);

      return base.Apply();
    }
  }
}
