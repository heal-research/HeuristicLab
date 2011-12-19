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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public abstract class SymbolicDataAnalysisEvaluator<T> : SingleSuccessorOperator,
    ISymbolicDataAnalysisEvaluator<T>, ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator
  where T : class, IDataAnalysisProblemData {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EvaluationPartitionParameterName = "EvaluationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    public override bool CanChangeName { get { return false; } }

    #region parameter properties
    public IValueLookupParameter<IRandom> RandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<T> ProblemDataParameter {
      get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
    }

    public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[EvaluationPartitionParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    #endregion


    [StorableConstructor]
    protected SymbolicDataAnalysisEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisEvaluator(SymbolicDataAnalysisEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic data analysis solution encoded as a symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntRange>(EvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The upper and lower limit that should be used as cut off value for the output values of symbolic data analysis trees."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
    }

    protected IEnumerable<int> GenerateRowsToEvaluate() {
      return GenerateRowsToEvaluate(RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value);
    }

    protected IEnumerable<int> GenerateRowsToEvaluate(double percentageOfRows) {


      IEnumerable<int> rows;
      int samplesStart = EvaluationPartitionParameter.ActualValue.Start;
      int samplesEnd = EvaluationPartitionParameter.ActualValue.End;
      int testPartitionStart = ProblemDataParameter.ActualValue.TestPartition.Start;
      int testPartitionEnd = ProblemDataParameter.ActualValue.TestPartition.End;

      if (samplesEnd < samplesStart) throw new ArgumentException("Start value is larger than end value.");

      if (percentageOfRows.IsAlmost(1.0))
        rows = Enumerable.Range(samplesStart, samplesEnd - samplesStart);
      else {
        int seed = RandomParameter.ActualValue.Next();
        int count = (int)((samplesEnd - samplesStart) * percentageOfRows);
        if (count == 0) count = 1;
        rows = RandomEnumerable.SampleRandomNumbers(seed, samplesStart, samplesEnd, count);
      }

      return rows.Where(i => i < testPartitionStart || testPartitionEnd <= i);
    }
  }
}
