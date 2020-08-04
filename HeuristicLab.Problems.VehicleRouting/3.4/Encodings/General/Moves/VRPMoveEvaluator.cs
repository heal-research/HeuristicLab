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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

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
    public ILookupParameter<VRPEvaluation> MoveEvaluationResultParameter {
      get { return (ILookupParameter<VRPEvaluation>)Parameters["MoveEvaluationResult"]; }
    }

    [StorableConstructor]
    protected VRPMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public VRPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("MovePenalty", "The penalty applied to the move."));
      Parameters.Add(new LookupParameter<VRPEvaluation>("MoveEvaluationResult", "The evaluation result of the move."));
    }

    protected VRPMoveEvaluator(VRPMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    //helper method to evaluate an updated individual
    protected void UpdateEvaluation(IVRPEncodedSolution updatedTours) {
      var evaluation = ProblemInstance.Evaluate(updatedTours);
      MoveEvaluationResultParameter.ActualValue = evaluation;
      MoveQualityParameter.ActualValue = new DoubleValue(evaluation.Quality);
      MovePenaltyParameter.ActualValue = new DoubleValue(evaluation.Penalty);
    }

    protected abstract void EvaluateMove();

    public override IOperation InstrumentedApply() {
      EvaluateMove();

      return base.InstrumentedApply();
    }
  }
}
