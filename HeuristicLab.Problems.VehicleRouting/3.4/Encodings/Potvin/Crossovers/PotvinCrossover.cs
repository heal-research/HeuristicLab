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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCrossover", "A VRP crossover operation.")]
  [StorableClass]
  public abstract class PotvinCrossover : VRPCrossover, IStochasticOperator, IPotvinOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueParameter<BoolValue> AllowInfeasibleSolutions {
      get { return (IValueParameter<BoolValue>)Parameters["AllowInfeasibleSolutions"]; }
    }

    [StorableConstructor]
    protected PotvinCrossover(bool deserializing) : base(deserializing) { }

    public PotvinCrossover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
    }

    protected PotvinCrossover(PotvinCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2);

    protected static bool FindInsertionPlace(PotvinEncoding individual, int city, bool allowInfeasible,
        out int route, out int place) {
      return individual.FindInsertionPlace(
        city, -1, allowInfeasible,
        out route, out place);
    }

    protected static Tour FindRoute(PotvinEncoding solution, int city) {
      Tour found = null;

      foreach (Tour tour in solution.Tours) {
        if (tour.Stops.Contains(city)) {
          found = tour;
          break;
        }
      }

      return found;
    }

    protected static bool RouteUnrouted(PotvinEncoding solution, bool allowInfeasible) {
      bool success = true;
      int index = 0;
      while (index < solution.Unrouted.Count && success) {
        int unrouted = solution.Unrouted[index];

        int route, place;
        if (FindInsertionPlace(solution, unrouted, allowInfeasible,
          out route, out place)) {
          solution.Tours[route].Stops.Insert(place, unrouted);
        } else {
          success = false;
        }

        index++;
      }

      for (int i = 0; i < index; i++)
        solution.Unrouted.RemoveAt(0);

      return success;
    }

    protected static bool Repair(IRandom random, PotvinEncoding solution, Tour newTour, IVRPProblemInstance instance, bool allowInfeasible) {
      bool success = true;

      //remove duplicates from new tour      
      for (int i = 0; i < newTour.Stops.Count; i++) {
        for (int j = 0; j < newTour.Stops.Count; j++) {
          if (newTour.Stops[i] == newTour.Stops[j] && i != j) {
            if (random.NextDouble() < 0.5)
              newTour.Stops[i] = 0;
            else
              newTour.Stops[j] = 0;
          }
        }
      }
      while (newTour.Stops.Contains(0))
        newTour.Stops.Remove(0);

      //remove duplicates from old tours
      for (int i = 0; i < newTour.Stops.Count; i++) {
        foreach (Tour tour in solution.Tours) {
          if (tour != newTour && tour.Stops.Contains(newTour.Stops[i])) {
            tour.Stops.Remove(newTour.Stops[i]);
          }
        }
      }

      //remove empty tours
      List<Tour> toBeDeleted = new List<Tour>();
      foreach (Tour tour in solution.Tours) {
        if (tour.Stops.Count == 0)
          toBeDeleted.Add(tour);
      }
      foreach (Tour tour in toBeDeleted) {
        solution.Tours.Remove(tour);
      }

      if (newTour.Stops.Count > 0 && !allowInfeasible && !instance.TourFeasible(newTour, solution))
        return false;

      //route unrouted vehicles
      success = RouteUnrouted(solution, allowInfeasible);

      return success;
    }

    public override IOperation InstrumentedApply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is PotvinEncoding)) {
          parents[i] = PotvinEncoding.ConvertFrom(solution, ProblemInstance);
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      ChildParameter.ActualValue = Crossover(RandomParameter.ActualValue, parents[0] as PotvinEncoding, parents[1] as PotvinEncoding);
      (ChildParameter.ActualValue as PotvinEncoding).Repair();

      return base.InstrumentedApply();
    }
  }
}
