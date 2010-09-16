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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  public sealed class FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ValidationSamplesStartParameterName = "SamplesStart";
    private const string ValidationSamplesEndParameterName = "SamplesEnd";
    // private const string QualityParameterName = "Quality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string EvaluatorParameterName = "Evaluator";
    private const string MaximizationParameterName = "Maximization";
    private const string BestSolutionParameterName = "Best solution (validation)";
    private const string BestSolutionQualityParameterName = "Best solution quality (validation)";
    private const string CurrentBestValidationQualityParameterName = "Current best validation quality";
    private const string BestSolutionQualityValuesParameterName = "Validation Quality";
    private const string ResultsParameterName = "Results";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string BestKnownQualityParameterName = "BestKnownQuality";
    private const string GenerationsParameterName = "Generations";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<ISymbolicRegressionEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicRegressionEvaluator>)Parameters[EvaluatorParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
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
    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
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
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public ISymbolicRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.ActualValue; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
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
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
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
    public DoubleValue BestSolutionQuality {
      get { return BestSolutionQualityParameter.ActualValue; }
    }

    #endregion

    public FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<ISymbolicRegressionEvaluator>(EvaluatorParameterName, "The evaluator which should be used to evaluate the solution on the validation set."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
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

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region compatibility remove before releasing 3.3.1
      if (!Parameters.ContainsKey(EvaluatorParameterName)) {
        Parameters.Add(new LookupParameter<ISymbolicRegressionEvaluator>(EvaluatorParameterName, "The evaluator which should be used to evaluate the solution on the validation set."));
      }
      if (!Parameters.ContainsKey(MaximizationParameterName)) {
        Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      }
      #endregion
    }

    public override IOperation Apply() {
      var trees = SymbolicExpressionTree;

      string targetVariable = ProblemData.TargetVariable.Value;

      // select a random subset of rows in the validation set
      int validationStart = ValidiationSamplesStart.Value;
      int validationEnd = ValidationSamplesEnd.Value;
      int seed = Random.Next();
      int count = (int)((validationEnd - validationStart) * RelativeNumberOfEvaluatedSamples.Value);
      if (count == 0) count = 1;
      IEnumerable<int> rows = RandomEnumerable.SampleRandomNumbers(seed, validationStart, validationEnd, count);

      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      SymbolicExpressionTree bestTree = null;

      foreach (var tree in trees) {
        double quality = Evaluator.Evaluate(SymbolicExpressionTreeInterpreter, tree,
          lowerEstimationLimit, upperEstimationLimit,
          ProblemData.Dataset, targetVariable,
         rows);

        if ((Maximization.Value && quality > bestQuality) ||
            (!Maximization.Value && quality < bestQuality)) {
          bestQuality = quality;
          bestTree = tree;
        }
      }

      // if the best validation tree is better than the current best solution => update
      bool newBest =
        BestSolutionQuality == null ||
        (Maximization.Value && bestQuality > BestSolutionQuality.Value) ||
        (!Maximization.Value && bestQuality < BestSolutionQuality.Value);
      if (newBest) {
        // calculate scaling parameters and only for the best tree using the full training set
        double alpha, beta;
        int trainingStart = ProblemData.TrainingSamplesStart.Value;
        int trainingEnd = ProblemData.TrainingSamplesEnd.Value;
        IEnumerable<int> trainingRows = Enumerable.Range(trainingStart, trainingEnd - trainingStart);
        SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(SymbolicExpressionTreeInterpreter, bestTree,
          lowerEstimationLimit, upperEstimationLimit,
          ProblemData.Dataset, targetVariable,
          trainingRows, out beta, out alpha);

        // scale tree for solution
        var scaledTree = SymbolicRegressionSolutionLinearScaler.Scale(bestTree, alpha, beta);
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          scaledTree);
        var solution = new SymbolicRegressionSolution(ProblemData, model, lowerEstimationLimit, upperEstimationLimit);
        solution.Name = BestSolutionParameterName;
        solution.Description = "Best solution on validation partition found over the whole run.";

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestQuality);

        BestSymbolicRegressionSolutionAnalyzer.UpdateBestSolutionResults(solution, ProblemData, Results, Generations, VariableFrequencies);
      }


      if (!Results.ContainsKey(BestSolutionQualityValuesParameterName)) {
        Results.Add(new Result(BestSolutionQualityValuesParameterName, new DataTable(BestSolutionQualityValuesParameterName, BestSolutionQualityValuesParameterName)));
        Results.Add(new Result(BestSolutionQualityParameterName, new DoubleValue()));
        Results.Add(new Result(CurrentBestValidationQualityParameterName, new DoubleValue()));
      }
      Results[BestSolutionQualityParameterName].Value = new DoubleValue(BestSolutionQualityParameter.ActualValue.Value);
      Results[CurrentBestValidationQualityParameterName].Value = new DoubleValue(bestQuality);

      DataTable validationValues = (DataTable)Results[BestSolutionQualityValuesParameterName].Value;
      AddValue(validationValues, BestSolutionQualityParameter.ActualValue.Value, BestSolutionQualityParameterName, BestSolutionQualityParameterName);
      AddValue(validationValues, bestQuality, CurrentBestValidationQualityParameterName, CurrentBestValidationQualityParameterName);
      return base.Apply();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() { }

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
