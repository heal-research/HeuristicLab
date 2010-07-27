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
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.Regression;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Evaluators {
  [Item("SymbolicTimeSeriesPrognosisScaledNormalizedMseEvaluator", "")]
  [StorableClass]
  public class SymbolicTimeSeriesPrognosisScaledNormalizedMseEvaluator : SingleSuccessorOperator, ISingleObjectiveSymbolicTimeSeriesPrognosisEvaluator {
    private const string RandomParameterName = "Random";
    private const string DataAnalysisProblemDataParameterName = "MultiVariateDataAnalysisProblemData";
    private const string TimeSeriesExpressionInterpreterParameterName = "TimeSeriesExpressionInterpreter";
    private const string TimeSeriesPrognosisModelParameterName = "TimeSeriesPrognosisModel";
    private const string PredictionHorizontParameterName = "PredictionHorizon";
    private const string ConditionVariableParameterName = "ConditionVariableName";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string QualityParameterName = "Quality";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<MultiVariateDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<MultiVariateDataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicTimeSeriesExpressionInterpreter> TimeSeriesExpressionInterpreterParameter {
      get { return (ILookupParameter<ISymbolicTimeSeriesExpressionInterpreter>)Parameters[TimeSeriesExpressionInterpreterParameterName]; }
    }
    public IValueLookupParameter<IntValue> PredictionHorizonParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[PredictionHorizontParameterName]; }
    }
    public OptionalValueParameter<StringValue> ConditionVariableNameParameter {
      get { return (OptionalValueParameter<StringValue>)Parameters[ConditionVariableParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> TimeSeriesPrognosisModelParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[TimeSeriesPrognosisModelParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<DoubleArray> AlphaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[AlphaParameterName]; }
    }
    public ILookupParameter<DoubleArray> BetaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[BetaParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }

    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public MultiVariateDataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public ISymbolicTimeSeriesExpressionInterpreter TimeSeriesExpressionInterpreter {
      get { return TimeSeriesExpressionInterpreterParameter.ActualValue; }
    }
    public IntValue PredictionHorizon {
      get { return PredictionHorizonParameter.ActualValue; }
    }
    public StringValue ConditionVariableName {
      get { return ConditionVariableNameParameter.Value; }
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
    public SymbolicExpressionTree TimeSeriesPrognosisModel {
      get { return TimeSeriesPrognosisModelParameter.ActualValue; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }
    #endregion

    public SymbolicTimeSeriesPrognosisScaledNormalizedMseEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "A random number generator."));
      Parameters.Add(new LookupParameter<MultiVariateDataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new LookupParameter<ISymbolicTimeSeriesExpressionInterpreter>(TimeSeriesExpressionInterpreterParameterName, "The interpreter that should be used to evaluate the time series model represented as a symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition to use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the data set partition to use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(PredictionHorizontParameterName, "The number of time steps for which to create a forecast."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(LowerEstimationLimitParameterName, "The lower limit for estimated values."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(UpperEstimationLimitParameterName, "The upper limit for estimated values."));
      Parameters.Add(new OptionalValueParameter<StringValue>(ConditionVariableParameterName, "The name of the condition variable indicating if a row should be considered for evaluation or not."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(TimeSeriesPrognosisModelParameterName, "The time series prognosis model encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the time series prognosis model encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DoubleArray>(AlphaParameterName, "The alpha parameter for linear scaling based on one step predictions."));
      Parameters.Add(new LookupParameter<DoubleArray>(BetaParameterName, "The beta parameter for linear scaling based on one step predictions."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
    }

    public override IOperation Apply() {
      double[] alpha, beta;
      double quality;
      string conditionVariableName = ConditionVariableName == null ? null : ConditionVariableName.Value;
      int nRows = (int)Math.Ceiling((SamplesEnd.Value - SamplesStart.Value) * RelativeNumberOfEvaluatedSamples.Value);

      IEnumerable<int> rows = RandomEnumerable.SampleRandomNumbers((uint)Random.Next(), SamplesStart.Value, SamplesEnd.Value, nRows);
      CalculateScalingParameters(TimeSeriesPrognosisModel, ProblemData, TimeSeriesExpressionInterpreter,
        conditionVariableName, rows,
        out beta, out alpha);

      quality = Evaluate(TimeSeriesPrognosisModel, ProblemData, TimeSeriesExpressionInterpreter,
        conditionVariableName, rows, PredictionHorizon.Value,
        LowerEstimationLimit, UpperEstimationLimit,
        beta, alpha);
      QualityParameter.ActualValue = new DoubleValue(quality);
      AlphaParameter.ActualValue = new DoubleArray(alpha);
      BetaParameter.ActualValue = new DoubleArray(beta);
      return base.Apply();
    }

    public static double Evaluate(SymbolicExpressionTree tree, MultiVariateDataAnalysisProblemData problemData,
      ISymbolicTimeSeriesExpressionInterpreter interpreter,
      IEnumerable<int> rows, int predictionHorizon, 
      DoubleArray lowerEstimationLimit, DoubleArray upperEstimationLimit,
      double[] beta, double[] alpha) {
      return Evaluate(tree, problemData, interpreter, null, rows, predictionHorizon, lowerEstimationLimit, upperEstimationLimit, beta, alpha);
    }

    public static double Evaluate(SymbolicExpressionTree tree, MultiVariateDataAnalysisProblemData problemData,
      ISymbolicTimeSeriesExpressionInterpreter interpreter, string conditionVariableName,
      IEnumerable<int> rows, int predictionHorizon,
      DoubleArray lowerEstimationLimit, DoubleArray upperEstimationLimit,
      double[] beta, double[] alpha) {
      if (conditionVariableName != null) {
        rows = from row in rows
               where !problemData.Dataset[conditionVariableName, row].IsAlmost(0.0)
               select row;
      }
      IEnumerable<string> selectedTargetVariables = from targetVariable in problemData.TargetVariables
                                                    where problemData.TargetVariables.ItemChecked(targetVariable)
                                                    select targetVariable.Value;

      IEnumerable<double[]> estimatedValues =
        interpreter.GetScaledSymbolicExpressionTreeValues(tree, problemData.Dataset, selectedTargetVariables,
        rows, predictionHorizon, beta, alpha);

      IEnumerable<double[]> originalValues = from row in rows
                                             from step in Enumerable.Range(0, predictionHorizon)
                                             select (from targetVariable in selectedTargetVariables
                                                     select problemData.Dataset[targetVariable, row + step]).ToArray();

      List<OnlineNormalizedMeanSquaredErrorEvaluator> evaluators = new List<OnlineNormalizedMeanSquaredErrorEvaluator>();
      foreach (string targetVariable in selectedTargetVariables)
        evaluators.Add(new OnlineNormalizedMeanSquaredErrorEvaluator());

      var estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      var originalValuesEnumerator = originalValues.GetEnumerator();
      while (originalValuesEnumerator.MoveNext() & estimatedValuesEnumerator.MoveNext()) {
        double[] original = originalValuesEnumerator.Current;
        double[] estimated = estimatedValuesEnumerator.Current;
        for (int i = 0; i < evaluators.Count; i++) {
          if (double.IsNaN(estimated[i])) estimated[i] = upperEstimationLimit[i];
          else estimated[i] = Math.Min(upperEstimationLimit[i], Math.Max(lowerEstimationLimit[i], estimated[i]));
          evaluators[i].Add(original[i], estimated[i]);
        }
      }

      double quality = evaluators.Select(x => x.NormalizedMeanSquaredError).Sum();
      return quality;
    }

    public static void CalculateScalingParameters(SymbolicExpressionTree tree, MultiVariateDataAnalysisProblemData problemData,
      ISymbolicTimeSeriesExpressionInterpreter interpreter,
      IEnumerable<int> rows, 
      out double[] betas, out double[] alphas) {
      CalculateScalingParameters(tree, problemData, interpreter, null, rows, out betas, out alphas);
    }

    public static void CalculateScalingParameters(SymbolicExpressionTree tree, MultiVariateDataAnalysisProblemData problemData,
      ISymbolicTimeSeriesExpressionInterpreter interpreter, string conditionVariableName,
      IEnumerable<int> rows, out double[] betas, out double[] alphas) {
      IEnumerable<string> selectedTargetVariables = from item in problemData.TargetVariables
                                                    where problemData.TargetVariables.ItemChecked(item)
                                                    select item.Value;
      int dimension = selectedTargetVariables.Count();

      if (conditionVariableName != null) {
        rows = from row in rows
               where !problemData.Dataset[conditionVariableName, row].IsAlmost(0.0)
               select row;
      }

      IEnumerable<double[]> oneStepPredictions =
        interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, selectedTargetVariables, rows, 1);
      IEnumerable<double[]> originalValues = from row in rows
                                             select (from targetVariable in selectedTargetVariables
                                                     select problemData.Dataset[targetVariable, row]).ToArray();

      alphas = new double[dimension];
      betas = new double[dimension];
      int[] cnt = new int[dimension];
      List<OnlineMeanAndVarianceCalculator> estimatedVarianceEvaluators = new List<OnlineMeanAndVarianceCalculator>();
      List<OnlineCovarianceEvaluator> covarianceEvaluators = new List<OnlineCovarianceEvaluator>();
      List<OnlineMeanAndVarianceCalculator> originalMeanCalculators = new List<OnlineMeanAndVarianceCalculator>();
      foreach (var selectedTargetVariable in selectedTargetVariables) {
        estimatedVarianceEvaluators.Add(new OnlineMeanAndVarianceCalculator());
        covarianceEvaluators.Add(new OnlineCovarianceEvaluator());
        originalMeanCalculators.Add(new OnlineMeanAndVarianceCalculator());
      }
      var estimatedEnumerator = oneStepPredictions.GetEnumerator();
      var originalEnumerator = originalValues.GetEnumerator();
      while (estimatedEnumerator.MoveNext() & originalEnumerator.MoveNext()) {
        double[] original = originalEnumerator.Current;
        double[] estimated = estimatedEnumerator.Current;
        for (int component = 0; component < dimension; component++) {
          if (IsValidValue(original[component]) && IsValidValue(estimated[component])) {
            cnt[component]++;
            estimatedVarianceEvaluators[component].Add(estimated[component]);
            covarianceEvaluators[component].Add(original[component], estimated[component]);
            originalMeanCalculators[component].Add(original[component]);
          }
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())
        throw new InvalidOperationException("Number of elements in estimated and original series doesn't match.");
      for (int component = 0; component < dimension; component++) {
        if (cnt[component] < 2) {
          alphas[component] = 0;
          betas[component] = 1;
        } else {
          if (estimatedVarianceEvaluators[component].Variance.IsAlmost(0.0))
            betas[component] = 1;
          else
            betas[component] = covarianceEvaluators[component].Covariance / estimatedVarianceEvaluators[component].Variance;

          alphas[component] = originalMeanCalculators[component].Mean - betas[component] * estimatedVarianceEvaluators[component].Mean;
        }
      }
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d) && d > -1.0E07 && d < 1.0E07;  // don't consider very large or very small values for scaling
    }
  }
}
