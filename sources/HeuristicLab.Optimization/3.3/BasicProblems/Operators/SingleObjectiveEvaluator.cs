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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Single-objective Evaluator", "Calls the script's Evaluate method to get the quality value of the parameter vector.")]
  [StorableClass]
  public sealed class SingleObjectiveEvaluator : InstrumentedOperator, ISingleObjectiveEvaluationOperator, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IEncoding> EncodingParameter {
      get { return (ILookupParameter<IEncoding>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public Func<Individual, IRandom, double> EvaluateFunc { get; set; }

    [StorableConstructor]
    private SingleObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    private SingleObjectiveEvaluator(SingleObjectiveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveEvaluator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IEncoding>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new SingleObjectiveEvaluator(this, cloner); }

    public override IOperation InstrumentedApply() {
      var random = RandomParameter.ActualValue;
      var encoding = EncodingParameter.ActualValue;
      var individual = encoding.GetIndividual(ExecutionContext.Scope);
      QualityParameter.ActualValue = new DoubleValue(EvaluateFunc(individual, random));
      return base.InstrumentedApply();
    }
  }
}
