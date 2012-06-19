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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMoveEvaluator", "Evaluates a vehicle assignment move for a VRP representation. ")]
  [StorableClass]
  public sealed class PotvinVehicleAssignmentMoveEvaluator : PotvinMoveEvaluator, IPotvinVehicleAssignmentMoveOperator {
    public ILookupParameter<PotvinVehicleAssignmentMove> VehicleAssignmentMoveParameter {
      get { return (ILookupParameter<PotvinVehicleAssignmentMove>)Parameters["PotvinVehicleAssignmentMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return VehicleAssignmentMoveParameter; }
    }

    [StorableConstructor]
    private PotvinVehicleAssignmentMoveEvaluator(bool deserializing) : base(deserializing) { }

    public PotvinVehicleAssignmentMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinVehicleAssignmentMove>("PotvinVehicleAssignmentMove", "The move that should be evaluated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMoveEvaluator(this, cloner);
    }

    private PotvinVehicleAssignmentMoveEvaluator(PotvinVehicleAssignmentMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      PotvinVehicleAssignmentMove move = VehicleAssignmentMoveParameter.ActualValue;

      PotvinEncoding newSolution = VehicleAssignmentMoveParameter.ActualValue.Individual.Clone() as PotvinEncoding;
      PotvinVehicleAssignmentMoveMaker.Apply(newSolution, move, ProblemInstance);

      UpdateEvaluation(newSolution);
    }
  }
}
