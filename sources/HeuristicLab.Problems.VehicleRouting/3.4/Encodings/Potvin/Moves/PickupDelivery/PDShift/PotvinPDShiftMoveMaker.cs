#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDShiftMoveMaker", "Peforms the shift move on a given PDP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinPDShiftMoveMaker : PotvinMoveMaker, IPotvinPDShiftMoveOperator, IMoveMaker {
    public ILookupParameter<PotvinPDShiftMove> PDShiftMoveParameter {
      get { return (ILookupParameter<PotvinPDShiftMove>)Parameters["PotvinPDShiftMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDShiftMoveParameter; }
    }

    [StorableConstructor]
    protected PotvinPDShiftMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinPDShiftMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDShiftMove>("PotvinPDShiftMove", "The moves that should be made."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDShiftMoveMaker(this, cloner);
    }

    protected PotvinPDShiftMoveMaker(PotvinPDShiftMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(PotvinEncoding solution, PotvinPDShiftMove move, IVRPProblemInstance problemInstance) {
      bool newTour = false;

      if (move.Tour >= solution.Tours.Count) {
        solution.Tours.Add(new Tour());
        newTour = true;
      }
      Tour tour = solution.Tours[move.Tour];

      Tour oldTour = solution.Tours.Find(t => t.Stops.Contains(move.City));
      oldTour.Stops.Remove(move.City);

      if (problemInstance is IPickupAndDeliveryProblemInstance) {
        IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;

        int location = pdp.GetPickupDeliveryLocation(move.City);
        Tour oldTour2 = solution.Tours.Find(t => t.Stops.Contains(location));
        oldTour2.Stops.Remove(location);

        solution.InsertPair(tour, move.City, location, problemInstance);
      } else {
        int place = solution.FindBestInsertionPlace(tour, move.City);
        tour.Stops.Insert(place, move.City);
      }

      if (newTour) {
        List<int> vehicles = new List<int>();
        for (int i = move.Tour; i < problemInstance.Vehicles.Value; i++) {
          vehicles.Add(solution.GetVehicleAssignment(i));
        }

        double bestQuality = double.MaxValue;
        int bestVehicle = -1;

        int originalVehicle = solution.GetVehicleAssignment(move.Tour);
        foreach (int vehicle in vehicles) {
          solution.VehicleAssignment[move.Tour] = vehicle;

          double quality = problemInstance.EvaluateTour(tour, solution).Quality;
          if (quality < bestQuality) {
            bestQuality = quality;
            bestVehicle = vehicle;
          }
        }

        solution.VehicleAssignment[move.Tour] = originalVehicle;

        int index = -1;
        for (int i = move.Tour; i < solution.VehicleAssignment.Length; i++) {
          if (solution.VehicleAssignment[i] == bestVehicle) {
            index = i;
            break;
          }
        }
        solution.VehicleAssignment[index] = originalVehicle;
        solution.VehicleAssignment[move.Tour] = bestVehicle;
      }

      solution.Repair();
    }

    protected override void PerformMove() {
      PotvinPDShiftMove move = PDShiftMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      VRPToursParameter.ActualValue = newSolution;
    }
  }
}
