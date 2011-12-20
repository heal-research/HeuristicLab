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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer : SymbolicRegressionValidationAnalyzer, ISymbolicRegressionAnalyzer {
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string MaximizationParameterName = "Maximization";
    private const string CalculateSolutionComplexityParameterName = "CalculateSolutionComplexity";
    private const string BestSolutionParameterName = "Best solution (validation)";
    private const string BestSolutionQualityParameterName = "Best solution quality (validation)";
    private const string BestSolutionLengthParameterName = "Best solution length (validation)";
    private const string BestSolutionHeightParameterName = "Best solution height (validiation)";
    private const string CurrentBestValidationQualityParameterName = "Current best validation quality";
    private const string BestSolutionQualityValuesParameterName = "Validation Quality";
    private const string ResultsParameterName = "Results";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string BestKnownQualityParameterName = "BestKnownQuality";
    private const string GenerationsParameterName = "Generations";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public IValueParameter<BoolValue> CalculateSolutionComplexityParameter {
      get { return (IValueParameter<BoolValue>)Parameters[CalculateSolutionComplexityParameterName]; }
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
    public ILookupParameter<IntValue> BestSolutionLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionLengthParameterName]; }
    }
    public ILookupParameter<IntValue> BestSolutionHeightParameter {
      get { return (ILookupParameter<IntValue>)Parameters[BestSolutionHeightParameterName]; }
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
    public IValueLookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion
    #region properties
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    public BoolValue CalculateSolutionComplexity {
      get { return CalculateSolutionComplexityParameter.Value; }
      set { CalculateSolutionComplexityParameter.Value = value; }
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
    public IntValue BestSolutionLength {
      get { return BestSolutionLengthParameter.ActualValue; }
      set { BestSolutionLengthParameter.ActualValue = value; }
    }
    public IntValue BestSolutionHeight {
      get { return BestSolutionHeightParameter.ActualValue; }
      set { BestSolutionHeightParameter.ActualValue = value; }
    }
    public BoolValue ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.ActualValue; }
      set { ApplyLinearScalingParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer(FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(true)));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the validation best solution should be calculated.", new BoolValue(true)));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations calculated so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The best known (validation) quality achieved on the data set."));
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region compatibility remove before releasing 3.4
      if (!Parameters.ContainsKey("Evaluator")) {
        Parameters.Add(new LookupParameter<ISymbolicRegressionEvaluator>("Evaluator", "The evaluator which should be used to evaluate the solution on the validation set."));
      }
      if (!Parameters.ContainsKey(MaximizationParameterName)) {
        Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      }
      if (!Parameters.ContainsKey(CalculateSolutionComplexityParameterName)) {
        Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the validation best solution should be calculated.", new BoolValue(false)));
      }
      if (!Parameters.ContainsKey(BestSolutionLengthParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic regression solution."));
      }
      if (!Parameters.ContainsKey(BestSolutionHeightParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic regression solution."));
      }
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyLinearScalingParameterName, "The switch determines if the best solution should be linearly scaled on the whole training set.", new BoolValue(true)));
      }
      #endregion
    }

    protected override void Analyze(SymbolicExpressionTree[] trees, double[] validationQuality) {
      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      SymbolicExpressionTree bestTree = null;

      for (int i = 0; i < trees.Length; i++) {
        double quality = validationQuality[i];
        if ((Maximization.Value && quality > bestQuality) ||
            (!Maximization.Value && quality < bestQuality)) {
          bestQuality = quality;
          bestTree = trees[i];
        }
      }

      // if the best validation tree is better than the current best solution => update
      bool newBest =
        BestSolutionQuality == null ||
        (Maximization.Value && bestQuality > BestSolutionQuality.Value) ||
        (!Maximization.Value && bestQuality < BestSolutionQuality.Value);
      if (newBest) {
        double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
        double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;
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
        var solution = new SymbolicRegressionSolution((DataAnalysisProblemData)ProblemData.Clone(), model, lowerEstimationLimit, upperEstimationLimit);
        solution.Name = BestSolutionParameterName;
        solution.Description = "Best solution on validation partition found over the whole run.";

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestQuality);

        if (CalculateSolutionComplexity.Value) {
          BestSolutionLength = new IntValue(solution.Model.SymbolicExpressionTree.Size);
          BestSolutionHeight = new IntValue(solution.Model.SymbolicExpressionTree.Height);
          if (!Results.ContainsKey(BestSolutionLengthParameterName)) {
            Results.Add(new Result(BestSolutionLengthParameterName, "Length of the best solution on the validation set", new IntValue()));
            Results.Add(new Result(BestSolutionHeightParameterName, "Height of the best solution on the validation set", new IntValue()));
          }
          Results[BestSolutionLengthParameterName].Value = BestSolutionLength;
          Results[BestSolutionHeightParameterName].Value = BestSolutionHeight;
        }

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
