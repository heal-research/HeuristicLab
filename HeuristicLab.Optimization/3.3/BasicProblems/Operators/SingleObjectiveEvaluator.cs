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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("Single-objective Evaluator", "Calls the script's Evaluate method to get the quality value of the parameter vector.")]
  [StorableType("E8914B68-D0D7-407F-8D58-002FDF2F45CF")]
  public sealed class SingleObjectiveEvaluator<TEncodedSolution> : InstrumentedOperator, ISingleObjectiveEvaluationOperator<TEncodedSolution>, IStochasticOperator
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

    public Func<TEncodedSolution, IRandom, double> EvaluateFunc { get; set; }

    [StorableConstructor]
    private SingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveEvaluator(SingleObjectiveEvaluator<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveEvaluator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IEncoding<TEncodedSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new SingleObjectiveEvaluator<TEncodedSolution>(this, cloner); }

    public override IOperation InstrumentedApply() {
      var random = RandomParameter.ActualValue;
      var encoding = EncodingParameter.ActualValue;
      var solution = ScopeUtil.GetEncodedSolution(ExecutionContext.Scope, encoding);
      QualityParameter.ActualValue = new DoubleValue(EvaluateFunc(solution, random));
      return base.InstrumentedApply();
    }
  }
}
