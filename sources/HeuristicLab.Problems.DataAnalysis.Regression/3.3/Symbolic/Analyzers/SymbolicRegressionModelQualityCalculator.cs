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
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// "An operator to calculate the quality values of a symbolic regression solution symbolic expression tree encoding."
  /// </summary>
  [Item("SymbolicRegressionModelQualityCalculator", "An operator to calculate the quality values of a symbolic regression solution symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class SymbolicRegressionModelQualityCalculator : AlgorithmOperator {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ValuesParameterName = "Values";
    private const string RSQuaredQualityParameterName = "R-squared";
    private const string MeanSquaredErrorQualityParameterName = "Mean Squared Error";
    private const string RelativeErrorQualityParameterName = "Relative Error";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region parameter properties
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
    public ILookupParameter<DoubleValue> RSquaredQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[RSQuaredQualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> AverageRelativeErrorQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[RelativeErrorQualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> MeanSquaredErrorQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[MeanSquaredErrorQualityParameterName]; }
    }
    #endregion

    public SymbolicRegressionModelQualityCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree to analyze."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueParameter<DoubleMatrix>(ValuesParameterName, "The matrix of original target values and estimated values of the model."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(MeanSquaredErrorQualityParameterName, "The mean squared error value of the output of the model."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(RSQuaredQualityParameterName, "The R� correlation coefficient of the output of the model and the original target values."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(RelativeErrorQualityParameterName, "The average relative percentage error of the output of the model."));
      #region operator initialization
      SimpleSymbolicRegressionEvaluator simpleEvaluator = new SimpleSymbolicRegressionEvaluator();
      SimpleRSquaredEvaluator simpleR2Evalator = new SimpleRSquaredEvaluator();
      SimpleMeanAbsolutePercentageErrorEvaluator simpleRelErrorEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      SimpleMSEEvaluator simpleMseEvaluator = new SimpleMSEEvaluator();
      Assigner clearValues = new Assigner();
      #endregion

      #region parameter wiring
      simpleEvaluator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      simpleEvaluator.RegressionProblemDataParameter.ActualName = ProblemDataParameter.Name;
      simpleEvaluator.SamplesStartParameter.ActualName = SamplesStartParameter.Name;
      simpleEvaluator.SamplesEndParameter.ActualName = SamplesEndParameter.Name;
      simpleEvaluator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      simpleEvaluator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      simpleEvaluator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      simpleEvaluator.ValuesParameter.ActualName = ValuesParameterName;

      simpleR2Evalator.ValuesParameter.ActualName = ValuesParameterName;
      simpleR2Evalator.RSquaredParameter.ActualName = RSquaredQualityParameter.Name;

      simpleMseEvaluator.ValuesParameter.ActualName = ValuesParameterName;
      simpleMseEvaluator.MeanSquaredErrorParameter.ActualName = MeanSquaredErrorQualityParameter.Name;

      simpleRelErrorEvaluator.ValuesParameter.ActualName = ValuesParameterName;
      simpleRelErrorEvaluator.AverageRelativeErrorParameter.ActualName = AverageRelativeErrorQualityParameter.Name;

      clearValues.LeftSideParameter.ActualName = ValuesParameterName;
      clearValues.RightSideParameter.Value = new DoubleMatrix();
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = simpleEvaluator;
      simpleEvaluator.Successor = simpleR2Evalator;
      simpleR2Evalator.Successor = simpleRelErrorEvaluator;
      simpleRelErrorEvaluator.Successor = simpleMseEvaluator;
      simpleMseEvaluator.Successor = clearValues;
      clearValues.Successor = null;
      #endregion

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
