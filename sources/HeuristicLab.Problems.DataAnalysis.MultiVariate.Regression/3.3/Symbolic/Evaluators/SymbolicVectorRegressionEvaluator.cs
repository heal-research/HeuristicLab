#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.Regression;
using HeuristicLab.Problems.DataAnalysis.MultiVariate;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Evaluators {
  [StorableClass]
  public abstract class SymbolicVectorRegressionEvaluator : SingleSuccessorOperator, IMultiVariateDataAnalysisEvaluator {
    private const string RandomParameterName = "Random";
    private const string MultiVariateDataAnalysisProblemDataParameterName = "MultiVariateDataAnalysisProblemData";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<MultiVariateDataAnalysisProblemData> MultiVariateDataAnalysisProblemDataParameter {
      get { return (ILookupParameter<MultiVariateDataAnalysisProblemData>)Parameters[MultiVariateDataAnalysisProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }

    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public MultiVariateDataAnalysisProblemData MultiVariateDataAnalysisProblemData {
      get { return MultiVariateDataAnalysisProblemDataParameter.ActualValue; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public DoubleArray LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public DoubleArray UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }
    #endregion

    public SymbolicVectorRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "A random number generator."));
      Parameters.Add(new LookupParameter<MultiVariateDataAnalysisProblemData>(MultiVariateDataAnalysisProblemDataParameterName, "The multi-variate data analysis problem data to use for training."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The tree interpreter that should be used to evaluate the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition to use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the data set partition to use for training."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(UpperEstimationLimitParameterName, "The upper limit for the estimated values for each component."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(LowerEstimationLimitParameterName, "The lower limit for the estimated values for each component."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic vector regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
    }

    public override IOperation Apply() {
      var interpreter = SymbolicExpressionTreeInterpreter;
      var tree = SymbolicExpressionTree;
      var problemData = MultiVariateDataAnalysisProblemData;

      IEnumerable<string> selectedTargetVariables =
        problemData.TargetVariables.CheckedItems
        .Select(x => x.Value.Value);

      // check if there is a vector component for each target variable
      if (selectedTargetVariables.Count() != tree.Root.SubTrees[0].SubTrees.Count)
        throw new ArgumentException("The dimension of the output-vector of the tree doesn't match the number of selected target variables.");
      int start = SamplesStart.Value;
      int end = SamplesEnd.Value;

      IEnumerable<int> rows = GenerateRowsToEvaluate((uint)Random.Next(), RelativeNumberOfEvaluatedSamples.Value, start, end);

      Evaluate(tree, interpreter, problemData, selectedTargetVariables, rows, LowerEstimationLimit, UpperEstimationLimit);

      return base.Apply();
    }

    public abstract void Evaluate(SymbolicExpressionTree tree, ISymbolicExpressionTreeInterpreter interpreter, MultiVariateDataAnalysisProblemData problemData, IEnumerable<string> targetVariables, IEnumerable<int> rows, DoubleArray lowerEstimationBound, DoubleArray upperEstimationBound);

    private static IEnumerable<int> GenerateRowsToEvaluate(uint seed, double relativeAmount, int start, int end) {
      if (end < start) throw new ArgumentException("Start value is larger than end value.");
      int count = (int)((end - start) * relativeAmount);
      if (count == 0) count = 1;
      return RandomEnumerable.SampleRandomNumbers(seed, start, end, count);
    }
  }
}
