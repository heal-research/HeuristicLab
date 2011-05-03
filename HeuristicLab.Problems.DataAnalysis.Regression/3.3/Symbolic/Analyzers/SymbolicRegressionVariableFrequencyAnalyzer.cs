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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  [Item("SymbolicRegressionVariableFrequencyAnalyzer", "An operator for analyzing the variable frequencies of symbolic regression solutions given in symbolic expression tree encoding.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicRegressionVariableFrequencyAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ProblemDataParameterName = "ProblemData";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string ResultsParameterName = "Results";

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DataTable> VariableFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[VariableFrequenciesParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public DataTable VariableFrequencies {
      get { return VariableFrequenciesParameter.ActualValue; }
      set { VariableFrequenciesParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionVariableFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionVariableFrequencyAnalyzer(SymbolicRegressionVariableFrequencyAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionVariableFrequencyAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<DataTable>(VariableFrequenciesParameterName, "The data table to store the variable frequencies."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionVariableFrequencyAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      DataAnalysisProblemData problemData = ProblemDataParameter.ActualValue;
      var inputVariables = problemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      ResultCollection results = ResultsParameter.ActualValue;

      if (VariableFrequencies == null) {
        VariableFrequencies = new DataTable("Variable frequencies", "Relative frequency of variable references aggregated over the whole population.");
        VariableFrequencies.VisualProperties.XAxisTitle = "Generation";
        VariableFrequencies.VisualProperties.YAxisTitle = "Relative Variable Frequency";
        // add a data row for each input variable
        foreach (var inputVariable in inputVariables) {
          DataRow row = new DataRow(inputVariable);
          row.VisualProperties.StartIndexZero = true;
          VariableFrequencies.Rows.Add(row);
        }
        results.Add(new Result("Variable frequencies", VariableFrequencies));
      }
      foreach (var pair in VariableFrequencyAnalyser.CalculateVariableFrequencies(expressions, inputVariables)) {
        VariableFrequencies.Rows[pair.Key].Values.Add(pair.Value);
        results["Variable frequencies"].Value = VariableFrequencies;
      }

      return base.Apply();
    }
  }
}
