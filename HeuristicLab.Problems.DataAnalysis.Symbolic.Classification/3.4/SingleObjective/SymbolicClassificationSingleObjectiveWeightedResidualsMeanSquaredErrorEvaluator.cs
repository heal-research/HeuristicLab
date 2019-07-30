#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Weighted Residuals Mean Squared Error Evaluator", @"A modified mean squared error evaluator that enables the possibility to weight residuals differently.
The first residual category belongs to estimated values which definitely belong to a specific class because the estimated value is located above the maximum or below the minimum of all the class values (DefiniteResidualsWeight).
The second residual category represents residuals which belong to the positive class whereby the estimated value is located between the positive and a negative class (PositiveClassResidualsWeight).
All other cases are represented by the third category (NegativeClassesResidualsWeight).
The weight gets multiplied to the squared error. Note that the Evaluator acts like a normal MSE-Evaluator if all the weights are set to 1.")]
  [StorableType("A3193296-1A0F-46E2-8F43-22E2ED9CFFC5")]
  public sealed class SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    private const string DefiniteResidualsWeightParameterName = "DefiniteResidualsWeight";
    private const string PositiveClassResidualsWeightParameterName = "PositiveClassResidualsWeight";
    private const string NegativeClassesResidualsWeightParameterName = "NegativeClassesResidualsWeight";
    [StorableConstructor]
    private SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator(StorableConstructorFlag _) : base(_) { }
    private SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator(SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator(this, cloner);
    }

    public SymbolicClassificationSingleObjectiveWeightedResidualsMeanSquaredErrorEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(DefiniteResidualsWeightParameterName, "Weight of residuals which definitely belong to a specific class because the estimated values is located above the maximum or below the minimum of all the class values.", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(PositiveClassResidualsWeightParameterName, "Weight of residuals which belong to the positive class whereby the estimated value is located between the positive and a negative class.", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NegativeClassesResidualsWeightParameterName, "Weight of residuals which are not covered by the DefiniteResidualsWeight or the PositiveClassResidualsWeight.", new DoubleValue(1)));
    }

    #region parameter properties
    public IFixedValueParameter<DoubleValue> DefiniteResidualsWeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DefiniteResidualsWeightParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> PositiveClassResidualsWeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PositiveClassResidualsWeightParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> NegativeClassesResidualsWeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NegativeClassesResidualsWeightParameterName]; }
    }
    #endregion

    #region properties
    public override bool Maximization { get { return false; } }

    public double DefiniteResidualsWeight {
      get { return DefiniteResidualsWeightParameter.Value.Value; }
    }
    public double PositiveClassResidualsWeight {
      get { return PositiveClassResidualsWeightParameter.Value.Value; }
    }
    public double NegativeClassesResidualsWeight {
      get { return NegativeClassesResidualsWeightParameter.Value.Value; }
    }
    #endregion

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value,
        DefiniteResidualsWeight, PositiveClassResidualsWeight, NegativeClassesResidualsWeight);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree tree, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling,
      double definiteResidualsWeight, double positiveClassResidualsWeight, double negativeClassesResidualsWeight) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;

      double positiveClassValue = problemData.GetClassValue(problemData.PositiveClass);
      //get class values min/max
      double classValuesMin = problemData.ClassValues.ElementAtOrDefault(0);
      double classValuesMax = classValuesMin;
      foreach (double classValue in problemData.ClassValues) {
        if (classValuesMin > classValue) classValuesMin = classValue;
        if (classValuesMax < classValue) classValuesMax = classValue;
      }

      double quality;
      if (applyLinearScaling) {
        var calculator = new OnlineWeightedClassificationMeanSquaredErrorCalculator(positiveClassValue, classValuesMax, classValuesMin,
          definiteResidualsWeight, positiveClassResidualsWeight, negativeClassesResidualsWeight);
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, calculator, problemData.Dataset.Rows);
        errorState = calculator.ErrorState;
        quality = calculator.WeightedResidualsMeanSquaredError;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        quality = OnlineWeightedClassificationMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, positiveClassValue, classValuesMax,
          classValuesMin, definiteResidualsWeight, positiveClassResidualsWeight, negativeClassesResidualsWeight, out errorState);
      }
      if (errorState != OnlineCalculatorError.None) return Double.NaN;
      return quality;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value, DefiniteResidualsWeight, PositiveClassResidualsWeight, NegativeClassesResidualsWeight);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return quality;
    }
  }
}
