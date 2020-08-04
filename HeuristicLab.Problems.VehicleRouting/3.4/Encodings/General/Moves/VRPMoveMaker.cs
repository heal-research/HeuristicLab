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
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPMoveMaker", "Performs a VRP move.")]
  [StorableType("00082602-A676-48A7-A2D7-66733B972685")]
  public abstract class VRPMoveMaker : VRPMoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<VRPEvaluation> EvaluationResultParameter {
      get { return (ILookupParameter<VRPEvaluation>)Parameters["EvaluationResult"]; }
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
    protected VRPMoveMaker(StorableConstructorFlag _) : base(_) { }

    public VRPMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<VRPEvaluation>("EvaluationResult", "The evaluation of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("MovePenalty", "The penalty applied to the move."));
      Parameters.Add(new LookupParameter<VRPEvaluation>("MoveEvaluationResult", "The move evaluation."));
    }

    protected VRPMoveMaker(VRPMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract void PerformMove();

    private void UpdateMoveEvaluation() {
      EvaluationResultParameter.ActualValue = MoveEvaluationResultParameter.ActualValue;
      QualityParameter.ActualValue = MoveQualityParameter.ActualValue;
    }

    public override IOperation InstrumentedApply() {
      PerformMove();
      UpdateMoveEvaluation();

      return base.InstrumentedApply();
    }
  }
}
