#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;
using HEAL.Attic;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableType("25134297-7d7e-4d77-bd7a-25b2b10e15c1")]
  public interface IGESymbolicDataAnalysisEvaluator<T> : IEvaluator
    where T : class, IDataAnalysisProblemData {

    ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter { get; }  // phenotype
    IValueLookupParameter<IntRange> EvaluationPartitionParameter { get; }
    IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter { get; }
    ILookupParameter<BoolValue> ApplyLinearScalingParameter { get; }

    IValueLookupParameter<T> ProblemDataParameter { get; }

    ILookupParameter<IntegerVector> IntegerVectorParameter { get; } // genotype
    ILookupParameter<IGenotypeToPhenotypeMapper> GenotypeToPhenotypeMapperParameter { get; }
    IValueLookupParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter { get; }
  }
}
