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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeSingleMoveGenerator", "Generates a single exchange move from a given PDP encoding.")]
  [StorableClass]
  public sealed class PotvinPDExchangeSingleMoveGenerator : PotvinPDExchangeMoveGenerator,
    ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDExchangeSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDExchangeSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDExchangeSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinPDExchangeSingleMoveGenerator(PotvinPDExchangeSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinPDExchangeMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      List<int> cities = new List<int>();

      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;
      for (int i = 1; i <= individual.Cities; i++) {
        if (pdp == null || pdp.GetDemand(i) >= 0)
          cities.Add(i);
      }

      if (cities.Count > 1 && individual.Tours.Count > 1) {
        PotvinPDExchangeMove move = null;
        while (cities.Count > 1 && move == null) {
          int city = cities[rand.Next(cities.Count)];
          Tour oldTour = individual.Tours.Find(t => t.Stops.Contains(city));
          int oldTourIndex = individual.Tours.IndexOf(oldTour);

          int max = individual.Tours.Count;
          if (individual.Tours.Count < problemInstance.Vehicles.Value)
            max = max - 1;

          int newTourIndex = rand.Next(max);
          if (newTourIndex >= oldTourIndex)
            newTourIndex++;

          Tour newTour = individual.Tours[newTourIndex];
          List<int> tourCities = new List<int>();
          foreach (int stop in newTour.Stops) {
            if (pdp == null ||
              (pdp.GetDemand(stop) >= 0 &&
              pdp.GetPickupDeliveryLocation(stop) != pdp.GetPickupDeliveryLocation(city) &&
              pdp.GetPickupDeliveryLocation(stop) != city &&
              pdp.GetPickupDeliveryLocation(city) != stop))
              tourCities.Add(stop);
          }

          if (tourCities.Count > 0) {
            int replaced = tourCities[rand.Next(tourCities.Count)];
            move = new PotvinPDExchangeMove(city, oldTourIndex, newTourIndex, replaced, individual);
          }
        }

        return move;
      } else {
        return null;
      }
    }

    protected override PotvinPDExchangeMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDExchangeMove> result = new List<PotvinPDExchangeMove>();

      PotvinPDExchangeMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
