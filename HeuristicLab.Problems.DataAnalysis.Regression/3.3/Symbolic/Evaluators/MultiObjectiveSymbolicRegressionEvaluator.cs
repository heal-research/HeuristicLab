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

using System.Collections.Generic;
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
  [Item("MultiObjectiveSymbolicRegressionEvaluator", "Evaluates a symbolic regression solution.")]
  [StorableClass]
  public abstract class MultiObjectiveSymbolicRegressionEvaluator : SingleSuccessorOperator, IMultiObjectiveSymbolicRegressionEvaluator {
    private const string RandomParameterName = "Random";
    private const string QualitiesParameterName = "Qualities";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string FunctionTreeParameterName = "FunctionTree";
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    #region ISymbolicRegressionEvaluator Members

    public ILookupParameter<DoubleArray> QualitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[QualitiesParameterName]; }
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

    public IValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }

    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
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

    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiObjectiveSymbolicRegressionEvaluator(bool deserializing) : base(deserializing) { }
    protected MultiObjectiveSymbolicRegressionEvaluator(MultiObjectiveSymbolicRegressionEvaluator original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveSymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<DoubleArray>(QualitiesParameterName, "The qualities of the evaluated symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(FunctionTreeParameterName, "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(RegressionProblemDataParameterName, "The problem data on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The start index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The end index of the dataset partition on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new ValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index.", new PercentValue(1)));
    }

    public override IOperation Apply() {
      int seed = Random.Next();
      IEnumerable<int> rows = SingleObjectiveSymbolicRegressionEvaluator.GenerateRowsToEvaluate(seed, RelativeNumberOfEvaluatedSamples.Value, SamplesStart.Value, SamplesEnd.Value)
         .Where(i => i < RegressionProblemData.TestSamplesStart.Value || RegressionProblemData.TestSamplesEnd.Value <= i);
      double[] qualities = Evaluate(SymbolicExpressionTreeInterpreter, SymbolicExpressionTree, RegressionProblemData.Dataset,
        RegressionProblemData.TargetVariable, rows);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.Apply();
    }


    protected abstract double[] Evaluate(ISymbolicExpressionTreeInterpreter interpreter,
      SymbolicExpressionTree solution,
      Dataset dataset,
      StringValue targetVariable,
      IEnumerable<int> rows);
  }
}
