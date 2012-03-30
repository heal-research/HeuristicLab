#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  public abstract class SymbolicRegressionSingleObjectiveEvaluator : SymbolicDataAnalysisSingleObjectiveEvaluator<IRegressionProblemData>, ISymbolicRegressionSingleObjectiveEvaluator {
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    public IFixedValueParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    public bool ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.Value.Value; }
      set { ApplyLinearScalingParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSingleObjectiveEvaluator(SymbolicRegressionSingleObjectiveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicRegressionSingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating.", new BoolValue(true)));
      ApplyLinearScalingParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating.", new BoolValue(false)));
        ApplyLinearScalingParameter.Hidden = true;
      }
    }

    [ThreadStatic]
    private static double[] cache;

    protected static void CalculateWithScaling(IEnumerable<double> targetValues, IEnumerable<double> estimatedValues, IOnlineCalculator calculator, int maxRows) {
      if (cache == null || cache.GetLength(0) < maxRows) {
        cache = new double[maxRows];
      }

      //calculate linear scaling
      //the static methods of the calculator could not be used as it performs a check if the enumerators have an equal amount of elements
      //this is not true if the cache is used
      int i = 0;
      var linearScalingCalculator = new OnlineLinearScalingParameterCalculator();
      var targetValuesEnumerator = targetValues.GetEnumerator();
      var estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      while (targetValuesEnumerator.MoveNext() && estimatedValuesEnumerator.MoveNext()) {
        double target = targetValuesEnumerator.Current;
        double estimated = estimatedValuesEnumerator.Current;
        cache[i] = estimated;
        linearScalingCalculator.Add(estimated, target);
        i++;
      }
      double alpha = linearScalingCalculator.Alpha;
      double beta = linearScalingCalculator.Beta;

      //calculate the quality by using the passed online calculator
      targetValuesEnumerator = targetValues.GetEnumerator();
      i = 0;
      while (targetValuesEnumerator.MoveNext()) {
        double target = targetValuesEnumerator.Current;
        double estimated = cache[i] * beta + alpha;
        calculator.Add(target, estimated);
        i++;
      }
    }
  }
}
