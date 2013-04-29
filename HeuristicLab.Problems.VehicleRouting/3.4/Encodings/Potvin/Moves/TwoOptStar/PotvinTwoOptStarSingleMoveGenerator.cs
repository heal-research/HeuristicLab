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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarSingleMoveGenerator", "Generates a single two opt star move from a given VRP encoding.")]
  [StorableClass]
  public sealed class PotvinTwoOptStarSingleMoveGenerator : PotvinTwoOptStarMoveGenerator,
    ISingleMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public override ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["PotvinTwoOptStarMove"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinTwoOptStarSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinTwoOptStarSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinTwoOptStarSingleMoveGenerator(PotvinTwoOptStarSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinTwoOptStarMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      int route1Idx = rand.Next(individual.Tours.Count);
      int route2Idx = rand.Next(individual.Tours.Count - 1);
      if (route2Idx >= route1Idx)
        route2Idx++;

      Tour route1 = individual.Tours[route1Idx];
      Tour route2 = individual.Tours[route2Idx];

      int x1 = rand.Next(route1.Stops.Count + 1);
      int x2 = rand.Next(route2.Stops.Count + 1);

      return new PotvinTwoOptStarMove(route1Idx, x1, route2Idx, x2, individual);
    }

    protected override PotvinTwoOptStarMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinTwoOptStarMove> result = new List<PotvinTwoOptStarMove>();

      PotvinTwoOptStarMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
