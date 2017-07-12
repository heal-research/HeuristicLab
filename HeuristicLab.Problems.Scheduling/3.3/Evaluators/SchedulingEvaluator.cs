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
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {
  [Item("SchedulingEvaluator", "First applies the decoder operator to obtain a schedule from an encoding and then applies the evaluator to obtain a quality.")]
  [StorableClass]
  public class SchedulingEvaluator : InstrumentedOperator, ISchedulingEvaluator, IStochasticOperator {

    public IValueLookupParameter<IScheduleDecoder> ScheduleDecoderParameter {
      get { return (IValueLookupParameter<IScheduleDecoder>)Parameters["ScheduleDecoder"]; }
    }
    ILookupParameter<IScheduleDecoder> ISchedulingEvaluator.ScheduleDecoderParameter {
      get { return ScheduleDecoderParameter; }
    }
    public IValueLookupParameter<IScheduleEvaluator> ScheduleEvaluatorParameter {
      get { return (IValueLookupParameter<IScheduleEvaluator>)Parameters["ScheduleEvaluator"]; }
    }
    ILookupParameter<IScheduleEvaluator> ISchedulingEvaluator.ScheduleEvaluatorParameter {
      get { return ScheduleEvaluatorParameter; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    // ABE: This parameter exists purely, because some IScheduleDecoders are stochastic...
    // ... which could be solved by letting the algorithm parameterize them ...
    // ... but they have to use the same RNG as the evaluator (due to parallel execution)...
    // ... in particular relevant for Island-GA and ALPS (Local- vs GlobalRandom).
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected SchedulingEvaluator(bool deserializing) : base(deserializing) { }
    protected SchedulingEvaluator(SchedulingEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SchedulingEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IScheduleDecoder>("ScheduleDecoder", "The decoding operator that is used to calculate a schedule from the used representation."));
      Parameters.Add(new ValueLookupParameter<IScheduleEvaluator>("ScheduleEvaluator", "The actual schedule evaluation operator."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value aka fitness value of the solution."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      QualityParameter.Hidden = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchedulingEvaluator(this, cloner);
    }
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("Random")) {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      }
      #endregion
    }

    public override IOperation InstrumentedApply() {
      var decoder = ScheduleDecoderParameter.ActualValue;
      var evaluator = ScheduleEvaluatorParameter.ActualValue;
      if (evaluator == null) throw new InvalidOperationException("A ScheduleEvaluator could not be found.");

      var operations = new OperationCollection(base.InstrumentedApply());
      operations.Insert(0, ExecutionContext.CreateChildOperation(evaluator));
      if (decoder != null) // decode before evaluating
        operations.Insert(0, ExecutionContext.CreateChildOperation(decoder));
      return operations;
    }
  }
}
