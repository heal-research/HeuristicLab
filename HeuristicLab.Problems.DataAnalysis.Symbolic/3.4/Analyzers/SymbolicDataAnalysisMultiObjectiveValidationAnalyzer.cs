#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for symbolic data analysis analyzers that validate a solution on a separate data partition using the evaluator.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisMultiObjectiveValidationAnalyzer<T, U> : SymbolicDataAnalysisMultiObjectiveAnalyzer,
    ISymbolicDataAnalysisValidationAnalyzer<T, U>
    where T : class, ISymbolicDataAnalysisMultiObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string RandomParameterName = "Random";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EvaluatorParameterName = "Evaluator";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<U> ProblemDataParameter {
      get { return (ILookupParameter<U>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<T> EvaluatorParameter {
      get { return (ILookupParameter<T>)Parameters[EvaluatorParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<IntRange> ValidationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[ValidationPartitionParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveValidationAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisMultiObjectiveValidationAnalyzer(SymbolicDataAnalysisMultiObjectiveValidationAnalyzer<T, U> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisMultiObjectiveValidationAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new LookupParameter<U>(ProblemDataParameterName, "The problem data of the symbolic data analysis problem."));
      Parameters.Add(new LookupParameter<T>(EvaluatorParameterName, "The operator to use for fitness evaluation on the validation partition."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter for symbolic data analysis expression trees."));
      Parameters.Add(new ValueLookupParameter<IntRange>(ValidationPartitionParameterName, "The validation partition."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
    }

    protected IEnumerable<int> GenerateRowsToEvaluate() {
      int seed = RandomParameter.ActualValue.Next();
      int samplesStart = ValidationPartitionParameter.ActualValue.Start;
      int samplesEnd = ValidationPartitionParameter.ActualValue.End;
      int testPartitionStart = ProblemDataParameter.ActualValue.TestPartition.Start;
      int testPartitionEnd = ProblemDataParameter.ActualValue.TestPartition.End;

      if (samplesEnd < samplesStart) throw new ArgumentException("Start value is larger than end value.");
      int count = (int)((samplesEnd - samplesStart) * RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value);
      if (count == 0) count = 1;
      return RandomEnumerable.SampleRandomNumbers(seed, samplesStart, samplesEnd, count)
        .Where(i => i < testPartitionStart || testPartitionEnd <= i);
    }
  }
}
