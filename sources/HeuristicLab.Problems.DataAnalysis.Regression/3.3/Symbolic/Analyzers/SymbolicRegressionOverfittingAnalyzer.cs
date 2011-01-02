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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  [Item("SymbolicRegressionOverfittingAnalyzer", "Calculates and tracks correlation of training and validation fitness of symbolic regression models.")]
  [StorableClass]
  public sealed class SymbolicRegressionOverfittingAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaximizationParameterName = "Maximization";
    private const string QualityParameterName = "Quality";
    private const string ValidationQualityParameterName = "ValidationQuality";
    private const string TrainingValidationCorrelationParameterName = "TrainingValidationCorrelation";
    private const string TrainingValidationCorrelationTableParameterName = "TrainingValidationCorrelationTable";
    private const string LowerCorrelationThresholdParameterName = "LowerCorrelationThreshold";
    private const string UpperCorrelationThresholdParameterName = "UpperCorrelationThreshold";
    private const string OverfittingParameterName = "IsOverfitting";
    private const string ResultsParameterName = "Results";
    private const string EvaluatorParameterName = "Evaluator";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ValidationSamplesStartParameterName = "ValidationSamplesStart";
    private const string ValidationSamplesEndParameterName = "ValidationSamplesEnd";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> ValidationQualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[ValidationQualityParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<ISymbolicRegressionEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicRegressionEvaluator>)Parameters[EvaluatorParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesEndParameterName]; }
    }
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public ILookupParameter<DoubleValue> TrainingValidationQualityCorrelationParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TrainingValidationCorrelationParameterName]; }
    }
    public ILookupParameter<DataTable> TrainingValidationQualityCorrelationTableParameter {
      get { return (ILookupParameter<DataTable>)Parameters[TrainingValidationCorrelationTableParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerCorrelationThresholdParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerCorrelationThresholdParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperCorrelationThresholdParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperCorrelationThresholdParameterName]; }
    }
    public ILookupParameter<BoolValue> OverfittingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[OverfittingParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public ISymbolicRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.ActualValue; }
    }
    public DataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public IntValue ValidiationSamplesStart {
      get { return ValidationSamplesStartParameter.ActualValue; }
    }
    public IntValue ValidationSamplesEnd {
      get { return ValidationSamplesEndParameter.ActualValue; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }

    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionOverfittingAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionOverfittingAnalyzer(SymbolicRegressionOverfittingAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionOverfittingAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use.")); 
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "Training fitness"));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));

      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<ISymbolicRegressionEvaluator>(EvaluatorParameterName, "The evaluator which should be used to evaluate the solution on the validation set."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));

      Parameters.Add(new LookupParameter<DoubleValue>(TrainingValidationCorrelationParameterName, "Correlation of training and validation fitnesses"));
      Parameters.Add(new LookupParameter<DataTable>(TrainingValidationCorrelationTableParameterName, "Data table of training and validation fitness correlation values over the whole run."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerCorrelationThresholdParameterName, "Lower threshold for correlation value that marks the boundary from non-overfitting to overfitting.", new DoubleValue(0.65)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperCorrelationThresholdParameterName, "Upper threshold for correlation value that marks the boundary from overfitting to non-overfitting.", new DoubleValue(0.75)));
      Parameters.Add(new LookupParameter<BoolValue>(OverfittingParameterName, "Boolean indicator for overfitting."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The results collection."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionOverfittingAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      double[] trainingArr = qualities.Select(x => x.Value).ToArray();
      double[] validationArr = new double[trainingArr.Length];

      #region calculate validation fitness
      string targetVariable = ProblemData.TargetVariable.Value;

      // select a random subset of rows in the validation set
      int validationStart = ValidiationSamplesStart.Value;
      int validationEnd = ValidationSamplesEnd.Value;
      int seed = Random.Next();
      int count = (int)((validationEnd - validationStart) * RelativeNumberOfEvaluatedSamples.Value);
      if (count == 0) count = 1;
      IEnumerable<int> rows = RandomEnumerable.SampleRandomNumbers(seed, validationStart, validationEnd, count)
        .Where(row => row < ProblemData.TestSamplesStart.Value || ProblemData.TestSamplesEnd.Value <= row);

      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      var trees = SymbolicExpressionTreeParameter.ActualValue;

      for (int i = 0; i < validationArr.Length; i++) {
        var tree = trees[i];
        double quality = Evaluator.Evaluate(SymbolicExpressionTreeInterpreter, tree,
            lowerEstimationLimit, upperEstimationLimit,
            ProblemData.Dataset, targetVariable,
           rows);
        validationArr[i] = quality;
      }
      
      #endregion


      double r = alglib.spearmancorr2(trainingArr, validationArr);

      TrainingValidationQualityCorrelationParameter.ActualValue = new DoubleValue(r);

      if (TrainingValidationQualityCorrelationTableParameter.ActualValue == null) {
        var dataTable = new DataTable("Training and validation fitness correlation table", "Data table of training and validation fitness correlation values over the whole run.");
        dataTable.Rows.Add(new DataRow("Training and validation fitness correlation", "Training and validation fitness correlation values"));
        TrainingValidationQualityCorrelationTableParameter.ActualValue = dataTable;
        ResultsParameter.ActualValue.Add(new Result(TrainingValidationCorrelationTableParameterName, dataTable));
      }

      TrainingValidationQualityCorrelationTableParameter.ActualValue.Rows["Training and validation fitness correlation"].Values.Add(r);

      double correlationThreshold;
      if (OverfittingParameter.ActualValue != null && OverfittingParameter.ActualValue.Value) {
        // if is already overfitting => have to reach the upper threshold to switch back to non-overfitting state
        correlationThreshold = UpperCorrelationThresholdParameter.ActualValue.Value;
      } else {
        // if currently in non-overfitting state => have to reach to lower threshold to switch to overfitting state
        correlationThreshold = LowerCorrelationThresholdParameter.ActualValue.Value;
      }
      bool overfitting = r < correlationThreshold;

      OverfittingParameter.ActualValue = new BoolValue(overfitting);

      return base.Apply();
    }
  }
}
