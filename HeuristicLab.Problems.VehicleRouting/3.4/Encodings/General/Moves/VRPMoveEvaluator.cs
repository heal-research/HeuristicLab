#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPMoveEvaluator", "Evaluates a VRP move.")]
  [StorableType("2C1B7479-DCD7-41F7-BB65-D1D714313172")]
  public abstract class VRPMoveEvaluator : VRPMoveOperator, ISingleObjectiveMoveEvaluator {
    public const string MovePrefix = "Move";

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<DoubleValue> MovePenaltyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MovePenalty"]; }
    }

    [StorableConstructor]
    protected VRPMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public VRPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("MovePenalty", "The penalty applied to the move."));
    }

    protected VRPMoveEvaluator(VRPMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    //helper method to evaluate an updated individual
    protected void UpdateEvaluation(IVRPEncoding updatedTours) {
      IVRPEvaluator evaluator = ProblemInstance.MoveEvaluator;

      try {
        this.ExecutionContext.Scope.Variables.Add(new Variable(evaluator.VRPToursParameter.ActualName,
          updatedTours));

        IAtomicOperation op = this.ExecutionContext.CreateChildOperation(evaluator);
        op.Operator.Execute((IExecutionContext)op, CancellationToken);
      }
      finally {
        this.ExecutionContext.Scope.Variables.Remove(evaluator.VRPToursParameter.ActualName);
      }
    }

    protected abstract void EvaluateMove();

    public override IOperation InstrumentedApply() {
      EvaluateMove();

      return base.InstrumentedApply();
    }
  }
}
