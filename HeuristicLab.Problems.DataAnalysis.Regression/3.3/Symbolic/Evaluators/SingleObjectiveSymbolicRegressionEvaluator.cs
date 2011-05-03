#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SingleObjectiveSymbolicRegressionEvaluator", "Evaluates a symbolic regression solution.")]
  [StorableClass]
  public abstract class SingleObjectiveSymbolicRegressionEvaluator : SingleSuccessorOperator, ISymbolicRegressionEvaluator {
    private const string RandomParameterName = "Random";
    private const string QualityParameterName = "Quality";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string FunctionTreeParameterName = "FunctionTree";
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    #region ISymbolicRegressionEvaluator Members

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }

    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }

    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[FunctionTreeParameterName]; }
    }

    public ILookupParameter<DataAnalysisProblemData> RegressionProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[RegressionProblemDataParameterName]; }
    }

    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }

    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }


    #endregion
    #region properties
    public abstract bool Maximization { get; }

    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DataAnalysisProblemData RegressionProblemData {
      get { return RegressionProblemDataParameter.ActualValue; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected SingleObjectiveSymbolicRegressionEvaluator(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveSymbolicRegressionEvaluator(SingleObjectiveSymbolicRegressionEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SingleObjectiveSymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the evaluated symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(FunctionTreeParameterName, "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(RegressionProblemDataParameterName, "The problem data on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The start index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The end index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
    }


    [StorableHook(Persistence.Default.CompositeSerializers.Storable.HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(RelativeNumberOfEvaluatedSamplesParameterName))
        Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
      if (!Parameters.ContainsKey(RandomParameterName))
        Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
    }

    public override IOperation Apply() {
      int seed = Random.Next();
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;
      IEnumerable<int> rows = GenerateRowsToEvaluate(seed, RelativeNumberOfEvaluatedSamples.Value, SamplesStart.Value, SamplesEnd.Value)
          .Where(i => i < RegressionProblemData.TestSamplesStart.Value || RegressionProblemData.TestSamplesEnd.Value <= i);
      double quality = Evaluate(SymbolicExpressionTreeInterpreter, SymbolicExpressionTree, lowerEstimationLimit, upperEstimationLimit,
        RegressionProblemData.Dataset,
        RegressionProblemData.TargetVariable.Value, rows);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.Apply();
    }


    internal static IEnumerable<int> GenerateRowsToEvaluate(int seed, double relativeAmount, int start, int end) {
      if (end < start) throw new ArgumentException("Start value is larger than end value.");
      int count = (int)((end - start) * relativeAmount);
      if (count == 0) count = 1;
      return RandomEnumerable.SampleRandomNumbers(seed, start, end, count);
    }

    public abstract double Evaluate(ISymbolicExpressionTreeInterpreter interpreter,
      SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit,
      Dataset dataset,
      string targetVariable,
      IEnumerable<int> rows);
  }
}
