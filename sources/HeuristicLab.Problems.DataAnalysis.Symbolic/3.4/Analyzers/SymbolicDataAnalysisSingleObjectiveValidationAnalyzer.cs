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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for symbolic data analysis analyzers that validate a solution on a separate data partition using the evaluator.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U> : SymbolicDataAnalysisSingleObjectiveAnalyzer,
    ISymbolicDataAnalysisValidationAnalyzer<T, U>
    where T : class, ISymbolicDataAnalysisSingleObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string ProblemDataParameterName = "ProblemData";
    private const string EvaluatorParameterName = "Evaluator";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";

    #region parameter properties
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
    protected SymbolicDataAnalysisSingleObjectiveValidationAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisSingleObjectiveValidationAnalyzer(SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisSingleObjectiveValidationAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<U>(ProblemDataParameterName, "The problem data of the symbolic data analysis problem."));
      Parameters.Add(new LookupParameter<T>(EvaluatorParameterName, "The operator to use for fitness evaluation on the validation partition."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter for symbolic data analysis expression trees."));
      Parameters.Add(new ValueLookupParameter<IntRange>(ValidationPartitionParameterName, "Thes validation partition."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
    }
  }
}
