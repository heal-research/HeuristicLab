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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDRearrangeSingleMoveGenerator", "Generates a single rearrange move from a given PDP encoding.")]
  [StorableClass]
  public sealed class PotvinPDRearrangeSingleMoveGenerator : PotvinPDRearrangeMoveGenerator,
    ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDRearrangeSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDRearrangeSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDRearrangeSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinPDRearrangeSingleMoveGenerator(PotvinPDRearrangeSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinPDRearrangeMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      List<int> cities = new List<int>();

      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;
      for (int i = 1; i <= individual.Cities; i++) {
        if (pdp == null || pdp.GetDemand(i) >= 0)
          cities.Add(i);
      }

      if (cities.Count > 0) {
        int city = cities[rand.Next(cities.Count)];
        int tour = individual.Tours.FindIndex(t => t.Stops.Contains(city));
        return new PotvinPDRearrangeMove(city, tour, individual);
      } else {
        return null;
      }
    }

    protected override PotvinPDRearrangeMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDRearrangeMove> result = new List<PotvinPDRearrangeMove>();

      PotvinPDRearrangeMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
