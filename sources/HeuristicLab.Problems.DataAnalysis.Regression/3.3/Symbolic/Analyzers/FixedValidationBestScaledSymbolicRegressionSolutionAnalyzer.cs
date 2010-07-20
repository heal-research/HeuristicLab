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
using HeuristicLab.Analysis;
using System;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  public sealed class FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ValidationSamplesStartParameterName = "SamplesStart";
    private const string ValidationSamplesEndParameterName = "SamplesEnd";
    private const string QualityParameterName = "Quality";
    private const string ScaledQualityParameterName = "ScaledQuality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
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

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> AlphaParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[AlphaParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> BetaParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[BetaParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public ILookupParameter<SymbolicRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestSolutionParameterName]; }
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

    #endregion
    #region properties
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    public ItemArray<DoubleValue> Alpha {
      get { return AlphaParameter.ActualValue; }
    }
    public ItemArray<DoubleValue> Beta {
      get { return BetaParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public DataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public IntValue ValidiationSamplesStart {
      get { return ValidationSamplesStartParameter.ActualValue; }
    }
    public IntValue ValidationSamplesEnd {
      get { return ValidationSamplesEndParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
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

    #endregion

    public FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The quality of the symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(AlphaParameterName, "The alpha parameter for linear scaling."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(BetaParameterName, "The beta parameter for linear scaling."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The best known (validation) quality achieved on the data set."));
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
    }

    [StorableConstructor]
    private FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer(bool deserializing) : base() { }

    public override IOperation Apply() {
      var alphas = Alpha;
      var betas = Beta;
      var trees = SymbolicExpressionTree;

      IEnumerable<SymbolicExpressionTree> scaledTrees;
      if (alphas.Length == trees.Length) {
        scaledTrees = from i in Enumerable.Range(0, trees.Length)
                      select SymbolicRegressionSolutionLinearScaler.Scale(trees[i], alphas[i].Value, betas[i].Value);
      } else {
        scaledTrees = trees;
      }

      string targetVariable = ProblemData.TargetVariable.Value;
      int validationStart = ValidiationSamplesStart.Value;
      int validationEnd = ValidationSamplesEnd.Value;
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      double bestValidationMse = double.MaxValue;
      SymbolicExpressionTree bestTree = null;

      OnlineMeanSquaredErrorEvaluator mseEvaluator = new OnlineMeanSquaredErrorEvaluator();
      foreach (var scaledTree in scaledTrees) {
        double validationMse = SymbolicRegressionMeanSquaredErrorEvaluator.Calculate(SymbolicExpressionTreeInterpreter, scaledTree,
          lowerEstimationLimit, upperEstimationLimit,
          ProblemData.Dataset, targetVariable,
         Enumerable.Range(validationStart, validationEnd - validationStart));

        if (validationMse < bestValidationMse) {
          bestValidationMse = validationMse;
          bestTree = scaledTree;
        }
      }

      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > bestValidationMse) {
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          bestTree);
        var solution = new SymbolicRegressionSolution(ProblemData, model, lowerEstimationLimit, upperEstimationLimit);
        solution.Name = BestSolutionParameterName;
        solution.Description = "Best solution on validation partition found over the whole run.";

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestValidationMse);

        BestSymbolicRegressionSolutionAnalyzer.UpdateBestSolutionResults(solution, ProblemData, Results, Generations, VariableFrequencies);
      }

      if (!Results.ContainsKey(BestSolutionQualityValuesParameterName)) {
        Results.Add(new Result(BestSolutionQualityValuesParameterName, new DataTable(BestSolutionQualityValuesParameterName, BestSolutionQualityValuesParameterName)));
        Results.Add(new Result(BestSolutionQualityParameterName, new DoubleValue()));
        Results.Add(new Result(CurrentBestValidationQualityParameterName, new DoubleValue()));
      }
      Results[BestSolutionQualityParameterName].Value = new DoubleValue(BestSolutionQualityParameter.ActualValue.Value);
      Results[CurrentBestValidationQualityParameterName].Value = new DoubleValue(bestValidationMse);

      DataTable validationValues = (DataTable)Results[BestSolutionQualityValuesParameterName].Value;
      AddValue(validationValues, BestSolutionQualityParameter.ActualValue.Value, BestSolutionQualityParameterName, BestSolutionQualityParameterName);
      AddValue(validationValues, bestValidationMse, CurrentBestValidationQualityParameterName, CurrentBestValidationQualityParameterName);
      return base.Apply();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (!Parameters.ContainsKey(AlphaParameterName)) {
        Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(AlphaParameterName, "The alpha parameter for linear scaling."));
      }
      if (!Parameters.ContainsKey(BetaParameterName)) {
        Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(BetaParameterName, "The beta parameter for linear scaling."));
      }
      if (!Parameters.ContainsKey(VariableFrequenciesParameterName)) {
        Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
      }
      if (!Parameters.ContainsKey(GenerationsParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      }
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
