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
using HeuristicLab.Common;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPairwiseOneLevelExchangeMainpulator", "The 1M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172. It was adapted to the PDP formulation.")]
  [StorableClass]
  public sealed class PotvinPairwiseOneLevelExchangeMainpulator : PotvinManipulator {
    [StorableConstructor]
    private PotvinPairwiseOneLevelExchangeMainpulator(bool deserializing) : base(deserializing) { }

    public PotvinPairwiseOneLevelExchangeMainpulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPairwiseOneLevelExchangeMainpulator(this, cloner);
    }

    private PotvinPairwiseOneLevelExchangeMainpulator(PotvinPairwiseOneLevelExchangeMainpulator original, Cloner cloner)
      : base(original, cloner) {
    }

    public bool PairwiseMove(PotvinEncoding individual, int city, bool allowInfeasible) {
      bool success;

      IPickupAndDeliveryProblemInstance pdp = ProblemInstance as IPickupAndDeliveryProblemInstance;

      if (pdp != null) {
        Tour route1 = individual.Tours.Find(t => t.Stops.Contains(city));
        int i = route1.Stops.IndexOf(city);

        int dest = pdp.GetPickupDeliveryLocation(city);
        Tour destRoute = individual.Tours.Find(t => t.Stops.Contains(dest));
        int j = destRoute.Stops.IndexOf(dest);

        route1.Stops.Remove(city);
        destRoute.Stops.Remove(dest);

        int routeToAvoid = -1;
        if (route1 == destRoute)
          routeToAvoid = individual.Tours.IndexOf(route1);

        int source, target;
        if (ProblemInstance.GetDemand(city) >= 0) {
          source = city;
          target = dest;
        } else {
          source = dest;
          target = city;
        }

        double bestQuality = double.MaxValue;
        int bestTour = -1;
        int bestPositionSource = -1;
        int bestPositionTarget = -1;

        for (int tourIdx = 0; tourIdx < individual.Tours.Count; tourIdx++) {
          if (tourIdx != routeToAvoid) {
            Tour tour = individual.Tours[tourIdx];
            VRPEvaluation eval = ProblemInstance.EvaluateTour(tour, individual);
            individual.InsertPair(tour, source, target, ProblemInstance);
            VRPEvaluation evalNew = ProblemInstance.EvaluateTour(tour, individual);

            double delta = evalNew.Quality - eval.Quality;

            if (delta < bestQuality &&
               (ProblemInstance.Feasible(evalNew) || allowInfeasible)) {
              bestQuality = delta;
              bestTour = tourIdx;
              bestPositionSource = tour.Stops.IndexOf(source);
              bestPositionTarget = tour.Stops.IndexOf(target);
            }

            tour.Stops.Remove(source);
            tour.Stops.Remove(target);
          }
        }

        if (bestTour >= 0) {
          if (bestPositionTarget < bestPositionSource) {
            individual.Tours[bestTour].Stops.Insert(bestPositionTarget, target);
            individual.Tours[bestTour].Stops.Insert(bestPositionSource, source);
          } else {
            individual.Tours[bestTour].Stops.Insert(bestPositionSource, source);
            individual.Tours[bestTour].Stops.Insert(bestPositionTarget, target);
          }

          success = true;
        } else {
          if (j < i) {
            destRoute.Stops.Insert(j, dest);
            route1.Stops.Insert(i, city);
          } else {
            route1.Stops.Insert(i, city);
            destRoute.Stops.Insert(j, dest);
          }

          success = false;
        }
      } else {
        success = false;
      }

      return success;
    }

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;
      IPickupAndDeliveryProblemInstance pdp = ProblemInstance as IPickupAndDeliveryProblemInstance;

      if (pdp != null) {
        int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
        if (selectedIndex >= 0) {
          Tour route1 =
            individual.Tours[selectedIndex];

          int count = route1.Stops.Count;

          if (count > 0) {
            int i = random.Next(0, count);
            int city = route1.Stops[i];

            if (!PairwiseMove(individual, city, allowInfeasible))
              i++;

            count = route1.Stops.Count;
          }
        }
      }
    }
  }
}
