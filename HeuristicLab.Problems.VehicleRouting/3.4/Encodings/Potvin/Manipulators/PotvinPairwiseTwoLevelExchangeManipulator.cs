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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPairwiseTwoLevelExchangeManipulator", "The 2M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.  It was adapted to the PDP formulation.")]
  [StorableClass]
  public sealed class PotvinPairwiseTwoLevelExchangeManipulator : PotvinManipulator {
    [StorableConstructor]
    private PotvinPairwiseTwoLevelExchangeManipulator(bool deserializing) : base(deserializing) { }

    public PotvinPairwiseTwoLevelExchangeManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPairwiseTwoLevelExchangeManipulator(this, cloner);
    }

    private PotvinPairwiseTwoLevelExchangeManipulator(PotvinPairwiseTwoLevelExchangeManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    private PotvinEncoding ReplacePair(PotvinEncoding individual, int replaced, int replacing, bool allowInfeasible) {
      individual = individual.Clone() as PotvinEncoding;
      IPickupAndDeliveryProblemInstance pdp = ProblemInstance as IPickupAndDeliveryProblemInstance;

      int replacedDest = pdp.GetPickupDeliveryLocation(replaced);
      int replacedSource, replacedTarget;
      if (pdp.GetDemand(replaced) >= 0) {
        replacedSource = replaced;
        replacedTarget = replacedDest;
      } else {
        replacedSource = replacedDest;
        replacedTarget = replaced;
      }
      Tour replacedSourceTour = individual.Tours.Find(t => t.Stops.Contains(replacedSource));
      Tour replacedTargetTour = individual.Tours.Find(t => t.Stops.Contains(replacedTarget));

      int replacingDest = pdp.GetPickupDeliveryLocation(replacing);
      int replacingSource, replacingTarget;
      if (pdp.GetDemand(replacing) >= 0) {
        replacingSource = replacing;
        replacingTarget = replacingDest;
      } else {
        replacingSource = replacingDest;
        replacingTarget = replacing;
      }
      Tour replacingSourceTour = individual.Tours.Find(t => t.Stops.Contains(replacingSource));
      Tour replacingTargetTour = individual.Tours.Find(t => t.Stops.Contains(replacingTarget));

      replacingSourceTour.Stops.Remove(replacingSource);
      replacingTargetTour.Stops.Remove(replacingTarget);

      replacedSourceTour.Stops[replacedSourceTour.Stops.IndexOf(replacedSource)] = replacingSource;
      if (!allowInfeasible && !ProblemInstance.TourFeasible(replacedSourceTour, individual))
        return null;

      replacedTargetTour.Stops[replacedTargetTour.Stops.IndexOf(replacedTarget)] = replacingTarget;
      if (!allowInfeasible && !ProblemInstance.TourFeasible(replacedTargetTour, individual))
        return null;

      double bestQuality = double.MaxValue;
      int bestTour = -1;
      int bestPositionSource = -1;
      int bestPositionTarget = -1;

      int routeToAvoid = individual.Tours.IndexOf(replacingSourceTour);

      for (int tourIdx = 0; tourIdx < individual.Tours.Count; tourIdx++) {
        if (tourIdx != routeToAvoid) {
          Tour tour = individual.Tours[tourIdx];
          VRPEvaluation eval = ProblemInstance.EvaluateTour(tour, individual);
          individual.InsertPair(tour, replacedSource, replacedTarget, ProblemInstance);
          VRPEvaluation evalNew = ProblemInstance.EvaluateTour(tour, individual);

          double delta = evalNew.Quality - eval.Quality;

          if (delta < bestQuality &&
              (ProblemInstance.Feasible(evalNew) || allowInfeasible)) {
            bestQuality = delta;
            bestTour = tourIdx;
            bestPositionSource = tour.Stops.IndexOf(replacedSource);
            bestPositionTarget = tour.Stops.IndexOf(replacedTarget);
          }

          tour.Stops.Remove(replacedSource);
          tour.Stops.Remove(replacedTarget);
        }
      }

      if (bestTour != -1) {
        if (bestPositionTarget < bestPositionSource) {
          individual.Tours[bestTour].Stops.Insert(bestPositionTarget, replacedTarget);
          individual.Tours[bestTour].Stops.Insert(bestPositionSource, replacedSource);
        } else {
          individual.Tours[bestTour].Stops.Insert(bestPositionSource, replacedSource);
          individual.Tours[bestTour].Stops.Insert(bestPositionTarget, replacedTarget);
        }

        return individual;
      } else {
        return null;
      }
    }

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;
      IPickupAndDeliveryProblemInstance pdp = ProblemInstance as IPickupAndDeliveryProblemInstance;

      if (pdp != null) {
        int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
        if (selectedIndex >= 0) {
          bool performed = false;
          Tour route1 = individual.Tours[selectedIndex];

          if (route1.Stops.Count > 0) {
            //randomize customer selection
            Permutation perm = new Permutation(PermutationTypes.Absolute, route1.Stops.Count, random);
            int customer1Position = 0;

            while (customer1Position < route1.Stops.Count) {
              performed = false;

              int customer1 = route1.Stops[perm[customer1Position]];
              int customer2 = -1;

              for (int i = 0; i < individual.Tours.Count; i++) {
                if (i != selectedIndex) {
                  Tour tour = individual.Tours[i];
                  for (int customer2Position = 0; customer2Position < tour.Stops.Count; customer2Position++) {
                    customer2 = tour.Stops[customer2Position];

                    if (pdp.GetPickupDeliveryLocation(customer1) != customer2) {
                      PotvinEncoding result = ReplacePair(individual, customer2, customer1, allowInfeasible);
                      if (result != null) {
                        VRPToursParameter.ActualValue = result;
                        individual = result;

                        route1 = individual.Tours[selectedIndex];
                        performed = true;
                        break;
                      }
                    }
                  }
                }

                if (performed) {
                  break;
                }
              }

              if (!performed)
                customer1Position++;
              else
                break;
            }
          }
        }
      }
    }
  }
}
