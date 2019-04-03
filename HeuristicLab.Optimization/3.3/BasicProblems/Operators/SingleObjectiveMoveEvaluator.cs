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
  [Item("Single-objective MoveEvaluator", "Evaluates a parameter vector that results from a move.")]
  [StorableType("EE4B1EBA-50BF-40C7-B338-F4A9D9CC554E")]
  internal sealed class SingleObjectiveMoveEvaluator<TEncodedSolution> : SingleSuccessorOperator, ISingleObjectiveEvaluationOperator<TEncodedSolution>, ISingleObjectiveMoveEvaluator, IStochasticOperator, ISingleObjectiveMoveOperator
  where TEncodedSolution : class, IEncodedSolution {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IEncoding<TEncodedSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TEncodedSolution>>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }

    public Func<TEncodedSolution, IRandom, double> EvaluateFunc { get; set; }

    [StorableConstructor]
    private SingleObjectiveMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveMoveEvaluator(SingleObjectiveMoveEvaluator<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveMoveEvaluator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IEncoding<TEncodedSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMoveEvaluator<TEncodedSolution>(this, cloner);
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var encoding = EncodingParameter.ActualValue;
      var individual = ScopeUtil.GetEncodedSolution(ExecutionContext.Scope, encoding);
      MoveQualityParameter.ActualValue = new DoubleValue(EvaluateFunc(individual, random));
      return base.Apply();
    }
  }
}
