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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  /// <summary>
  /// An operator for visualizing the best symbolic regression solution based on the validation set.
  /// </summary>
  [Item("BestSymbolicExpressionTreeVisualizer", "An operator for visualizing the best symbolic regression solution based on the validation set.")]
  [StorableClass]
  public sealed class BestValidationSymbolicRegressionSolutionVisualizer : SingleSuccessorOperator, ISingleObjectiveSolutionsVisualizer, ISolutionsVisualizer {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string SymbolicRegressionModelParameterName = "SymbolicRegressionModel";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string BestValidationSolutionParameterName = "BestValidationSolution";
    private const string ValidationSamplesStartParameterName = "ValidationSamplesStart";
    private const string ValidationSamplesEndParameterName = "ValidationSamplesEnd";
    private const string QualityParameterName = "Quality";
    private const string ResultsParameterName = "Results";

    #region parameter properties
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesEndParameterName]; }
    }

    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicRegressionModelParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public ILookupParameter<SymbolicRegressionSolution> BestValidationSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestValidationSolutionParameterName]; }
    }
    ILookupParameter ISolutionsVisualizer.VisualizationParameter {
      get { return BestValidationSolutionParameter; }
    }

    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters[QualityParameterName]; }
    }

    public ILookupParameter<ResultCollection> ResultParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion

    #region properties
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public IntValue ValidationSamplesStart {
      get { return ValidationSamplesStartParameter.ActualValue; }
    }
    public IntValue ValidationSamplesEnd {
      get { return ValidationSamplesEndParameter.ActualValue; }
    }
    #endregion

    public BestValidationSymbolicRegressionSolutionVisualizer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>(SymbolicRegressionModelParameterName, "The symbolic regression solutions from which the best solution should be visualized."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>(QualityParameterName, "The quality of the symbolic regression solutions."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The symbolic regression problme data on which the best solution should be evaluated."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The start index of the validation partition (part of the training partition)."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The end index of the validation partition (part of the training partition)."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestValidationSolutionParameterName, "The best symbolic expression tree based on the validation data for the symbolic regression problem."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection of the algorithm."));
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      DataAnalysisProblemData problemData = DataAnalysisProblemDataParameter.ActualValue;

      int validationSamplesStart = ValidationSamplesStart.Value;
      int validationSamplesEnd = ValidationSamplesEnd.Value;
      var validationValues = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, validationSamplesStart, validationSamplesEnd);
      double upperEstimationLimit = UpperEstimationLimit.Value;
      double lowerEstimationLimit = LowerEstimationLimit.Value;
      var currentBestExpression = (from expression in expressions
                                   let validationQuality =
                                     SymbolicRegressionMeanSquaredErrorEvaluator.Calculate(
                                       SymbolicExpressionTreeInterpreter, expression,
                                       lowerEstimationLimit, upperEstimationLimit,
                                       problemData.Dataset, problemData.TargetVariable.Value,
                                       validationSamplesStart, validationSamplesEnd)
                                   select new { Expression = expression, ValidationQuality = validationQuality })
                                   .OrderBy(x => x.ValidationQuality)
                                   .First();

      SymbolicRegressionSolution bestOfRunSolution = BestValidationSolutionParameter.ActualValue;
      if (bestOfRunSolution == null) {
        // no best of run solution yet -> make a solution from the currentBestExpression
        UpdateBestOfRunSolution(problemData, currentBestExpression.Expression, SymbolicExpressionTreeInterpreter, lowerEstimationLimit, upperEstimationLimit);
      } else {
        // compare quality of current best with best of run solution
        var estimatedValidationValues = bestOfRunSolution.EstimatedValues.Skip(validationSamplesStart).Take(validationSamplesEnd - validationSamplesStart);
        var bestOfRunValidationQuality = SimpleMSEEvaluator.Calculate(validationValues, estimatedValidationValues);
        if (bestOfRunValidationQuality > currentBestExpression.ValidationQuality) {
          UpdateBestOfRunSolution(problemData, currentBestExpression.Expression, SymbolicExpressionTreeInterpreter, lowerEstimationLimit, upperEstimationLimit);
        }
      }


      return base.Apply();
    }

    private void UpdateBestOfRunSolution(DataAnalysisProblemData problemData, SymbolicExpressionTree tree, ISymbolicExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit, double upperEstimationLimit) {
      var newBestSolution = CreateDataAnalysisSolution(problemData, tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
      if (BestValidationSolutionParameter.ActualValue == null)
        BestValidationSolutionParameter.ActualValue = newBestSolution;
      else
        // only update model
        BestValidationSolutionParameter.ActualValue.Model = newBestSolution.Model;

      var trainingValues = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, problemData.TrainingSamplesStart.Value, problemData.TrainingSamplesEnd.Value);
      var testValues = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, problemData.TestSamplesStart.Value, problemData.TestSamplesEnd.Value);

      AddResult("MeanSquaredError (Training)", new DoubleValue(SimpleMSEEvaluator.Calculate(trainingValues, newBestSolution.EstimatedTrainingValues)));
      AddResult("MeanRelativeError (Training)", new PercentValue(SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(trainingValues, newBestSolution.EstimatedTrainingValues)));
      AddResult("RSquared (Training)", new DoubleValue(SimpleRSquaredEvaluator.Calculate(trainingValues, newBestSolution.EstimatedTrainingValues)));

      AddResult("MeanSquaredError (Test)", new DoubleValue(SimpleMSEEvaluator.Calculate(testValues, newBestSolution.EstimatedTestValues)));
      AddResult("MeanRelativeError (Test)", new PercentValue(SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(testValues, newBestSolution.EstimatedTestValues)));
      AddResult("RSquared (Test)", new DoubleValue(SimpleRSquaredEvaluator.Calculate(testValues, newBestSolution.EstimatedTestValues)));
    }

    private void AddResult(string resultName, IItem value) {
      var resultCollection = ResultParameter.ActualValue;
      if (resultCollection.ContainsKey(resultName)) {
        resultCollection[resultName].Value = value;
      } else {
        resultCollection.Add(new Result(resultName, value));
      }
    }

    private SymbolicRegressionSolution CreateDataAnalysisSolution(DataAnalysisProblemData problemData, SymbolicExpressionTree expression, ISymbolicExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit, double upperEstimationLimit) {
      var model = new SymbolicRegressionModel(interpreter, expression, problemData.InputVariables.Select(s => s.Value));
      return new SymbolicRegressionSolution(problemData, model, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
