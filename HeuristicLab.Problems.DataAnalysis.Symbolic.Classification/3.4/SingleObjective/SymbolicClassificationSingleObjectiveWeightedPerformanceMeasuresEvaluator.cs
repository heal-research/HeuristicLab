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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [NonDiscoverableType]
  [Item("Weighted Performance Measures Evaluator", "Calculates the quality of a symbolic classification solution based on three weighted measures(normalized mean squared error, false negative rate(1-sensitivity) and false positve rate(1-specificity)).")]
  [StorableType("0772F316-5E12-4153-857E-8625069B4677")]
  public class SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    private const string NormalizedMeanSquaredErrorWeightingFactorParameterName = "NormalizedMeanSquaredErrorWeightingFactor";
    private const string FalseNegativeRateWeightingFactorParameterName = "FalseNegativeRateWeightingFactor";
    private const string FalsePositiveRateWeightingFactorParameterName = "FalsePositiveRateWeightingFactor";
    private const string ModelCreatorParameterName = "ModelCreator";

    public override bool Maximization { get { return false; } }

    #region parameter properties
    public IFixedValueParameter<DoubleValue> NormalizedMeanSquaredErrorWeightingFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NormalizedMeanSquaredErrorWeightingFactorParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> FalseNegativeRateWeightingFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[FalseNegativeRateWeightingFactorParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> FalsePositiveRateWeightingFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[FalsePositiveRateWeightingFactorParameterName]; }
    }
    public IValueLookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (IValueLookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion

    public double NormalizedMeanSquaredErrorWeightingFactor {
      get { return NormalizedMeanSquaredErrorWeightingFactorParameter.Value.Value; }
    }
    public double FalseNegativeRateWeightingFactor {
      get { return FalseNegativeRateWeightingFactorParameter.Value.Value; }
    }
    public double FalsePositiveRateWeightingFactor {
      get { return FalsePositiveRateWeightingFactorParameter.Value.Value; }
    }

    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator(SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator(this, cloner);
    }

    public SymbolicClassificationSingleObjectiveWeightedPerformanceMeasuresEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(NormalizedMeanSquaredErrorWeightingFactorParameterName, "The weighting factor of the normalized mean squared error.", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(FalseNegativeRateWeightingFactorParameterName, "The weighting factor of the false negative rate (1-sensitivity).", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(FalsePositiveRateWeightingFactorParameterName, "The weighting factor of the false positive rate (1-specificity).", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, "The model creator which is used during the evaluations."));
    }

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var creator = ModelCreatorParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;


      double quality = Calculate(interpreter, tree, estimationLimits.Lower, estimationLimits.Upper,
              ProblemDataParameter.ActualValue, rows, applyLinearScaling, creator, NormalizedMeanSquaredErrorWeightingFactor, FalseNegativeRateWeightingFactor, FalsePositiveRateWeightingFactor);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree tree, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData,
                IEnumerable<int> rows, bool applyLinearScaling, ISymbolicClassificationModelCreator modelCreator, double normalizedMeanSquaredErrorWeightingFactor, double falseNegativeRateWeightingFactor, double falsePositiveRateWeightingFactor) {
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      var targetClassValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit).ToArray();
      OnlineCalculatorError errorState;
      double nmse;

      //calculate performance measures
      string positiveClassName = problemData.PositiveClass;
      double[] classValues, thresholds;
      IEnumerable<double> estimatedClassValues = null;
      ISymbolicDiscriminantFunctionClassificationModel m;

      var model = modelCreator.CreateSymbolicClassificationModel(problemData.TargetVariable, tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
      if ((m = model as ISymbolicDiscriminantFunctionClassificationModel) != null) {
        m.ThresholdCalculator.Calculate(problemData, boundedEstimatedValues, targetClassValues, out classValues, out thresholds);
        m.SetThresholdsAndClassValues(thresholds, classValues);
        estimatedClassValues = m.GetEstimatedClassValues(boundedEstimatedValues);
      } else {
        model.RecalculateModelParameters(problemData, rows);
        estimatedClassValues = model.GetEstimatedClassValues(problemData.Dataset, rows);
      }

      var performanceCalculator = new ClassificationPerformanceMeasuresCalculator(positiveClassName, problemData.GetClassValue(positiveClassName));
      performanceCalculator.Calculate(targetClassValues, estimatedClassValues);
      if (performanceCalculator.ErrorState != OnlineCalculatorError.None)
        return Double.NaN;
      double falseNegativeRate = 1 - performanceCalculator.TruePositiveRate;
      double falsePositiveRate = performanceCalculator.FalsePositiveRate;

      if (applyLinearScaling) {
        throw new NotSupportedException("The Weighted Performance Measures Evaluator does not suppport linear scaling!");
      }
      nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetClassValues, boundedEstimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return Double.NaN;
      return normalizedMeanSquaredErrorWeightingFactor * nmse + falseNegativeRateWeightingFactor * falseNegativeRate + falsePositiveRateWeightingFactor * falsePositiveRate;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      ModelCreatorParameter.ExecutionContext = context;

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper,
                                      problemData, rows, ApplyLinearScalingParameter.ActualValue.Value, ModelCreatorParameter.ActualValue, NormalizedMeanSquaredErrorWeightingFactorParameter.Value.Value, FalseNegativeRateWeightingFactor, FalsePositiveRateWeightingFactor);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;
      ModelCreatorParameter.ExecutionContext = null;

      return quality;
    }
  }
}
