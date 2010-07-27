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
using System;

using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Regression;
using HeuristicLab.Analysis;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Evaluators;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic time series prognosis solution.
  /// </summary>
  [Item("ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic time series prognosis solution.")]
  [StorableClass]
  public sealed class ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ValidationSamplesStartParameterName = "SamplesStart";
    private const string ValidationSamplesEndParameterName = "SamplesEnd";
    private const string QualityParameterName = "Quality";
    private const string ScaledQualityParameterName = "ScaledQuality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string PredictionHorizonParameterName = "PredictionHorizon";
    private const string ConditionVariableParameterName = "ConditionVariableName";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";
    private const string BestSolutionParameterName = "Best solution (validation)";
    private const string BestSolutionQualityParameterName = "Best solution quality (validation)";
    private const string CurrentBestValidationQualityParameterName = "Current best validation quality";
    private const string BestSolutionQualityValuesParameterName = "Validation Quality";
    private const string ResultsParameterName = "Results";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string BestKnownQualityParameterName = "BestKnownQuality";
    private const string GenerationsParameterName = "Generations";

    private const string TrainingMeanSquaredErrorQualityParameterName = "Mean squared error (training)";
    private const string MinTrainingMeanSquaredErrorQualityParameterName = "Min mean squared error (training)";
    private const string MaxTrainingMeanSquaredErrorQualityParameterName = "Max mean squared error (training)";
    private const string AverageTrainingMeanSquaredErrorQualityParameterName = "Average mean squared error (training)";
    private const string BestTrainingMeanSquaredErrorQualityParameterName = "Best mean squared error (training)";

    private const string TrainingAverageRelativeErrorQualityParameterName = "Average relative error (training)";
    private const string MinTrainingAverageRelativeErrorQualityParameterName = "Min average relative error (training)";
    private const string MaxTrainingAverageRelativeErrorQualityParameterName = "Max average relative error (training)";
    private const string AverageTrainingAverageRelativeErrorQualityParameterName = "Average average relative error (training)";
    private const string BestTrainingAverageRelativeErrorQualityParameterName = "Best average relative error (training)";

    private const string TrainingRSquaredQualityParameterName = "R² (training)";
    private const string MinTrainingRSquaredQualityParameterName = "Min R² (training)";
    private const string MaxTrainingRSquaredQualityParameterName = "Max R² (training)";
    private const string AverageTrainingRSquaredQualityParameterName = "Average R² (training)";
    private const string BestTrainingRSquaredQualityParameterName = "Best R² (training)";

    private const string TestMeanSquaredErrorQualityParameterName = "Mean squared error (test)";
    private const string MinTestMeanSquaredErrorQualityParameterName = "Min mean squared error (test)";
    private const string MaxTestMeanSquaredErrorQualityParameterName = "Max mean squared error (test)";
    private const string AverageTestMeanSquaredErrorQualityParameterName = "Average mean squared error (test)";
    private const string BestTestMeanSquaredErrorQualityParameterName = "Best mean squared error (test)";

    private const string TestAverageRelativeErrorQualityParameterName = "Average relative error (test)";
    private const string MinTestAverageRelativeErrorQualityParameterName = "Min average relative error (test)";
    private const string MaxTestAverageRelativeErrorQualityParameterName = "Max average relative error (test)";
    private const string AverageTestAverageRelativeErrorQualityParameterName = "Average average relative error (test)";
    private const string BestTestAverageRelativeErrorQualityParameterName = "Best average relative error (test)";

    private const string TestRSquaredQualityParameterName = "R² (test)";
    private const string MinTestRSquaredQualityParameterName = "Min R² (test)";
    private const string MaxTestRSquaredQualityParameterName = "Max R² (test)";
    private const string AverageTestRSquaredQualityParameterName = "Average R² (test)";
    private const string BestTestRSquaredQualityParameterName = "Best R² (test)";

    private const string RSquaredValuesParameterName = "R²";
    private const string MeanSquaredErrorValuesParameterName = "Mean squared error";
    private const string RelativeErrorValuesParameterName = "Average relative error";
    private const string BestSolutionResultName = "Best solution (on validiation set)";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public OptionalValueParameter<StringValue> ConditionVariableNameParameter {
      get { return (OptionalValueParameter<StringValue>)Parameters[ConditionVariableParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> AlphaParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters[AlphaParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> BetaParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters[BetaParameterName]; }
    }
    public IValueLookupParameter<ISymbolicTimeSeriesExpressionInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicTimeSeriesExpressionInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<MultiVariateDataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<MultiVariateDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleArray> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleArray>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<IntValue> PredictionHorizonParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[PredictionHorizonParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<IntValue> GenerationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[GenerationsParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestKnownQualityParameterName]; }
    }
    public ILookupParameter<DataTable> VariableFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[VariableFrequenciesParameterName]; }
    }
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ItemArray<DoubleArray> Alpha {
      get { return AlphaParameter.ActualValue; }
    }
    public ItemArray<DoubleArray> Beta {
      get { return BetaParameter.ActualValue; }
    }
    public ISymbolicTimeSeriesExpressionInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public MultiVariateDataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public IntValue ValidiationSamplesStart {
      get { return ValidationSamplesStartParameter.ActualValue; }
    }
    public IntValue ValidationSamplesEnd {
      get { return ValidationSamplesEndParameter.ActualValue; }
    }
    public DoubleArray UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleArray LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public IntValue PredictionHorizon {
      get { return PredictionHorizonParameter.ActualValue; }
    }
    public StringValue ConditionVariableName {
      get { return ConditionVariableNameParameter.Value; }
    }
    public ResultCollection Results {
      get { return ResultsParameter.ActualValue; }
    }
    public DataTable VariableFrequencies {
      get { return VariableFrequenciesParameter.ActualValue; }
    }
    public IntValue Generations {
      get { return GenerationsParameter.ActualValue; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }
    #endregion

    public ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "A random number generator."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new OptionalValueParameter<StringValue>(ConditionVariableParameterName, "The name of the condition variable indicating if a row should be considered for evaluation or not."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(AlphaParameterName, "The alpha parameter for linear scaling."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(BetaParameterName, "The beta parameter for linear scaling."));
      Parameters.Add(new ValueLookupParameter<ISymbolicTimeSeriesExpressionInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<MultiVariateDataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(PredictionHorizonParameterName, "The number of time steps for which to create a forecast."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(BestSolutionParameterName, "The best symbolic time series prognosis solution."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The best known (validation) quality achieved on the data set."));
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));

    }

    [StorableConstructor]
    private ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer(bool deserializing) : base(deserializing) { }

    public override IOperation Apply() {
      var alphas = Alpha;
      var betas = Beta;
      var trees = SymbolicExpressionTree;

      IEnumerable<SymbolicExpressionTree> scaledTrees;
      if (alphas.Length == trees.Length) {
        scaledTrees = from i in Enumerable.Range(0, trees.Length)
                      select SymbolicVectorRegressionSolutionLinearScaler.Scale(trees[i], betas[i].ToArray(), alphas[i].ToArray());
      } else {
        scaledTrees = trees;
      }

      int trainingStart = ProblemData.TrainingSamplesStart.Value;
      int trainingEnd = ProblemData.TrainingSamplesEnd.Value;
      int testStart = ProblemData.TestSamplesStart.Value;
      int testEnd = ProblemData.TestSamplesEnd.Value;

      #region validation best model
      int validationStart = ValidiationSamplesStart.Value;
      int validationEnd = ValidationSamplesEnd.Value;
      int rowCount = (int)Math.Ceiling((validationEnd - validationStart) * RelativeNumberOfEvaluatedSamples.Value);
      IEnumerable<int> rows = RandomEnumerable.SampleRandomNumbers((uint)Random.Next(), validationStart, validationEnd, rowCount);
      double bestValidationNmse = double.MaxValue;
      SymbolicExpressionTree bestTree = null;
      string conditionalVariableName = ConditionVariableName == null ? null : ConditionVariableName.Value;
      foreach (var tree in scaledTrees) {
        double validationNmse;
        validationNmse = SymbolicTimeSeriesPrognosisNormalizedMseEvaluator.Evaluate(tree, ProblemData,
          SymbolicExpressionTreeInterpreter, conditionalVariableName,
          rows, PredictionHorizon.Value, LowerEstimationLimit, UpperEstimationLimit);
        if (validationNmse < bestValidationNmse) {
          bestValidationNmse = validationNmse;
          bestTree = tree;
        }
      }


      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > bestValidationNmse) {
        var solution = bestTree;
        //solution.Name = BestSolutionParameterName;
        //solution.Description = "Best solution on validation partition found over the whole run.";

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestValidationNmse);

        // BestSymbolicTimeSeriesPrognosisSolutionAnalyzer.UpdateBestSolutionResults(solution, ProblemData, Results, Generations, VariableFrequencies);
      }

      if (!Results.ContainsKey(BestSolutionQualityValuesParameterName)) {
        Results.Add(new Result(BestSolutionResultName, BestSolutionParameter.ActualValue));
        Results.Add(new Result(BestSolutionQualityValuesParameterName, new DataTable(BestSolutionQualityValuesParameterName, BestSolutionQualityValuesParameterName)));
        Results.Add(new Result(BestSolutionQualityParameterName, new DoubleValue()));
        Results.Add(new Result(CurrentBestValidationQualityParameterName, new DoubleValue()));
      }
      Results[BestSolutionResultName].Value = BestSolutionParameter.ActualValue;
      Results[BestSolutionQualityParameterName].Value = new DoubleValue(BestSolutionQualityParameter.ActualValue.Value);
      Results[CurrentBestValidationQualityParameterName].Value = new DoubleValue(bestValidationNmse);

      DataTable validationValues = (DataTable)Results[BestSolutionQualityValuesParameterName].Value;
      AddValue(validationValues, BestSolutionQualityParameter.ActualValue.Value, BestSolutionQualityParameterName, BestSolutionQualityParameterName);
      AddValue(validationValues, bestValidationNmse, CurrentBestValidationQualityParameterName, CurrentBestValidationQualityParameterName);
      #endregion
      return base.Apply();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
    }

    private static void AddValue(DataTable table, double data, string name, string description) {
      DataRow row;
      table.Rows.TryGetValue(name, out row);
      if (row == null) {
        row = new DataRow(name, description);
        row.Values.Add(data);
        table.Rows.Add(row);
      } else {
        row.Values.Add(data);
      }
    }
  }
}
