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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionEvaluator", "Evaluates a symbolic regression solution.")]
  [StorableClass]
  public abstract class SymbolicRegressionEvaluator : SingleSuccessorOperator, ISymbolicRegressionEvaluator {
    private const string QualityParameterName = "Quality";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string FunctionTreeParameterName = "FunctionTree";
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    #region ISymbolicRegressionEvaluator Members

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }

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
    #endregion

    public SymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the evaluated symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(FunctionTreeParameterName, "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(RegressionProblemDataParameterName, "The problem data on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The start index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The end index of the dataset partition on which the symbolic regression solution should be evaluated."));
    }

    public override IOperation Apply() {
      QualityParameter.ActualValue = new DoubleValue(Evaluate(SymbolicExpressionTreeInterpreter, SymbolicExpressionTree, RegressionProblemData.Dataset,
        RegressionProblemData.TargetVariable, SamplesStart, SamplesEnd));
      return null;
    }

    protected abstract double Evaluate(ISymbolicExpressionTreeInterpreter interpreter,
      SymbolicExpressionTree solution,
      Dataset dataset,
      StringValue targetVariable,
      IntValue samplesStart, IntValue samplesEnd);
  }
}
