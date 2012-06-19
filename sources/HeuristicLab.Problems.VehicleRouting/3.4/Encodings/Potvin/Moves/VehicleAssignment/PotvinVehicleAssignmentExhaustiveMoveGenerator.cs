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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentExhaustiveMoveGenerator", "Generates vehicle assignment moves from a given VRP encoding.")]
  [StorableClass]
  public sealed class PotvinVehicleAssignmentExhaustiveMoveGenerator : PotvinVehicleAssignmentMoveGenerator, IExhaustiveMoveGenerator {
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentExhaustiveMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinVehicleAssignmentExhaustiveMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinVehicleAssignmentExhaustiveMoveGenerator()
      : base() {
    }

    private PotvinVehicleAssignmentExhaustiveMoveGenerator(PotvinVehicleAssignmentExhaustiveMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinVehicleAssignmentMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinVehicleAssignmentMove> result = new List<PotvinVehicleAssignmentMove>();

      for (int i = 0; i < individual.Tours.Count; i++) {
        for (int j = i + 1; j < problemInstance.Vehicles.Value; j++) {
          result.Add(new PotvinVehicleAssignmentMove(i, j, individual));
        }

      }

      return result.ToArray();
    }
  }
}
