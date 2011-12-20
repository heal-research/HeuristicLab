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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Classification.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the training best scaled symbolic classification solution.
  /// </summary>
  [Item("TrainingBestSymbolicClassificationSolutionAnalyzer", "An operator that analyzes the training best scaled symbolic classification solution.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class TrainingBestSymbolicClassificationSolutionAnalyzer : SingleSuccessorOperator, ISymbolicClassificationAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string QualityParameterName = "Quality";
    private const string MaximizationParameterName = "Maximization";
    private const string CalculateSolutionComplexityParameterName = "CalculateSolutionComplexity";
    private const string CalculateSolutionAccuracyParameterName = "CalculateSolutionAccuracy";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ClassificationProblemData";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string BestSolutionParameterName = "Best training solution";
    private const string BestSolutionQualityParameterName = "Best training solution quality";
    private const string BestSolutionLengthParameterName = "Best training solution length";
    private const string BestSolutionHeightParameterName = "Best training solution height";
    private const string BestSolutionVariablesParameterName = "Best training solution variables";
    private const string BestSolutionTrainingRSquaredParameterName = "Best training solution R� (training)";
    private const string BestSolutionTestRSquaredParameterName = "Best training solution R� (test)";
    private const string BestSolutionTrainingMseParameterName = "Best training solution mean squared error (training)";
    private const string BestSolutionTestMseParameterName = "Best training solution mean squared error (test)";
    private const string BestSolutionTrainingRelativeErrorParameterName = "Best training solution relative error (training)";
    private const string BestSolutionTestRelativeErrorParameterName = "Best training solution relative error (test)";
    private const string BestSolutionAccuracyTrainingParameterName = "Best training solution accuracy (training)";
    private const string BestSolutionAccuracyTestParameterName = "Best training solution accuracy (test)";
    private const string ResultsParameterName = "Results";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public IValueParameter<BoolValue> CalculateSolutionComplexityParameter {
      get { return (IValueParameter<BoolValue>)Parameters[CalculateSolutionComplexityParameterName]; }
    }
    public IValueParameter<BoolValue> CalculateSolutionAccuracyParameter {
      get { return (IValueParameter<BoolValue>)Parameters[CalculateSolutionAccuracyParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<ClassificationProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<ClassificationProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }

    public ILookupParameter<SymbolicClassificationSolution> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicClassificationSolution>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionLengthParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionHeightParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionHeightParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionVariablesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionVariablesParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTrainingRSquaredParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTrainingRSquaredParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTestRSquaredParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTestRSquaredParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTrainingMseParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTrainingMseParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTestMseParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTestMseParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTrainingRelativeErrorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTrainingRelativeErrorParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTestRelativeErrorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionTestRelativeErrorParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTrainingAccuracyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionAccuracyTrainingParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionTestAccuracyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionAccuracyTestParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    public BoolValue CalculateSolutionComplexity {
      get { return CalculateSolutionComplexityParameter.Value; }
      set { CalculateSolutionComplexityParameter.Value = value; }
    }
    public BoolValue CalculateSolutionAccuracy {
      get { return CalculateSolutionAccuracyParameter.Value; }
      set { CalculateSolutionAccuracyParameter.Value = value; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public ClassificationProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public BoolValue ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.ActualValue; }
      set { ApplyLinearScalingParameter.ActualValue = value; }
    }

    public ResultCollection Results {
      get { return ResultsParameter.ActualValue; }
    }
    public SymbolicClassificationSolution BestSolution {
      get { return BestSolutionParameter.ActualValue; }
      set { BestSolutionParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionQuality {
      get { return BestSolutionQualityParameter.ActualValue; }
      set { BestSolutionQualityParameter.ActualValue = value; }
    }
    public IntValue BestSolutionLength {
      get { return BestSolutionLengthParameter.ActualValue; }
      set { BestSolutionLengthParameter.ActualValue = value; }
    }
    public IntValue BestSolutionHeight {
      get { return BestSolutionHeightParameter.ActualValue; }
      set { BestSolutionHeightParameter.ActualValue = value; }
    }
    public IntValue BestSolutionVariables {
      get { return BestSolutionVariablesParameter.ActualValue; }
      set { BestSolutionVariablesParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTrainingRSquared {
      get { return BestSolutionTrainingRSquaredParameter.ActualValue; }
      set { BestSolutionTrainingRSquaredParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTestRSquared {
      get { return BestSolutionTestRSquaredParameter.ActualValue; }
      set { BestSolutionTestRSquaredParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTrainingMse {
      get { return BestSolutionTrainingMseParameter.ActualValue; }
      set { BestSolutionTrainingMseParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTestMse {
      get { return BestSolutionTestMseParameter.ActualValue; }
      set { BestSolutionTestMseParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTrainingRelativeError {
      get { return BestSolutionTrainingRelativeErrorParameter.ActualValue; }
      set { BestSolutionTrainingRelativeErrorParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTestRelativeError {
      get { return BestSolutionTestRelativeErrorParameter.ActualValue; }
      set { BestSolutionTestRelativeErrorParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTrainingAccuracy {
      get { return BestSolutionTrainingAccuracyParameter.ActualValue; }
      set { BestSolutionTrainingAccuracyParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionTestAccuracy {
      get { return BestSolutionTestAccuracyParameter.ActualValue; }
      set { BestSolutionTestAccuracyParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private TrainingBestSymbolicClassificationSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private TrainingBestSymbolicClassificationSolutionAnalyzer(TrainingBestSymbolicClassificationSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public TrainingBestSymbolicClassificationSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the symbolic expression trees to analyze."));
      Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the training best solution should be calculated.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionAccuracyParameterName, "Determines if the accuracy of the training best solution on the training and test set should be calculated.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<ClassificationProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<SymbolicClassificationSolution>(BestSolutionParameterName, "The best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic classfication solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionVariablesParameterName, "The number of variables used by the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTrainingRSquaredParameterName, "The R� value on the training set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTestRSquaredParameterName, "The R� value on the test set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTrainingMseParameterName, "The mean squared error on the training set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTestMseParameterName, "The mean squared error value on the test set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTrainingRelativeErrorParameterName, "The relative error on the training set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionTestRelativeErrorParameterName, "The relative error value on the test set of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionAccuracyTrainingParameterName, "The accuracy on the training set of the best symbolic classification  solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionAccuracyTestParameterName, "The accuracy on the test set of the best symbolic classification  solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic classification solution should be stored."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(false)));
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TrainingBestSymbolicClassificationSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      #region find best tree
      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      SymbolicExpressionTree bestTree = null;
      SymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      double[] quality = Quality.Select(x => x.Value).ToArray();
      for (int i = 0; i < tree.Length; i++) {
        if ((Maximization.Value && quality[i] > bestQuality) ||
            (!Maximization.Value && quality[i] < bestQuality)) {
          bestQuality = quality[i];
          bestTree = tree[i];
        }
      }
      #endregion

      #region update best solution
      // if the best tree is better than the current best solution => update
      bool newBest =
        BestSolutionQuality == null ||
        (Maximization.Value && bestQuality > BestSolutionQuality.Value) ||
        (!Maximization.Value && bestQuality < BestSolutionQuality.Value);
      if (newBest) {
        double lowerEstimationLimit = LowerEstimationLimit.Value;
        double upperEstimationLimit = UpperEstimationLimit.Value;
        string targetVariable = ProblemData.TargetVariable.Value;

        if (ApplyLinearScaling.Value) {
          // calculate scaling parameters and only for the best tree using the full training set
          double alpha, beta;
          SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(SymbolicExpressionTreeInterpreter, bestTree,
            lowerEstimationLimit, upperEstimationLimit,
            ProblemData.Dataset, targetVariable,
            ProblemData.TrainingIndizes, out beta, out alpha);

          // scale tree for solution
          bestTree = SymbolicRegressionSolutionLinearScaler.Scale(bestTree, alpha, beta);
        }
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          bestTree);
        var solution = new SymbolicClassificationSolution((ClassificationProblemData)ProblemData.Clone(), model, lowerEstimationLimit, upperEstimationLimit);
        solution.Name = BestSolutionParameterName;
        solution.Description = "Best solution on training partition found over the whole run.";

        BestSolution = solution;
        BestSolutionQuality = new DoubleValue(bestQuality);

        if (CalculateSolutionComplexity.Value) {
          BestSolutionLength = new IntValue(solution.Model.SymbolicExpressionTree.Size);
          BestSolutionHeight = new IntValue(solution.Model.SymbolicExpressionTree.Height);
          BestSolutionVariables = new IntValue(solution.Model.InputVariables.Count());
          if (!Results.ContainsKey(BestSolutionLengthParameterName)) {
            Results.Add(new Result(BestSolutionLengthParameterName, "Length of the best solution on the training set.", BestSolutionLength));
            Results.Add(new Result(BestSolutionHeightParameterName, "Height of the best solution on the training set.", BestSolutionHeight));
            Results.Add(new Result(BestSolutionVariablesParameterName, "Number of variables used by the best solution on the training set.", BestSolutionVariables));
          } else {
            Results[BestSolutionLengthParameterName].Value = BestSolutionLength;
            Results[BestSolutionHeightParameterName].Value = BestSolutionHeight;
            Results[BestSolutionVariablesParameterName].Value = BestSolutionVariables;
          }
        }

        if (CalculateSolutionAccuracy.Value) {
          #region update R2,MSE, Rel Error
          IEnumerable<double> trainingValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable.Value, ProblemData.TrainingIndizes);
          IEnumerable<double> testValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable.Value, ProblemData.TestIndizes);
          OnlineAccuracyEvaluator accuracyEvaluator = new OnlineAccuracyEvaluator();
          OnlineMeanSquaredErrorEvaluator mseEvaluator = new OnlineMeanSquaredErrorEvaluator();
          OnlineMeanAbsolutePercentageErrorEvaluator relErrorEvaluator = new OnlineMeanAbsolutePercentageErrorEvaluator();
          OnlinePearsonsRSquaredEvaluator r2Evaluator = new OnlinePearsonsRSquaredEvaluator();

          #region training
          var originalEnumerator = trainingValues.GetEnumerator();
          var estimatedEnumerator = solution.EstimatedTrainingClassValues.GetEnumerator();
          while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
            accuracyEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
          }
          originalEnumerator = trainingValues.GetEnumerator();
          estimatedEnumerator = solution.EstimatedTrainingValues.GetEnumerator();
          while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
            mseEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
            r2Evaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
            relErrorEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
          }
          double trainingAccuracy = accuracyEvaluator.Accuracy;
          double trainingR2 = r2Evaluator.RSquared;
          double trainingMse = mseEvaluator.MeanSquaredError;
          double trainingRelError = relErrorEvaluator.MeanAbsolutePercentageError;
          #endregion

          accuracyEvaluator.Reset();
          mseEvaluator.Reset();
          relErrorEvaluator.Reset();
          r2Evaluator.Reset();

          #region test
          originalEnumerator = testValues.GetEnumerator();
          estimatedEnumerator = solution.EstimatedTestClassValues.GetEnumerator();
          while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
            accuracyEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
          }
          originalEnumerator = testValues.GetEnumerator();
          estimatedEnumerator = solution.EstimatedTestValues.GetEnumerator();
          while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
            mseEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
            r2Evaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
            relErrorEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
          }
          double testAccuracy = accuracyEvaluator.Accuracy;
          double testR2 = r2Evaluator.RSquared;
          double testMse = mseEvaluator.MeanSquaredError;
          double testRelError = relErrorEvaluator.MeanAbsolutePercentageError;
          #endregion
          BestSolutionTrainingAccuracy = new DoubleValue(trainingAccuracy);
          BestSolutionTestAccuracy = new DoubleValue(testAccuracy);
          BestSolutionTrainingRSquared = new DoubleValue(trainingR2);
          BestSolutionTestRSquared = new DoubleValue(testR2);
          BestSolutionTrainingMse = new DoubleValue(trainingMse);
          BestSolutionTestMse = new DoubleValue(testMse);
          BestSolutionTrainingRelativeError = new DoubleValue(trainingRelError);
          BestSolutionTestRelativeError = new DoubleValue(testRelError);

          if (!Results.ContainsKey(BestSolutionAccuracyTrainingParameterName)) {
            Results.Add(new Result(BestSolutionTrainingRSquaredParameterName, BestSolutionTrainingRSquared));
            Results.Add(new Result(BestSolutionTestRSquaredParameterName, BestSolutionTestRSquared));
            Results.Add(new Result(BestSolutionTrainingMseParameterName, BestSolutionTrainingMse));
            Results.Add(new Result(BestSolutionTestMseParameterName, BestSolutionTestMse));
            Results.Add(new Result(BestSolutionTrainingRelativeErrorParameterName, BestSolutionTrainingRelativeError));
            Results.Add(new Result(BestSolutionTestRelativeErrorParameterName, BestSolutionTestRelativeError));
            Results.Add(new Result(BestSolutionAccuracyTrainingParameterName, BestSolutionTrainingAccuracy));
            Results.Add(new Result(BestSolutionAccuracyTestParameterName, BestSolutionTestAccuracy));
          } else {
            Results[BestSolutionTrainingRSquaredParameterName].Value = BestSolutionTrainingRSquared;
            Results[BestSolutionTestRSquaredParameterName].Value = BestSolutionTestRSquared;
            Results[BestSolutionTrainingMseParameterName].Value = BestSolutionTrainingMse;
            Results[BestSolutionTestMseParameterName].Value = BestSolutionTestMse;
            Results[BestSolutionTrainingRelativeErrorParameterName].Value = BestSolutionTrainingRelativeError;
            Results[BestSolutionTestRelativeErrorParameterName].Value = BestSolutionTestRelativeError;
            Results[BestSolutionAccuracyTrainingParameterName].Value = BestSolutionTrainingAccuracy;
            Results[BestSolutionAccuracyTestParameterName].Value = BestSolutionTestAccuracy;
          }
          #endregion
        }

        if (!Results.ContainsKey(BestSolutionQualityParameterName)) {
          Results.Add(new Result(BestSolutionQualityParameterName, BestSolutionQuality));
          Results.Add(new Result(BestSolutionParameterName, BestSolution));
        } else {
          Results[BestSolutionQualityParameterName].Value = BestSolutionQuality;
          Results[BestSolutionParameterName].Value = BestSolution;
        }
      }
      #endregion
      return base.Apply();
    }
  }
}
