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

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that calculates the mean squared error of a symbolic regression solution encoded as a symbolic expression tree.
  /// </summary>
  [Item("SymbolicRegressionMeanSquaredErrorCalculator", "An operator that calculates the mean squared error of a symbolic regression solution encoded as a symbolic expression tree.")]
  [StorableClass]
  public sealed class SymbolicRegressionMeanSquaredErrorCalculator : AlgorithmOperator {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ProblemDataParameterName = "ProblemData";
    private const string QualityParameterName = "Mean Squared Error";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }

    public SymbolicRegressionMeanSquaredErrorCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree to analyze."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(QualityParameterName, "The mean squared error value of the output of the model."));


      SymbolicRegressionMeanSquaredErrorEvaluator evaluator = new SymbolicRegressionMeanSquaredErrorEvaluator();
      evaluator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      evaluator.QualityParameter.ActualName = QualityParameter.Name;
      evaluator.RegressionProblemDataParameter.ActualName = ProblemDataParameter.Name;
      evaluator.SamplesEndParameter.ActualName = SamplesEndParameter.Name;
      evaluator.SamplesStartParameter.ActualName = SamplesStartParameter.Name;
      evaluator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      evaluator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;

      OperatorGraph.InitialOperator = evaluator;
      evaluator.Successor = null;
    }

    // need to create custom operations for each solution scope (this has to be adapted on basis of the depth value of SymbolicExpressionTreeParameter)
    public override IOperation Apply() {
      var scopes = GetScopesOnLevel(ExecutionContext.Scope, SymbolicExpressionTreeParameter.Depth);
      OperationCollection operations = new OperationCollection();
      foreach (IScope treeScopes in scopes) {
        operations.Add(ExecutionContext.CreateChildOperation(OperatorGraph.InitialOperator, treeScopes));
      }
      if (Successor != null) operations.Add(ExecutionContext.CreateOperation(Successor));
      return operations;
    }

    private IEnumerable<IScope> GetScopesOnLevel(IScope scope, int d) {
      if (d == 0) yield return scope;
      else {
        foreach (IScope subScope in scope.SubScopes) {
          foreach (IScope scopesOfSubScope in GetScopesOnLevel(subScope, d - 1)) {
            yield return scopesOfSubScope;
          }
        }
      }
    }
  }
}
