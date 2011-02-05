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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SimpleSymbolicRegressionEvaluator", "Evaluates a symbolic regression solution and outputs a matrix of target and estimated values.")]
  [StorableClass]
  public sealed class SimpleSymbolicRegressionEvaluator : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string FunctionTreeParameterName = "FunctionTree";
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string ValuesParameterName = "Values";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region ISymbolicRegressionEvaluator Members
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }

    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[FunctionTreeParameterName]; }
    }

    public ILookupParameter<DataAnalysisProblemData> RegressionProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[RegressionProblemDataParameterName]; }
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

    public ILookupParameter<DoubleMatrix> ValuesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters[ValuesParameterName]; }
    }

    #endregion
    #region properties
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DataAnalysisProblemData RegressionProblemData {
      get { return RegressionProblemDataParameter.ActualValue; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }

    #endregion

    [StorableConstructor]
    private SimpleSymbolicRegressionEvaluator(bool deserializing) : base(deserializing) { }
    private SimpleSymbolicRegressionEvaluator(SimpleSymbolicRegressionEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SimpleSymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(FunctionTreeParameterName, "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(RegressionProblemDataParameterName, "The problem data on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The start index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The end index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new LookupParameter<DoubleMatrix>(ValuesParameterName, "The matrix of target and estimated values as generated by the symbolic regression solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleSymbolicRegressionEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      Dataset dataset = RegressionProblemData.Dataset;
      string targetVariable = RegressionProblemData.TargetVariable.Value;
      ISymbolicExpressionTreeInterpreter interpreter = SymbolicExpressionTreeInterpreter;
      SymbolicExpressionTree tree = SymbolicExpressionTree;
      int start = SamplesStart.Value;
      int end = SamplesEnd.Value;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      var estimatedValues = from x in interpreter.GetSymbolicExpressionTreeValues(tree, dataset, Enumerable.Range(start, end - start))
                            let boundedX = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, x))
                            select double.IsNaN(boundedX) ? upperEstimationLimit : boundedX;
      var originalValues = from row in Enumerable.Range(start, end - start) select dataset[row, targetVariableIndex];
      // NB: indexes must match SimpleEvaluator.ORIGINAL_INDEX and SimpleEvaluator.ESTIMATED_INDEX
      ValuesParameter.ActualValue = new DoubleMatrix(MatrixExtensions<double>.Create(originalValues.ToArray(), estimatedValues.ToArray()));
      return base.Apply();
    }
  }
}
