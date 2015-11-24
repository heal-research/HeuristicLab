#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public interface ISymbolicDataAnalysisProblem : IDataAnalysisProblem, IHeuristicOptimizationProblem {
    IValueParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter { get; }
    IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter { get; }
    IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter { get; }
    IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter { get; }
    IFixedValueParameter<IntValue> MaximumFunctionDefinitionsParameter { get; }
    IFixedValueParameter<IntValue> MaximumFunctionArgumentsParameter { get; }
    IFixedValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter { get; }
    IFixedValueParameter<IntRange> FitnessCalculationPartitionParameter { get; }
    IFixedValueParameter<IntRange> ValidationPartitionParameter { get; }

    ISymbolicDataAnalysisGrammar SymbolicExpressionTreeGrammar { get; set; }
    ISymbolicDataAnalysisExpressionTreeInterpreter SymbolicExpressionTreeInterpreter { get; set; }
    IntValue MaximumSymbolicExpressionTreeDepth { get; }
    IntValue MaximumSymbolicExpressionTreeLength { get; }
    IntValue MaximumFunctionDefinitions { get; }
    IntValue MaximumFunctionArguments { get; }
    PercentValue RelativeNumberOfEvaluatedSamples { get; }
    IntRange FitnessCalculationPartition { get; }
    IntRange ValidationPartition { get; }
  }

  public interface ISymbolicDataAnalysisSingleObjectiveProblem : ISymbolicDataAnalysisProblem, ISingleObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> { }
  public interface ISymbolicDataAnalysisMultiObjectiveProblem : ISymbolicDataAnalysisProblem, IMultiObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> { }
}
