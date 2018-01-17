#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PotvinPDShiftSingleMoveGenerator", "Generates a single shift move from a given PDP encoding.")]
  [StorableClass]
  public sealed class PotvinPDShiftSingleMoveGenerator : PotvinPDShiftMoveGenerator,
    ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDShiftSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDShiftSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDShiftSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinPDShiftSingleMoveGenerator(PotvinPDShiftSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinPDShiftMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      List<int> cities = new List<int>();

      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;
      for (int i = 1; i <= individual.Cities; i++) {
        if (pdp == null || pdp.GetDemand(i) >= 0)
          cities.Add(i);
      }

      if (cities.Count >= 1) {
        int city = cities[rand.Next(cities.Count)];
        Tour oldTour = individual.Tours.Find(t => t.Stops.Contains(city));
        int oldTourIndex = individual.Tours.IndexOf(oldTour);

        int max = individual.Tours.Count;
        if (individual.Tours.Count >= problemInstance.Vehicles.Value)
          max = max - 1;

        int newTourIndex = rand.Next(max);
        if (newTourIndex >= oldTourIndex)
          newTourIndex++;

        return new PotvinPDShiftMove(city, oldTourIndex, newTourIndex, individual);
      } else {
        return null;
      }
    }

    protected override PotvinPDShiftMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDShiftMove> result = new List<PotvinPDShiftMove>();

      PotvinPDShiftMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
