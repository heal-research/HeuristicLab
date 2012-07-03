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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDRearrangeMoveMaker", "Peforms the rearrange move on a given PDP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinPDRearrangeMoveMaker : PotvinMoveMaker, IPotvinPDRearrangeMoveOperator, IMoveMaker {
    public ILookupParameter<PotvinPDRearrangeMove> PDRearrangeMoveParameter {
      get { return (ILookupParameter<PotvinPDRearrangeMove>)Parameters["PotvinPDRearrangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDRearrangeMoveParameter; }
    }

    [StorableConstructor]
    protected PotvinPDRearrangeMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinPDRearrangeMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDRearrangeMove>("PotvinPDRearrangeMove", "The moves that should be made."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDRearrangeMoveMaker(this, cloner);
    }

    protected PotvinPDRearrangeMoveMaker(PotvinPDRearrangeMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(PotvinEncoding solution, PotvinPDRearrangeMove move, IVRPProblemInstance problemInstance) {
      Tour tour = solution.Tours[move.Tour];
      int position = tour.Stops.IndexOf(move.City);

      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;
      if (pdp != null) {
        int location = pdp.GetPickupDeliveryLocation(move.City);
        Tour tour2 = solution.Tours.Find(t => t.Stops.Contains(location));
        int position2 = tour2.Stops.IndexOf(location);

        tour.Stops.Remove(move.City);
        tour2.Stops.Remove(location);

        solution.InsertPair(tour, move.City, location, problemInstance, position, position2);
      } else {
        tour.Stops.Remove(move.City);
        int place = solution.FindBestInsertionPlace(tour, move.City, position);
        tour.Stops.Insert(place, move.City);
      }

      solution.Repair();
    }

    protected override void PerformMove() {
      PotvinPDRearrangeMove move = PDRearrangeMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      VRPToursParameter.ActualValue = newSolution;
    }
  }
}
