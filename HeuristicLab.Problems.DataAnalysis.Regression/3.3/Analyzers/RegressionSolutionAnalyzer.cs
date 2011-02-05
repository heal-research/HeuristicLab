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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
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
    private const string BestSolutionResultName = "Best solution (on validation set)";
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


    [StorableConstructor]
    protected RegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected RegressionSolutionAnalyzer(RegressionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
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
    private void AfterDeserialization() {
      // backwards compatibility
      if (!Parameters.ContainsKey(GenerationsParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      }
    }

    public override IOperation Apply() {
      DoubleValue prevBestSolutionQuality = BestSolutionQualityParameter.ActualValue;
      var bestSolution = UpdateBestSolution();
      if (prevBestSolutionQuality == null || prevBestSolutionQuality.Value > BestSolutionQualityParameter.ActualValue.Value) {
        RegressionSolutionAnalyzer.UpdateBestSolutionResults(bestSolution, ProblemData, Results, GenerationsParameter.ActualValue);
      }

      return base.Apply();
    }

    public static void UpdateBestSolutionResults(DataAnalysisSolution solution, DataAnalysisProblemData problemData, ResultCollection results, IntValue generation) {
      #region update R2,MSE, Rel Error
      IEnumerable<double> trainingValues = problemData.Dataset.GetEnumeratedVariableValues(problemData.TargetVariable.Value, problemData.TrainingIndizes);
      IEnumerable<double> testValues = problemData.Dataset.GetEnumeratedVariableValues(problemData.TargetVariable.Value, problemData.TestIndizes);
      OnlineMeanSquaredErrorEvaluator mseEvaluator = new OnlineMeanSquaredErrorEvaluator();
      OnlineMeanAbsolutePercentageErrorEvaluator relErrorEvaluator = new OnlineMeanAbsolutePercentageErrorEvaluator();
      OnlinePearsonsRSquaredEvaluator r2Evaluator = new OnlinePearsonsRSquaredEvaluator();

      #region training
      var originalEnumerator = trainingValues.GetEnumerator();
      var estimatedEnumerator = solution.EstimatedTrainingValues.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        mseEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
        r2Evaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
        relErrorEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
      }
      double trainingR2 = r2Evaluator.RSquared;
      double trainingMse = mseEvaluator.MeanSquaredError;
      double trainingRelError = relErrorEvaluator.MeanAbsolutePercentageError;
      #endregion

      mseEvaluator.Reset();
      relErrorEvaluator.Reset();
      r2Evaluator.Reset();

      #region test
      originalEnumerator = testValues.GetEnumerator();
      estimatedEnumerator = solution.EstimatedTestValues.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        mseEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
        r2Evaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
        relErrorEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
      }
      double testR2 = r2Evaluator.RSquared;
      double testMse = mseEvaluator.MeanSquaredError;
      double testRelError = relErrorEvaluator.MeanAbsolutePercentageError;
      #endregion

      if (results.ContainsKey(BestSolutionResultName)) {
        results[BestSolutionResultName].Value = solution;
        results[BestSolutionTrainingRSquared].Value = new DoubleValue(trainingR2);
        results[BestSolutionTestRSquared].Value = new DoubleValue(testR2);
        results[BestSolutionTrainingMse].Value = new DoubleValue(trainingMse);
        results[BestSolutionTestMse].Value = new DoubleValue(testMse);
        results[BestSolutionTrainingRelativeError].Value = new DoubleValue(trainingRelError);
        results[BestSolutionTestRelativeError].Value = new DoubleValue(testRelError);
        if (generation != null) // this check is needed because linear regression solutions do not have a generations parameter
          results[BestSolutionGeneration].Value = new IntValue(generation.Value);
      } else {
        results.Add(new Result(BestSolutionResultName, solution));
        results.Add(new Result(BestSolutionTrainingRSquared, new DoubleValue(trainingR2)));
        results.Add(new Result(BestSolutionTestRSquared, new DoubleValue(testR2)));
        results.Add(new Result(BestSolutionTrainingMse, new DoubleValue(trainingMse)));
        results.Add(new Result(BestSolutionTestMse, new DoubleValue(testMse)));
        results.Add(new Result(BestSolutionTrainingRelativeError, new DoubleValue(trainingRelError)));
        results.Add(new Result(BestSolutionTestRelativeError, new DoubleValue(testRelError)));
        if (generation != null)
          results.Add(new Result(BestSolutionGeneration, new IntValue(generation.Value)));
      }
      #endregion
    }

    protected abstract DataAnalysisSolution UpdateBestSolution();
  }
}
