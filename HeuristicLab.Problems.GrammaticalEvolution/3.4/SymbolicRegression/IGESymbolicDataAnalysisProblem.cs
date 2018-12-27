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
 * 
 * Author: Sabine Winkler
 */

#endregion

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  public interface IGESymbolicDataAnalysisProblem : IDataAnalysisProblem, IHeuristicOptimizationProblem {

    IValueParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter { get; }
    IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter { get; }
    IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter { get; }
    IFixedValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter { get; }
    IFixedValueParameter<IntRange> FitnessCalculationPartitionParameter { get; }
    IFixedValueParameter<IntRange> ValidationPartitionParameter { get; }

    ISymbolicDataAnalysisGrammar SymbolicExpressionTreeGrammar { get; set; }
    ISymbolicDataAnalysisExpressionTreeInterpreter SymbolicExpressionTreeInterpreter { get; set; }
    IntValue MaximumSymbolicExpressionTreeLength { get; }
    PercentValue RelativeNumberOfEvaluatedSamples { get; }
    IntRange FitnessCalculationPartition { get; }
    IntRange ValidationPartition { get; }
  }

  public interface IGESymbolicDataAnalysisSingleObjectiveProblem : IGESymbolicDataAnalysisProblem, ISingleObjectiveHeuristicOptimizationProblem { }
}
