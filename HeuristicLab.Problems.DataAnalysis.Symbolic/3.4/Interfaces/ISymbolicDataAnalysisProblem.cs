#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("17dc51c4-499c-49dc-b3ac-72364358f7e7")]
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

  [StorableType("07b08ca0-40cb-433e-8aed-72df06a87d62")]
  public interface ISymbolicDataAnalysisSingleObjectiveProblem : ISymbolicDataAnalysisProblem, ISingleObjectiveHeuristicOptimizationProblem { }

  [StorableType("1c5a0cf4-1286-45d8-b126-a6f5ddccf7bf")]
  public interface ISymbolicDataAnalysisMultiObjectiveProblem : ISymbolicDataAnalysisProblem, IMultiObjectiveHeuristicOptimizationProblem { }
}
