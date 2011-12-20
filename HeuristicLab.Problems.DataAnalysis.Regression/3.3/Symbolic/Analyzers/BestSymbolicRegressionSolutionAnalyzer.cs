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

using System.Linq;
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
  [Item("BestSymbolicRegressionSolutionAnalyzer", "An operator for analyzing the best solution of symbolic regression problems given in symbolic expression tree encoding.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class BestSymbolicRegressionSolutionAnalyzer : RegressionSolutionAnalyzer, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string BestSolutionInputvariableCountResultName = "Variables used by best solution";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string VariableImpactsResultName = "Integrated variable frequencies";
    private const string BestSolutionParameterName = "BestSolution";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<SymbolicRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<DataTable> VariableFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[VariableFrequenciesParameterName]; }
    }
    #endregion
    #region properties
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DataTable VariableFrequencies {
      get { return VariableFrequenciesParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private BestSymbolicRegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestSymbolicRegressionSolutionAnalyzer(BestSymbolicRegressionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSymbolicRegressionSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(VariableFrequenciesParameterName)) {
        Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The variable frequencies table to use for the calculation of variable impacts"));
      }
    }

    protected override DataAnalysisSolution UpdateBestSolution() {
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      int i = Quality.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > Quality[i].Value) {
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          SymbolicExpressionTree[i]);
        DataAnalysisProblemData problemDataClone = (DataAnalysisProblemData)ProblemData.Clone();
        var solution = new SymbolicRegressionSolution(problemDataClone, model, lowerEstimationLimit, upperEstimationLimit);
        solution.Name = BestSolutionParameterName;
        solution.Description = "Best solution on validation partition found over the whole run.";
        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = Quality[i];
        BestSymbolicRegressionSolutionAnalyzer.UpdateSymbolicRegressionBestSolutionResults(solution, problemDataClone, Results, VariableFrequencies);
      }
      return BestSolutionParameter.ActualValue;
    }

    public static void UpdateBestSolutionResults(SymbolicRegressionSolution solution, DataAnalysisProblemData problemData, ResultCollection results, IntValue currentGeneration, DataTable variableFrequencies) {
      RegressionSolutionAnalyzer.UpdateBestSolutionResults(solution, problemData, results, currentGeneration);
      UpdateSymbolicRegressionBestSolutionResults(solution, problemData, results, variableFrequencies);
    }

    private static void UpdateSymbolicRegressionBestSolutionResults(SymbolicRegressionSolution solution, DataAnalysisProblemData problemData, ResultCollection results, DataTable variableFrequencies) {
      if (results.ContainsKey(BestSolutionInputvariableCountResultName)) {
        results[BestSolutionInputvariableCountResultName].Value = new IntValue(solution.Model.InputVariables.Count());
        results[VariableImpactsResultName].Value = CalculateVariableImpacts(variableFrequencies);
      } else {
        results.Add(new Result(BestSolutionInputvariableCountResultName, new IntValue(solution.Model.InputVariables.Count())));
        results.Add(new Result(VariableImpactsResultName, CalculateVariableImpacts(variableFrequencies)));
      }
    }


    private static DoubleMatrix CalculateVariableImpacts(DataTable variableFrequencies) {
      if (variableFrequencies != null) {
        var impacts = new DoubleMatrix(variableFrequencies.Rows.Count, 1, new string[] { "Impact" }, variableFrequencies.Rows.Select(x => x.Name));
        impacts.SortableView = true;
        int rowIndex = 0;
        foreach (var dataRow in variableFrequencies.Rows) {
          string variableName = dataRow.Name;
          impacts[rowIndex++, 0] = dataRow.Values.Average();
        }
        return impacts;
      } else return new DoubleMatrix(1, 1);
    }
  }
}
