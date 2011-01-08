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
using HeuristicLab.Common;
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
  /// An operator that analyzes the training best scaled symbolic regression solution.
  /// </summary>
  [Item("TrainingBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the training best scaled symbolic regression solution.")]
  [StorableClass]
  public sealed class TrainingBestScaledSymbolicRegressionSolutionAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string QualityParameterName = "Quality";
    private const string MaximizationParameterName = "Maximization";
    private const string CalculateSolutionComplexityParameterName = "CalculateSolutionComplexity";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "DataAnalysisProblemData";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string BestSolutionParameterName = "Best solution (training)";
    private const string BestSolutionQualityParameterName = "Best solution quality (training)";
    private const string BestSolutionLengthParameterName = "Best solution length (training)";
    private const string BestSolutionHeightParameterName = "Best solution height (training)";
    private const string BestSolutionVariablesParameterName = "Best solution variables (training)"; 
    private const string ResultsParameterName = "Results";

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
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
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
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public DataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
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
    public SymbolicRegressionSolution BestSolution {
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

    #endregion

    [StorableConstructor]
    private TrainingBestScaledSymbolicRegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private TrainingBestScaledSymbolicRegressionSolutionAnalyzer(TrainingBestScaledSymbolicRegressionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public TrainingBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the symbolic expression trees to analyze."));
      Parameters.Add(new ValueParameter<BoolValue>(CalculateSolutionComplexityParameterName, "Determines if the length and height of the training best solution should be calculated.", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionLengthParameterName, "The length of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionHeightParameterName, "The height of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>(BestSolutionVariablesParameterName, "The number of variables used by the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TrainingBestScaledSymbolicRegressionSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

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

        // calculate scaling parameters and only for the best tree using the full training set
        double alpha, beta;
        SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(SymbolicExpressionTreeInterpreter, bestTree,
          lowerEstimationLimit, upperEstimationLimit,
          ProblemData.Dataset, targetVariable,
          ProblemData.TrainingIndizes, out beta, out alpha);

        // scale tree for solution
        var scaledTree = SymbolicRegressionSolutionLinearScaler.Scale(bestTree, alpha, beta);
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          scaledTree);
        var solution = new SymbolicRegressionSolution((DataAnalysisProblemData)ProblemData.Clone(), model, lowerEstimationLimit, upperEstimationLimit);
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
            Results[BestSolutionVariablesParameterName].Value = BestSolutionHeight;
          }
        }

        if (!Results.ContainsKey(BestSolutionQualityParameterName)) {
          Results.Add(new Result(BestSolutionQualityParameterName, BestSolutionQuality));
          Results.Add(new Result(BestSolutionParameterName, BestSolution));
        } else {
          Results[BestSolutionQualityParameterName].Value = BestSolutionQuality;
        }
      }
      #endregion
      return base.Apply();
    }
  }
}
