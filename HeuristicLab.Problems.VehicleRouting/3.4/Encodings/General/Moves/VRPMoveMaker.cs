#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPMoveMaker", "Performs a VRP move.")]
  [StorableClass]
  public abstract class VRPMoveMaker : VRPMoveOperator, IMoveMaker, ISingleObjectiveOperator {
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
    protected VRPMoveMaker(bool deserializing) : base(deserializing) { }

    public VRPMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("MovePenalty", "The penalty applied to the move."));
    }

    protected VRPMoveMaker(VRPMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract void PerformMove();

    private void UpdateMoveEvaluation() {
      IVRPEvaluator evaluator = ProblemInstance.SolutionEvaluator;
      ICollection<IParameter> addedParameters = new List<IParameter>();

      try {
        foreach (IParameter parameter in evaluator.Parameters) {
          if (parameter is ILookupParameter
            && parameter != evaluator.VRPToursParameter
            && parameter != evaluator.ProblemInstanceParameter) {
            ILookupParameter evaluatorParameter = parameter as ILookupParameter;

            string resultName = evaluatorParameter.ActualName;
            if (!this.Parameters.ContainsKey(resultName)) {
              ILookupParameter resultParameter = new LookupParameter<IItem>(resultName);
              resultParameter.ExecutionContext = ExecutionContext;
              this.Parameters.Add(resultParameter);
              addedParameters.Add(resultParameter);
            }

            string moveResultName = VRPMoveEvaluator.MovePrefix + resultName;
            if (!this.Parameters.ContainsKey(moveResultName)) {
              ILookupParameter moveResultParameter = new LookupParameter<IItem>(moveResultName);
              moveResultParameter.ExecutionContext = ExecutionContext;
              this.Parameters.Add(moveResultParameter);
              addedParameters.Add(moveResultParameter);
            }

            ILookupParameter result = Parameters[resultName] as ILookupParameter;
            ILookupParameter moveResult = Parameters[moveResultName] as ILookupParameter;
            result.ActualValue = moveResult.ActualValue;
          }
        }
      } finally {
        foreach (IParameter parameter in addedParameters) {
          this.Parameters.Remove(parameter);
        }
      }
    }

    public override IOperation InstrumentedApply() {
      PerformMove();
      UpdateMoveEvaluation();

      return base.InstrumentedApply();
    }
  }
}
