#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinEncoding", "Represents a potvin encoding of VRP solutions. It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public class PotvinEncoding : TourEncoding {
    [Storable]
    public List<int> Unrouted { get; set; }

    [Storable]
    public Permutation VehicleAssignment { get; private set; }

    public PotvinEncoding(IVRPProblemInstance instance)
      : base(instance) {
      Unrouted = new List<int>();
      VehicleAssignment = new Permutation(PermutationTypes.Absolute, instance.Vehicles.Value);
    }

    [StorableConstructor]
    protected PotvinEncoding(bool serializing)
      : base(serializing) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinEncoding(this, cloner);
    }

    protected PotvinEncoding(PotvinEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.Unrouted = new List<int>(original.Unrouted);
      this.VehicleAssignment = cloner.Clone<Permutation>(original.VehicleAssignment);
    }

    public override void Repair() {
      List<Tour> toBeRemoved = new List<Tour>();
      foreach (Tour tour in Tours) {
        if (tour.Stops.Count == 0)
          toBeRemoved.Add(tour);
      }

      foreach (Tour tour in toBeRemoved) {
        int index = Tours.IndexOf(tour);
        Tours.Remove(tour);
        if (index < VehicleAssignment.Length) {
          int vehicle = VehicleAssignment[index];
          int max = System.Math.Min(VehicleAssignment.Length - 1, Tours.Count);

          for (int i = index; i < max; i++) {
            VehicleAssignment[i] = VehicleAssignment[i + 1];
          }
          VehicleAssignment[max] = vehicle;
        }
      }

      while (Tours.Count > ProblemInstance.Vehicles.Value) {
        Tour tour = Tours[Tours.Count - 1];
        Tours[Tours.Count - 2].Stops.AddRange(tour.Stops);

        Tours.Remove(tour);
      }
    }

    public override int GetVehicleAssignment(int tour) {
      return VehicleAssignment[tour];
    }

    public static PotvinEncoding ConvertFrom(IVRPEncoding encoding, IVRPProblemInstance instance) {
      PotvinEncoding solution = new PotvinEncoding(instance);

      TourEncoding.ConvertFrom(encoding, solution, instance);

      List<int> vehicles = new List<int>();
      for (int i = 0; i < instance.Vehicles.Value; i++)
        vehicles.Add(i);

      int[] assignment = new int[instance.Vehicles.Value];
      for (int i = 0; i < assignment.Length; i++)
        assignment[i] = -1;

      for (int i = 0; i < solution.Tours.Count; i++) {
        int vehicle = encoding.GetVehicleAssignment(i);
        assignment[i] = vehicle;
        vehicles.Remove(vehicle);
      }

      for (int i = 0; i < instance.Vehicles.Value; i++) {
        if (assignment[i] == -1) {
          int vehicle = vehicles[0];
          assignment[i] = vehicle;
          vehicles.RemoveAt(0);
        }
      }

      solution.VehicleAssignment = new Permutation(PermutationTypes.Absolute, assignment);

      return solution;
    }

    public static PotvinEncoding ConvertFrom(List<int> route, IVRPProblemInstance instance) {
      PotvinEncoding solution = new PotvinEncoding(instance);

      solution.Tours = new ItemList<Tour>();

      Tour tour = new Tour();
      int routeIdx = 0;
      for (int i = 0; i < route.Count; i++) {
        if (route[i] <= 0) {
          if (tour.Stops.Count > 0) {
            solution.Tours.Add(tour);
            tour = new Tour();
          }
          int vehicle = -route[i];
          solution.VehicleAssignment[routeIdx] = vehicle;
          routeIdx++;
        } else {
          tour.Stops.Add(route[i]);
        }
      }

      solution.Repair();

      return solution;
    }

    public double GetTourLength(Tour tour) {
      return tour.GetTourLength(ProblemInstance, this);
    }

    public int FindBestInsertionPlace(Tour tour, int city, int positionToAvoid = -1) {
      if (tour.Stops.Count == 0)
        return 0;

      int place = -1;
      double minQuality = -1;

      VRPEvaluation eval = ProblemInstance.EvaluateTour(tour, this);

      for (int i = 0; i <= tour.Stops.Count; i++) {
        if (positionToAvoid != i) {
          bool feasible;
          double quality = ProblemInstance.GetInsertionCosts(eval, this, city, 0, i, out feasible);
          if (place < 0 || quality < minQuality) {
            place = i;
            minQuality = quality;
          }
        }
      }

      if (place == -1)
        place = 0;

      return place;
    }

    public void InsertPair(Tour tour, int source, int target, IVRPProblemInstance problemInstance, int positionToAvoid = -1, int positionToAvoid2 = -1) {
      int stops = tour.Stops.Count;
      VRPEvaluation eval = problemInstance.EvaluateTour(tour, this);
      double minCosts = double.MaxValue;
      int sourceLocation = -1;
      int targetLocation = -1;

      for (int i = 0; i <= stops; i++) {
        tour.Stops.Insert(i, source);
        VRPEvaluation tourEval = problemInstance.EvaluateTour(tour, this);
        double sourceCosts = tourEval.Quality - eval.Quality;

        for (int j = i + 1; j <= stops + 1; j++) {
          if (positionToAvoid != i || positionToAvoid2 != j || stops == 0) {
            bool feasible;
            double targetCosts = problemInstance.GetInsertionCosts(tourEval, this, target, 0, j, out feasible);

            double costs = sourceCosts + targetCosts;
            if (costs < minCosts) {
              minCosts = costs;
              sourceLocation = i;
              targetLocation = j;
            }
          }
        }
        tour.Stops.Remove(source);
      }

      tour.Stops.Insert(sourceLocation, source);
      tour.Stops.Insert(targetLocation, target);
    }

    public bool FindInsertionPlace(int city, int routeToAvoid, bool allowInfeasible, out int route, out int place) {
      route = -1;
      place = -1;
      double minDetour = double.MaxValue;

      VRPEvaluation eval = ProblemInstance.Evaluate(this);
      bool originalFeasible = ProblemInstance.Feasible(eval);

      for (int tour = 0; tour < Tours.Count; tour++) {
        if (tour != routeToAvoid) {
          double length = eval.Quality;

          for (int i = 0; i <= Tours[tour].Stops.Count; i++) {
            bool feasible;
            double detour = ProblemInstance.GetInsertionCosts(eval, this, city, tour, i, out feasible);

            if (feasible || allowInfeasible) {
              if (route < 0 || detour < minDetour) {
                route = tour;
                place = i;
                minDetour = detour;
              }
            }
          }
        }
      }

      return route >= 0 && place >= 0;
    }
  }
}
