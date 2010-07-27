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
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic vector regression solution.
  /// </summary>
  [Item("ValidationBestScaledSymbolicVectorRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic vector regression solution.")]
  [StorableClass]
  public sealed class ValidationBestScaledSymbolicVectorRegressionSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ScaledSymbolicExpressionTreeParameterName = "ScaledSymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string TrainingSamplesStartParameterName = "TrainingSamplesStart";
    private const string TrainingSamplesEndParameterName = "TrainingSamplesEnd";
    private const string ValidationSamplesStartParameterName = "ValidationSamplesStart";
    private const string ValidationSamplesEndParameterName = "ValidationSamplesEnd";
    private const string TestSamplesStartParameterName = "TestSamplesStart";
    private const string TestSamplesEndParameterName = "TestSamplesEnd";
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
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> AlphaParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters[AlphaParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> BetaParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters[BetaParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
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
    public ILookupParameter<SymbolicExpressionTree> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[BestSolutionParameterName]; }
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
    #endregion
    #region properties
    public MultiVariateDataAnalysisProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    public ItemArray<DoubleArray> Alpha {
      get { return AlphaParameter.ActualValue; }
    }
    public ItemArray<DoubleArray> Beta {
      get { return BetaParameter.ActualValue; }
    }
    public DoubleArray LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public DoubleArray UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    #endregion

    public ValidationBestScaledSymbolicVectorRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(AlphaParameterName, "The alpha parameter for linear scaling."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(BetaParameterName, "The beta parameter for linear scaling."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<MultiVariateDataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleArray>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The best known (validation) quality achieved on the data set."));

    }

    public override IOperation Apply() {
      var trees = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<SymbolicExpressionTree> scaledTrees;
      if (Alpha.Length == trees.Length) {
        scaledTrees = from i in Enumerable.Range(0, trees.Length)
                      select SymbolicVectorRegressionSolutionLinearScaler.Scale(trees[i], Beta[i].ToArray(), Alpha[i].ToArray());
      } else {
        scaledTrees = trees;
      }
      IEnumerable<string> selectedTargetVariables = from item in ProblemData.TargetVariables.CheckedItems
                                                    select item.Value.Value;
      var interpreter = SymbolicExpressionTreeInterpreterParameter.ActualValue;
      int validationStart = ValidationSamplesStartParameter.ActualValue.Value;
      int validationEnd = ValidationSamplesEndParameter.ActualValue.Value;
      IEnumerable<int> rows = Enumerable.Range(validationStart, validationEnd - validationStart);
      SymbolicExpressionTree bestTree = null;
      double bestValidationError = double.PositiveInfinity;
      foreach (var tree in scaledTrees) {
        // calculate error on validation set
        double validationMse = SymbolicVectorRegressionNormalizedMseEvaluator.Calculate(tree, interpreter, ProblemData, selectedTargetVariables, rows, LowerEstimationLimit, UpperEstimationLimit);
        if (bestValidationError > validationMse) {
          bestValidationError = validationMse;
          bestTree = tree;
        }
      }
      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > bestValidationError) {
        var bestSolution = bestTree;

        //bestSolution.Name = BestSolutionParameterName;
        //solution.Description = "Best solution on validation partition found over the whole run.";

        BestSolutionParameter.ActualValue = bestSolution;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestValidationError);
      }

      // update results
      var results = ResultsParameter.ActualValue;
      if (!results.ContainsKey(BestSolutionQualityValuesParameterName)) {
        results.Add(new Result(BestSolutionParameterName, BestSolutionParameter.ActualValue));
        results.Add(new Result(BestSolutionQualityValuesParameterName, new DataTable(BestSolutionQualityValuesParameterName, BestSolutionQualityValuesParameterName)));
        results.Add(new Result(BestSolutionQualityParameterName, new DoubleValue()));
        results.Add(new Result(CurrentBestValidationQualityParameterName, new DoubleValue()));
      }
      results[BestSolutionParameterName].Value = BestSolutionParameter.ActualValue;
      results[BestSolutionQualityParameterName].Value = new DoubleValue(BestSolutionQualityParameter.ActualValue.Value);
      results[CurrentBestValidationQualityParameterName].Value = new DoubleValue(bestValidationError);

      DataTable validationValues = (DataTable)results[BestSolutionQualityValuesParameterName].Value;
      AddValue(validationValues, BestSolutionQualityParameter.ActualValue.Value, BestSolutionQualityParameterName, BestSolutionQualityParameterName);
      AddValue(validationValues, bestValidationError, CurrentBestValidationQualityParameterName, CurrentBestValidationQualityParameterName);

      return base.Apply();
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
