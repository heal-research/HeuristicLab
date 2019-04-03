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

using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Single-objective MoveMaker", "Applies a move.")]
  [StorableType("C0ABF392-C825-4B98-8FB9-5749A9091FD6")]
  internal sealed class SingleObjectiveMoveMaker<TEncodedSolution> : InstrumentedOperator, IMoveMaker, ISingleObjectiveMoveOperator
  where TEncodedSolution : class, IEncodedSolution {
    public ILookupParameter<IEncoding<TEncodedSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TEncodedSolution>>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }

    [StorableConstructor]
    private SingleObjectiveMoveMaker(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveMoveMaker(SingleObjectiveMoveMaker<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveMoveMaker() {
      Parameters.Add(new LookupParameter<IEncoding<TEncodedSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMoveMaker<TEncodedSolution>(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      if (MoveQualityParameter.ActualValue == null) throw new InvalidOperationException("Move has not been evaluated!");

      var encoding = EncodingParameter.ActualValue;
      var solution = ScopeUtil.GetEncodedSolution(ExecutionContext.Scope, encoding);
      ScopeUtil.CopyEncodedSolutionToScope(ExecutionContext.Scope.Parent.Parent, encoding, solution);

      if (QualityParameter.ActualValue == null) QualityParameter.ActualValue = new DoubleValue(MoveQualityParameter.ActualValue.Value);
      else QualityParameter.ActualValue.Value = MoveQualityParameter.ActualValue.Value;

      return base.InstrumentedApply();
    }
  }
}
