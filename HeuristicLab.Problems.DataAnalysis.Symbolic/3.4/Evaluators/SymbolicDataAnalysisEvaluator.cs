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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public abstract class SymbolicDataAnalysisEvaluator<T> : SingleSuccessorOperator,
    ISymbolicDataAnalysisEvaluator<T>, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator, IStochasticOperator
  where T : class, IDataAnalysisProblemData {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EvaluationPartitionParameterName = "EvaluationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string ValidRowIndicatorParameterName = "ValidRowIndicator";

    public override bool CanChangeName { get { return false; } }

    #region parameter properties
    ILookupParameter<IRandom> IStochasticOperator.RandomParameter {
      get { return RandomParameter; }
    }

    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<T> ProblemDataParameter {
      get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
    }

    public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[EvaluationPartitionParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    public IValueLookupParameter<StringValue> ValidRowIndicatorParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[ValidRowIndicatorParameterName]; }
    }
    #endregion


    [StorableConstructor]
    protected SymbolicDataAnalysisEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisEvaluator(SymbolicDataAnalysisEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic data analysis solution encoded as a symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntRange>(EvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The upper and lower limit that should be used as cut off value for the output values of symbolic data analysis trees."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating."));
      Parameters.Add(new ValueLookupParameter<StringValue>(ValidRowIndicatorParameterName, "An indicator variable in the data set that specifies which rows should be evaluated (those for which the indicator <> 0) (optional)."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (Parameters.ContainsKey(ApplyLinearScalingParameterName) && !(Parameters[ApplyLinearScalingParameterName] is LookupParameter<BoolValue>))
        Parameters.Remove(ApplyLinearScalingParameterName);
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName))
        Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating."));
      if (!Parameters.ContainsKey(ValidRowIndicatorParameterName))
        Parameters.Add(new ValueLookupParameter<StringValue>(ValidRowIndicatorParameterName, "An indicator variable in the data set that specifies which rows should be evaluated (those for which the indicator <> 0) (optional)."));
    }

    protected IEnumerable<int> GenerateRowsToEvaluate() {
      return GenerateRowsToEvaluate(RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value);
    }

    protected IEnumerable<int> GenerateRowsToEvaluate(double percentageOfRows) {
      IEnumerable<int> rows;
      int samplesStart = EvaluationPartitionParameter.ActualValue.Start;
      int samplesEnd = EvaluationPartitionParameter.ActualValue.End;
      int testPartitionStart = ProblemDataParameter.ActualValue.TestPartition.Start;
      int testPartitionEnd = ProblemDataParameter.ActualValue.TestPartition.End;
      if (samplesEnd < samplesStart) throw new ArgumentException("Start value is larger than end value.");

      if (percentageOfRows.IsAlmost(1.0))
        rows = Enumerable.Range(samplesStart, samplesEnd - samplesStart);
      else {
        int seed = RandomParameter.ActualValue.Next();
        int count = (int)((samplesEnd - samplesStart) * percentageOfRows);
        if (count == 0) count = 1;
        rows = RandomEnumerable.SampleRandomNumbers(seed, samplesStart, samplesEnd, count);
      }

      rows = rows.Where(i => i < testPartitionStart || testPartitionEnd <= i);
      if (ValidRowIndicatorParameter.ActualValue != null) {
        string indicatorVar = ValidRowIndicatorParameter.ActualValue.Value;
        var problemData = ProblemDataParameter.ActualValue;
        var indicatorRow = problemData.Dataset.GetReadOnlyDoubleValues(indicatorVar);
        rows = rows.Where(r => !indicatorRow[r].IsAlmost(0.0));
      }
      return rows;
    }

    [ThreadStatic]
    private static double[] cache;
    protected static void CalculateWithScaling(IEnumerable<double> targetValues, IEnumerable<double> estimatedValues,
      double lowerEstimationLimit, double upperEstimationLimit,
      IOnlineCalculator calculator, int maxRows) {
      if (cache == null || cache.Length < maxRows) {
        cache = new double[maxRows];
      }

      // calculate linear scaling
      int i = 0;
      var linearScalingCalculator = new OnlineLinearScalingParameterCalculator();
      var targetValuesEnumerator = targetValues.GetEnumerator();
      var estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      while (targetValuesEnumerator.MoveNext() & estimatedValuesEnumerator.MoveNext()) {
        double target = targetValuesEnumerator.Current;
        double estimated = estimatedValuesEnumerator.Current;
        cache[i] = estimated;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated))
          linearScalingCalculator.Add(estimated, target);
        i++;
      }
      if (linearScalingCalculator.ErrorState == OnlineCalculatorError.None && (targetValuesEnumerator.MoveNext() || estimatedValuesEnumerator.MoveNext()))
        throw new ArgumentException("Number of elements in target and estimated values enumeration do not match.");

      double alpha = linearScalingCalculator.Alpha;
      double beta = linearScalingCalculator.Beta;
      if (linearScalingCalculator.ErrorState != OnlineCalculatorError.None) {
        alpha = 0.0;
        beta = 1.0;
      }

      //calculate the quality by using the passed online calculator
      targetValuesEnumerator = targetValues.GetEnumerator();
      var scaledBoundedEstimatedValuesEnumerator = Enumerable.Range(0, i).Select(x => cache[x] * beta + alpha)
        .LimitToRange(lowerEstimationLimit, upperEstimationLimit).GetEnumerator();

      while (targetValuesEnumerator.MoveNext() & scaledBoundedEstimatedValuesEnumerator.MoveNext()) {
        calculator.Add(targetValuesEnumerator.Current, scaledBoundedEstimatedValuesEnumerator.Current);
      }
    }
  }
}
