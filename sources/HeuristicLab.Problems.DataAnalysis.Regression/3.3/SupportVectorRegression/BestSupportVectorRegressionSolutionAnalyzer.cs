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
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;

namespace HeuristicLab.Problems.DataAnalysis.Regression.SupportVectorRegression {
  [Item("BestSupportVectorRegressionSolutionAnalyzer", "An operator for analyzing the best support vector solution of regression problems.")]
  [StorableClass]
  public sealed class BestSymbolicRegressionSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string SupportVectorRegressionModelParameterName = "SupportVectorRegressionModel";
    private const string ProblemDataParameterName = "ProblemData";
    private const string QualityParameterName = "Quality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string BestSolutionParameterName = "BestSolution";
    private const string BestSolutionQualityParameterName = "BestSolutionQuality";
    private const string ResultsParameterName = "Results";
    private const string BestSolutionResultName = "Best solution (on validiation set)";
    private const string BestSolutionInputvariableCountResultName = "Variables used by best solution";
    private const string BestSolutionTrainingRSquared = "Best solution R² (training)";
    private const string BestSolutionTestRSquared = "Best solution R² (test)";
    private const string BestSolutionTrainingMse = "Best solution mean squared error (training)";
    private const string BestSolutionTestMse = "Best solution mean squared error (test)";
    private const string BestSolutionTrainingRelativeError = "Best solution average relative error (training)";
    private const string BestSolutionTestRelativeError = "Best solution average relative error (test)";

    public ScopeTreeLookupParameter<SupportVectorMachineModel> SupportVectorRegressionModelParameter {
      get { return (ScopeTreeLookupParameter<SupportVectorMachineModel>)Parameters[SupportVectorRegressionModelParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public ILookupParameter<SupportVectorRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SupportVectorRegressionSolution>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }

    public BestSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SupportVectorMachineModel>(SupportVectorRegressionModelParameterName, "The support vector regression models to analyze."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the support vector model is a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the support vector model."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the support vector model."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the support vector models which should be analyzed."));
      Parameters.Add(new LookupParameter<SupportVectorRegressionSolution>(BestSolutionParameterName, "The best support vector solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best support vector solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best support vector solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      ItemArray<SupportVectorMachineModel> models = SupportVectorRegressionModelParameter.ActualValue;
      DataAnalysisProblemData problemData = ProblemDataParameter.ActualValue;
      DoubleValue upperEstimationLimit = UpperEstimationLimitParameter.ActualValue;
      DoubleValue lowerEstimationLimit = LowerEstimationLimitParameter.ActualValue;
      var inputVariables = ProblemDataParameter.ActualValue.InputVariables.Select(x => x.Value);

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      SupportVectorRegressionSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new SupportVectorRegressionSolution(problemData, models[i], inputVariables, lowerEstimationLimit.Value, upperEstimationLimit.Value);
        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = qualities[i];
        results.Add(new Result(BestSolutionResultName, solution));
        results.Add(new Result(BestSolutionInputvariableCountResultName, new IntValue(inputVariables.Count())));
        #region calculate R2,MSE,Rel Error
        double[] trainingValues = problemData.Dataset.GetVariableValues(
          problemData.TargetVariable.Value,
          problemData.TrainingSamplesStart.Value,
          problemData.TrainingSamplesEnd.Value);
        double[] testValues = problemData.Dataset.GetVariableValues(
          problemData.TargetVariable.Value,
          problemData.TestSamplesStart.Value,
          problemData.TestSamplesEnd.Value);
        double trainingR2 = SimpleRSquaredEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
        double testR2 = SimpleRSquaredEvaluator.Calculate(testValues, solution.EstimatedTestValues);
        double trainingMse = SimpleMSEEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
        double testMse = SimpleMSEEvaluator.Calculate(testValues, solution.EstimatedTestValues);
        double trainingRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
        double testRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(testValues, solution.EstimatedTestValues);
        results.Add(new Result(BestSolutionTrainingRSquared, new DoubleValue(trainingR2)));
        results.Add(new Result(BestSolutionTestRSquared, new DoubleValue(testR2)));
        results.Add(new Result(BestSolutionTrainingMse, new DoubleValue(trainingMse)));
        results.Add(new Result(BestSolutionTestMse, new DoubleValue(testMse)));
        results.Add(new Result(BestSolutionTrainingRelativeError, new DoubleValue(trainingRelError)));
        results.Add(new Result(BestSolutionTestRelativeError, new DoubleValue(testRelError)));
        #endregion
      } else {
        if (BestSolutionQualityParameter.ActualValue.Value > qualities[i].Value) {
          solution = new SupportVectorRegressionSolution(problemData, models[i], inputVariables, lowerEstimationLimit.Value, upperEstimationLimit.Value);
          BestSolutionParameter.ActualValue = solution;
          BestSolutionQualityParameter.ActualValue = qualities[i];
          results[BestSolutionResultName].Value = solution;
          results[BestSolutionInputvariableCountResultName].Value = new IntValue(inputVariables.Count());
          #region update R2,MSE, Rel Error
          double[] trainingValues = problemData.Dataset.GetVariableValues(
            problemData.TargetVariable.Value,
            problemData.TrainingSamplesStart.Value,
            problemData.TrainingSamplesEnd.Value);
          double[] testValues = problemData.Dataset.GetVariableValues(
            problemData.TargetVariable.Value,
            problemData.TestSamplesStart.Value,
            problemData.TestSamplesEnd.Value);
          double trainingR2 = SimpleRSquaredEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
          double testR2 = SimpleRSquaredEvaluator.Calculate(testValues, solution.EstimatedTestValues);
          double trainingMse = SimpleMSEEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
          double testMse = SimpleMSEEvaluator.Calculate(testValues, solution.EstimatedTestValues);
          double trainingRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
          double testRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(testValues, solution.EstimatedTestValues);
          results[BestSolutionTrainingRSquared].Value = new DoubleValue(trainingR2);
          results[BestSolutionTestRSquared].Value = new DoubleValue(testR2);
          results[BestSolutionTrainingMse].Value = new DoubleValue(trainingMse);
          results[BestSolutionTestMse].Value = new DoubleValue(testMse);
          results[BestSolutionTrainingRelativeError].Value = new DoubleValue(trainingRelError);
          results[BestSolutionTestRelativeError].Value = new DoubleValue(testRelError);
          #endregion
        }
      }

      return base.Apply();
    }

    private IEnumerable<string> GetInputVariables(SymbolicExpressionTree tree) {
      return (from varNode in tree.IterateNodesPrefix().OfType<VariableTreeNode>()
              select varNode.VariableName).Distinct();
    }
  }
}
