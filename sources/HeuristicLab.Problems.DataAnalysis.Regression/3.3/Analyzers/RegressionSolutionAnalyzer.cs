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

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  [StorableClass]
  public abstract class RegressionSolutionAnalyzer : SingleSuccessorOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string QualityParameterName = "Quality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string BestSolutionQualityParameterName = "BestSolutionQuality";
    private const string GenerationsParameterName = "Generations";
    private const string ResultsParameterName = "Results";
    private const string BestSolutionResultName = "Best solution (on validiation set)";
    private const string BestSolutionTrainingRSquared = "Best solution R² (training)";
    private const string BestSolutionTestRSquared = "Best solution R² (test)";
    private const string BestSolutionTrainingMse = "Best solution mean squared error (training)";
    private const string BestSolutionTestMse = "Best solution mean squared error (test)";
    private const string BestSolutionTrainingRelativeError = "Best solution average relative error (training)";
    private const string BestSolutionTestRelativeError = "Best solution average relative error (test)";
    private const string BestSolutionGeneration = "Best solution generation";

    #region parameter properties
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
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public ILookupParameter<IntValue> GenerationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[GenerationsParameterName]; }
    }
    #endregion
    #region properties
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    public ResultCollection Results {
      get { return ResultsParameter.ActualValue; }
    }
    public DataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    #endregion

    public RegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the symbolic regression trees which should be analyzed."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    public void Initialize() {
      // backwards compatibility
      if (!Parameters.ContainsKey(GenerationsParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      }
    }

    public override IOperation Apply() {
      DoubleValue prevBestSolutionQuality = BestSolutionQualityParameter.ActualValue;
      var bestSolution = UpdateBestSolution();
      if (prevBestSolutionQuality == null || prevBestSolutionQuality.Value > BestSolutionQualityParameter.ActualValue.Value) {
        UpdateBestSolutionResults(bestSolution);
      }

      return base.Apply();
    }
    private void UpdateBestSolutionResults(DataAnalysisSolution bestSolution) {
      var solution = bestSolution;
      #region update R2,MSE, Rel Error
      double[] trainingValues = ProblemData.Dataset.GetVariableValues(
        ProblemData.TargetVariable.Value,
        ProblemData.TrainingSamplesStart.Value,
        ProblemData.TrainingSamplesEnd.Value);
      double[] testValues = ProblemData.Dataset.GetVariableValues(
        ProblemData.TargetVariable.Value,
        ProblemData.TestSamplesStart.Value,
        ProblemData.TestSamplesEnd.Value);
      double trainingR2 = SimpleRSquaredEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
      double testR2 = SimpleRSquaredEvaluator.Calculate(testValues, solution.EstimatedTestValues);
      double trainingMse = SimpleMSEEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
      double testMse = SimpleMSEEvaluator.Calculate(testValues, solution.EstimatedTestValues);
      double trainingRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(trainingValues, solution.EstimatedTrainingValues);
      double testRelError = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(testValues, solution.EstimatedTestValues);
      if (Results.ContainsKey(BestSolutionResultName)) {
        Results[BestSolutionResultName].Value = solution;
        Results[BestSolutionTrainingRSquared].Value = new DoubleValue(trainingR2);
        Results[BestSolutionTestRSquared].Value = new DoubleValue(testR2);
        Results[BestSolutionTrainingMse].Value = new DoubleValue(trainingMse);
        Results[BestSolutionTestMse].Value = new DoubleValue(testMse);
        Results[BestSolutionTrainingRelativeError].Value = new DoubleValue(trainingRelError);
        Results[BestSolutionTestRelativeError].Value = new DoubleValue(testRelError);
        if (GenerationsParameter.ActualValue != null) // this check is needed because linear regression solutions do not have a generations parameter
          Results[BestSolutionGeneration].Value = new IntValue(GenerationsParameter.ActualValue.Value);
      } else {
        Results.Add(new Result(BestSolutionResultName, solution));
        Results.Add(new Result(BestSolutionTrainingRSquared, new DoubleValue(trainingR2)));
        Results.Add(new Result(BestSolutionTestRSquared, new DoubleValue(testR2)));
        Results.Add(new Result(BestSolutionTrainingMse, new DoubleValue(trainingMse)));
        Results.Add(new Result(BestSolutionTestMse, new DoubleValue(testMse)));
        Results.Add(new Result(BestSolutionTrainingRelativeError, new DoubleValue(trainingRelError)));
        Results.Add(new Result(BestSolutionTestRelativeError, new DoubleValue(testRelError)));
        if (GenerationsParameter.ActualValue != null)
          Results.Add(new Result(BestSolutionGeneration, new IntValue(GenerationsParameter.ActualValue.Value)));
      }
      #endregion
    }

    protected abstract DataAnalysisSolution UpdateBestSolution();
  }
}
