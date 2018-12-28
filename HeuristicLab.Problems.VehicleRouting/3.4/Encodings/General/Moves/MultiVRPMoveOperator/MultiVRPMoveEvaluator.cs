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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("MultiVRPMoveEvaluator", "Evaluates a move for the VRP representation.")]
  [StorableType("CF6F48D1-D488-4266-8F46-68EC1A0A7828")]
  public sealed class MultiVRPMoveEvaluator : VRPMoveEvaluator, IMultiVRPMoveOperator, IGeneralVRPOperator {
    public override ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }

    [StorableConstructor]
    private MultiVRPMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public MultiVRPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The generated moves."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveEvaluator(this, cloner);
    }

    private MultiVRPMoveEvaluator(MultiVRPMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() { }

    public override IOperation InstrumentedApply() {
      IVRPMove move = VRPMoveParameter.ActualValue as IVRPMove;

      VRPMoveEvaluator moveEvaluator = move.GetMoveEvaluator();
      moveEvaluator.VRPMoveParameter.ActualName = VRPMoveParameter.Name;

      OperationCollection next = new OperationCollection(base.InstrumentedApply());
      next.Insert(0, ExecutionContext.CreateOperation(moveEvaluator));

      return next;
    }
  }
}
