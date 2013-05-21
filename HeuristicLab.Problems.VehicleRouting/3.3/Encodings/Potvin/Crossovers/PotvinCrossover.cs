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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCrossover", "A VRP crossover operation.")]
  [StorableClass]
  public abstract class PotvinCrossover : VRPCrossover, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueParameter<BoolValue> AllowInfeasibleSolutions {
      get { return (IValueParameter<BoolValue>)Parameters["AllowInfeasibleSolutions"]; }
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("AllowInfeasibleSolutions")) {
        Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
      }
      #endregion
    }

    [StorableConstructor]
    protected PotvinCrossover(bool deserializing) : base(deserializing) { }
    protected PotvinCrossover(PotvinCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    public PotvinCrossover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
    }

    protected abstract PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2);

    protected static bool FindInsertionPlace(PotvinEncoding individual, int city, 
      DoubleArray dueTime, DoubleArray serviceTime, DoubleArray readyTime, DoubleArray demand,
      DoubleValue capacity, DistanceMatrix distMatrix, bool allowInfeasible,
      out int route, out int place) {
      return individual.FindInsertionPlace(
        dueTime, serviceTime, readyTime,
        demand, capacity, distMatrix,
        city, -1, allowInfeasible,
        out route, out place);
    }

    protected Tour FindRoute(PotvinEncoding solution, int city) {
      Tour found = null;

      foreach (Tour tour in solution.Tours) {
        if (tour.Cities.Contains(city)) {
          found = tour;
          break;
        }
      }

      return found;
    }

    protected static bool RouteUnrouted(PotvinEncoding solution, DistanceMatrix distMatrix,
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime, DoubleArray demand, DoubleValue capacity, bool allowInfeasible) {
      bool success = true;
      int index = 0;
      while (index < solution.Unrouted.Count && success) {
        int unrouted = solution.Unrouted[index];

        int route, place;
        if (FindInsertionPlace(solution, unrouted,
          dueTime, serviceTime, readyTime, demand, capacity,
          distMatrix, allowInfeasible,
          out route, out place)) {
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

    protected static bool Repair(IRandom random, PotvinEncoding solution, Tour newTour, DistanceMatrix distmatrix,
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime, DoubleArray demand, DoubleValue capacity, 
      bool allowInfeasible) {
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
      while (newTour.Cities.Contains(0))
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

      if (!allowInfeasible && !newTour.Feasible(
        dueTime, serviceTime, readyTime, demand, capacity, distmatrix))
        return false;

      //route unrouted vehicles
      success = RouteUnrouted(solution, distmatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible);

      return success;
    }

    public override IOperation Apply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is PotvinEncoding)) {
          parents[i] = PotvinEncoding.ConvertFrom(solution, DistanceMatrixParameter);
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
