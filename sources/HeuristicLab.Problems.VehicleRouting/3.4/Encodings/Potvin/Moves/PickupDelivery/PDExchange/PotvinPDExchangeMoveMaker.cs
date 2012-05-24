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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeMoveMaker", "Peforms the exchange move on a given PDP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinPDExchangeMoveMaker : PotvinMoveMaker, IPotvinPDExchangeMoveOperator, IMoveMaker {
    public ILookupParameter<PotvinPDExchangeMove> PDExchangeMoveParameter {
      get { return (ILookupParameter<PotvinPDExchangeMove>)Parameters["PotvinPDExchangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDExchangeMoveParameter; }
    }

    [StorableConstructor]
    private PotvinPDExchangeMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinPDExchangeMoveMaker()
      : base() {
        Parameters.Add(new LookupParameter<PotvinPDExchangeMove>("PotvinPDExchangeMove", "The moves that should be made."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDExchangeMoveMaker(this, cloner);
    }

    protected PotvinPDExchangeMoveMaker(PotvinPDExchangeMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(PotvinEncoding solution, PotvinPDExchangeMove move, IVRPProblemInstance problemInstance) {
      if (move.Tour >= solution.Tours.Count)
        solution.Tours.Add(new Tour());
      Tour tour = solution.Tours[move.Tour];

      Tour oldTour = solution.Tours.Find(t => t.Stops.Contains(move.City));
      oldTour.Stops.Remove(move.City);

      if (problemInstance is IPickupAndDeliveryProblemInstance) {
        IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;

        int location = pdp.GetPickupDeliveryLocation(move.City);
        Tour oldTour2 = solution.Tours.Find(t => t.Stops.Contains(location));
        oldTour2.Stops.Remove(location);

        location = pdp.GetPickupDeliveryLocation(move.Replaced);
        oldTour2 = solution.Tours.Find(t => t.Stops.Contains(location));

        oldTour2.Stops.Remove(location);
        tour.Stops.Remove(move.Replaced);

        solution.InsertPair(tour, move.City, pdp.GetPickupDeliveryLocation(move.City), problemInstance);
        solution.InsertPair(oldTour, move.Replaced, pdp.GetPickupDeliveryLocation(move.Replaced), problemInstance);
      } else {
        tour.Stops.Remove(move.Replaced);

        int place = solution.FindBestInsertionPlace(tour, move.City);
        tour.Stops.Insert(place, move.City);

        place = solution.FindBestInsertionPlace(oldTour, move.Replaced);
        oldTour.Stops.Insert(place, move.Replaced);
      }

      solution.Repair();
    }

    protected override void PerformMove() {
      PotvinPDExchangeMove move = PDExchangeMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      VRPToursParameter.ActualValue = newSolution;
    }
  }
}
