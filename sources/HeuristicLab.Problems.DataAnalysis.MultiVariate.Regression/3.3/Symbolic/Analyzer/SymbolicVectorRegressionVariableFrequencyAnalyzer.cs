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


namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Analyzers {
  [Item("SymbolicVectorRegressionVariableFrequencyAnalyzer", "An operator for analyzing the variable frequencies of symbolic vector regression solutions given in symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class SymbolicVectorRegressionVariableFrequencyAnalyzer : SingleSuccessorOperator, IAnalyzer {
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
    public ILookupParameter<MultiVariateDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<MultiVariateDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
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

    public SymbolicVectorRegressionVariableFrequencyAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<MultiVariateDataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<DataTable>(VariableFrequenciesParameterName, "The data table to store the variable frequencies."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      MultiVariateDataAnalysisProblemData problemData = ProblemDataParameter.ActualValue;
      var inputVariables = problemData.InputVariables.Select(x => x.Value);
      ResultCollection results = ResultsParameter.ActualValue;

      if (VariableFrequencies == null) {
        VariableFrequencies = new DataTable("Variable frequencies", "Relative frequency of variable references aggregated over the whole population.");
        // add a data row for each input variable
        foreach (var inputVariable in inputVariables)
          VariableFrequencies.Rows.Add(new DataRow(inputVariable));
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
