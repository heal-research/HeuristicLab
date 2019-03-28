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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentSingleMoveGenerator", "Generates a single vehicle assignment move from a given VRP encoding.")]
  [StorableType("437E0361-CEEE-41E4-BD20-BA40D6BB8DA4")]
  public sealed class PotvinVehicleAssignmentSingleMoveGenerator : PotvinVehicleAssignmentMoveGenerator,
    ISingleMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public override ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["PotvinVehicleAssignmentMove"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinVehicleAssignmentSingleMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinVehicleAssignmentSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinVehicleAssignmentSingleMoveGenerator(PotvinVehicleAssignmentSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinVehicleAssignmentMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      if (individual.Tours.Count > 1) {
        int tour1 = rand.Next(individual.Tours.Count);
        int tour2 = rand.Next(problemInstance.Vehicles.Value);

        while (tour2 == tour1)
          tour2 = rand.Next(problemInstance.Vehicles.Value);

        return new PotvinVehicleAssignmentMove(tour1, tour2, individual);
      } else {
        return null;
      }
    }

    protected override PotvinVehicleAssignmentMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinVehicleAssignmentMove> result = new List<PotvinVehicleAssignmentMove>();

      PotvinVehicleAssignmentMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
