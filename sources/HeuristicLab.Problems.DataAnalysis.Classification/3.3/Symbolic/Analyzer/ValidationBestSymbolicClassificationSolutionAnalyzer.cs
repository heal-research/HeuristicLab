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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  [Item("ValidationBestSymbolicClassificationSolutionAnalyzer", "An operator that analyzes the validation best symbolic classification solution.")]
  [StorableClass]
  [NonDiscoverableType]
  public class ValidationBestSymbolicClassificationSolutionAnalyzer : SingleSuccessorOperator, ISymbolicClassificationAnalyzer {
    private const string MaximizationParameterName = "Maximization";
    private const string GenerationsParameterName = "Generations";
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";

    private const string ClassificationProblemDataParameterName = "ClassificationProblemData";
    private const string EvaluatorParameterName = "Evaluator";
    private const string ValidationSamplesStartParameterName = "SamplesStart";
    private const string ValidationSamplesEndParameterName = "SamplesEnd";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string CalculateSolutionComplexityParameterName = "CalculateSolutionComplexity";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";

    private const string ResultsParameterName = "Results";
    private const string BestValidationQualityParameterName = "Best validation quality";
    private const string BestValidationSolutionParameterName = "Best validation solution";
    private const string BestSolutionAccuracyTrainingParameterName = "Best solution accuracy (training)";
    private const string BestSolutionAccuracyTestParameterName = "Best solution accuracy (test)";
    private const string BestSolutionLengthParameterName = "Best solution length (on validation set)";
    private const string BestSolutionHeightParameterName = "Best solution height (on validation set)";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";

    public virtual bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public ILookupParameter<IntValue> GenerationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[GenerationsParameterName]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<ClassificationProblemData> ClassificationProblemDataParameter {
      get { return (ILookupParameter<ClassificationProblemData>)Parameters[ClassificationProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicClassificationEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationEvaluator>)Parameters[EvaluatorParameterName]; }
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
    public IValueLookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    public ILookupParameter<DataTable> VariableFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[VariableFrequenciesParameterName]; }
    }
    public IValueParameter<BoolValue> CalculateSolutionComplexityParameter {
      get { return (IValueParameter<BoolValue>)Parameters[CalculateSolutionComplexityParameterName]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestValidationQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestValidationQualityParameterName]; }
    }
    public ILookupParameter<SymbolicClassificationSolution> BestValidationSolutionParameter {
      get { return (ILookupParameter<SymbolicClassificationSolution>)Parameters[BestValidationSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionAccuracyTrainingParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionAccuracyTrainingParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionAccuracyTestParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionAccuracyTestParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionLengthParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionHeightParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionHeightParameterName]; }
    }
    #endregion
    #region properties
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    public IntValue Generations {
      get { return GenerationsParameter.ActualValue; }
    }
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }

    public ClassificationProblemData ClassificationProblemData {
      get { return ClassificationProblemDataParameter.ActualValue; }
    }
    public ISymbolicClassificationEvaluator Evaluator {
      get { return EvaluatorParameter.ActualValue; }
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
    public BoolValue ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.ActualValue; }
      set { ApplyLinearScalingParameter.ActualValue = value; }
    }
    public DataTable VariableFrequencies {
      get { return VariableFrequenciesParameter.ActualValue; }
    }
    public BoolValue CalculateSolutionComplexity {
      get { return CalculateSolutionComplexityParameter.Value; }
      set { CalculateSolutionComplexityParameter.Value = value; }
    }

    public ResultCollection Results {
      get { return ResultsParameter.ActualValue; }
    }
    public DoubleValue BestValidationQuality {
      get { return BestValidationQualityParameter.ActualValue; }
      protected set { BestValidationQualityParameter.ActualValue = value; }
    }
    public SymbolicClassificationSolution BestValidationSolution {
      get { return BestValidationSolutionParameter.ActualValue; }
      protected set { BestValidationSolutionParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionAccuracyTraining {
      get { return BestSolutionAccuracyTrainingParameter.ActualValue; }
      protected set { BestSolutionAccuracyTrainingParameter.ActualValue = value; }
    }
    public DoubleValue BestSolutionAccuracyTest {
      get { return BestSolutionAccuracyTestParameter.ActualValue; }
      protected set { BestSolutionAccuracyTestParameter.ActualValue = value; }
    }
    public IntValue BestSolutionLength {
      get { return BestSolutionLengthParameter.ActualValue; }
      set { BestSolutionLengthParameter.ActualValue = value; }
    }
    public IntValue BestSolutionHeight {
      get { return BestSolutionHeightParameter.ActualValue; }
      set { BestSolutionHeightParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected ValidationBestSymbolicClassificationSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected ValidationBestSymbolicClassificationSolutionAnalyzer(ValidationBestSymbolicClassificationSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public ValidationBestSymbolicClassificationSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new LookupParameter<ClassificationProblemData>(ClassificationProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new LookupParameter<ISymbolicClassificationEvaluator>(EvaluatorParameterName, "The evaluator which should be used to evaluate the solution on the validation set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
      Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the validation best solution should be calculated.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(false)));

      Parameters.Add(new ValueLookupParameter<ResultCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestValidationQualityParameterName, "The validation quality of the best solution in the current run."));
      Parameters.Add(new LookupParameter<SymbolicClassificationSolution>(BestValidationSolutionParameterName, "The best solution on the validation data found in the current run."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionAccuracyTrainingParameterName, "The training accuracy of the best solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionAccuracyTestParameterName, "The test accuracy of the best solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic classification solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic classification solution."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(CalculateSolutionComplexityParameterName)) {
        Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the validation best solution should be calculated.", new BoolValue(true)));
      }
      if (!Parameters.ContainsKey(BestSolutionLengthParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic classification solution."));
      }
      if (!Parameters.ContainsKey(BestSolutionHeightParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic classification solution."));
      }
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(false)));
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ValidationBestSymbolicClassificationSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var trees = SymbolicExpressionTree;
      string targetVariable = ClassificationProblemData.TargetVariable.Value;

      // select a random subset of rows in the validation set
      int validationStart = ValidiationSamplesStart.Value;
      int validationEnd = ValidationSamplesEnd.Value;
      int seed = Random.Next();
      int count = (int)((validationEnd - validationStart) * RelativeNumberOfEvaluatedSamples.Value);
      if (count == 0) count = 1;
      IEnumerable<int> rows = RandomEnumerable.SampleRandomNumbers(seed, validationStart, validationEnd, count)
         .Where(row => row < ClassificationProblemData.TestSamplesStart.Value || ClassificationProblemData.TestSamplesEnd.Value <= row);

      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      SymbolicExpressionTree bestTree = null;

      foreach (var tree in trees) {
        double quality = Evaluator.Evaluate(SymbolicExpressionTreeInterpreter, tree,
          lowerEstimationLimit, upperEstimationLimit, ClassificationProblemData.Dataset,
          targetVariable, rows);

        if ((Maximization.Value && quality > bestQuality) ||
            (!Maximization.Value && quality < bestQuality)) {
          bestQuality = quality;
          bestTree = tree;
        }
      }

      // if the best validation tree is better than the current best solution => update
      bool newBest =
        BestValidationQuality == null ||
        (Maximization.Value && bestQuality > BestValidationQuality.Value) ||
        (!Maximization.Value && bestQuality < BestValidationQuality.Value);
      if (newBest) {
        if (ApplyLinearScaling.Value) {
          double alpha, beta;
          SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(SymbolicExpressionTreeInterpreter, bestTree,
            lowerEstimationLimit, upperEstimationLimit,
            ClassificationProblemData.Dataset, targetVariable,
            ClassificationProblemData.TrainingIndizes, out beta, out alpha);

          // scale tree for solution
          bestTree = SymbolicRegressionSolutionLinearScaler.Scale(bestTree, alpha, beta);
        }
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          bestTree);

        if (BestValidationSolution == null) {
          BestValidationSolution = new SymbolicClassificationSolution(ClassificationProblemData, model, LowerEstimationLimit.Value, UpperEstimationLimit.Value);
          BestValidationSolution.Name = BestValidationSolutionParameterName;
          BestValidationSolution.Description = "Best solution on validation partition found over the whole run.";
          BestValidationQuality = new DoubleValue(bestQuality);
        } else {
          BestValidationSolution.Model = model;
          BestValidationQuality.Value = bestQuality;
        }

        UpdateBestSolutionResults();
      }
      return base.Apply();
    }

    private void UpdateBestSolutionResults() {
      if (CalculateSolutionComplexity.Value) {
        BestSolutionLength = new IntValue(BestValidationSolution.Model.SymbolicExpressionTree.Size);
        BestSolutionHeight = new IntValue(BestValidationSolution.Model.SymbolicExpressionTree.Height);
        if (!Results.ContainsKey(BestSolutionLengthParameterName)) {
          Results.Add(new Result(BestSolutionLengthParameterName, "Length of the best solution on the validation set", new IntValue()));
          Results.Add(new Result(BestSolutionHeightParameterName, "Height of the best solution on the validation set", new IntValue()));
        }
        Results[BestSolutionLengthParameterName].Value = BestSolutionLength;
        Results[BestSolutionHeightParameterName].Value = BestSolutionHeight;
      }

      BestSymbolicRegressionSolutionAnalyzer.UpdateBestSolutionResults(BestValidationSolution, ClassificationProblemData, Results, Generations, VariableFrequencies);

      IEnumerable<double> trainingValues = ClassificationProblemData.Dataset.GetEnumeratedVariableValues(
        ClassificationProblemData.TargetVariable.Value, ClassificationProblemData.TrainingIndizes);
      IEnumerable<double> testValues = ClassificationProblemData.Dataset.GetEnumeratedVariableValues(
        ClassificationProblemData.TargetVariable.Value, ClassificationProblemData.TestIndizes);

      OnlineAccuracyEvaluator accuracyEvaluator = new OnlineAccuracyEvaluator();
      var originalEnumerator = trainingValues.GetEnumerator();
      var estimatedEnumerator = BestValidationSolution.EstimatedTrainingClassValues.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        accuracyEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
      }
      double trainingAccuracy = accuracyEvaluator.Accuracy;

      accuracyEvaluator.Reset();
      originalEnumerator = testValues.GetEnumerator();
      estimatedEnumerator = BestValidationSolution.EstimatedTestClassValues.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        accuracyEvaluator.Add(originalEnumerator.Current, estimatedEnumerator.Current);
      }
      double testAccuracy = accuracyEvaluator.Accuracy;

      if (!Results.ContainsKey(BestSolutionAccuracyTrainingParameterName)) {
        BestSolutionAccuracyTraining = new DoubleValue(trainingAccuracy);
        BestSolutionAccuracyTest = new DoubleValue(testAccuracy);

        Results.Add(new Result(BestSolutionAccuracyTrainingParameterName, BestSolutionAccuracyTraining));
        Results.Add(new Result(BestSolutionAccuracyTestParameterName, BestSolutionAccuracyTest));
      } else {
        BestSolutionAccuracyTraining.Value = trainingAccuracy;
        BestSolutionAccuracyTest.Value = testAccuracy;
      }
    }
  }
}
