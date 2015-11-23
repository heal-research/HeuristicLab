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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicExpressionTreePhenotypicSimilarityCalculator", "An operator that calculates the similarity betweeon two trees based on the correlation of their outputs.")]
  [StorableClass]
  public class SymbolicExpressionTreePhenotypicSimilarityCalculator : SolutionSimilarityCalculator {
    [Storable]
    public IDataAnalysisProblemData ProblemData { get; set; }
    [Storable]
    public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter { get; set; }

    protected override bool IsCommutative { get { return true; } }

    [StorableConstructor]
    protected SymbolicExpressionTreePhenotypicSimilarityCalculator(bool deserializing) : base(deserializing) { }

    public SymbolicExpressionTreePhenotypicSimilarityCalculator(SymbolicExpressionTreePhenotypicSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      this.ProblemData = cloner.Clone(original.ProblemData);
      this.Interpreter = cloner.Clone(original.Interpreter);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreePhenotypicSimilarityCalculator(this, cloner);
    }

    public SymbolicExpressionTreePhenotypicSimilarityCalculator() { }

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2) {
      if (Interpreter == null || ProblemData == null)
        throw new InvalidOperationException("Cannot calculate phenotypic similarity when no interpreter or problem data were set.");

      var v1 = Interpreter.GetSymbolicExpressionTreeValues(t1, ProblemData.Dataset, ProblemData.TrainingIndices);
      var v2 = Interpreter.GetSymbolicExpressionTreeValues(t2, ProblemData.Dataset, ProblemData.TrainingIndices);

      if (v1.Variance().IsAlmost(0) && v2.Variance().IsAlmost(0))
        return 1.0;

      OnlineCalculatorError error;
      var r = OnlinePearsonsRCalculator.Calculate(v1, v2, out error);

      var r2 = error == OnlineCalculatorError.None ? r * r : 0;

      if (r2 > 1.0)
        r2 = 1.0;

      return r2;
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      if (leftSolution == rightSolution)
        return 1.0;

      if (!leftSolution.Variables.ContainsKey("EstimatedValues") || !rightSolution.Variables.ContainsKey("EstimatedValues"))
        throw new ArgumentException("No estimated values are present in the subscopes.");

      var leftValues = (DoubleArray)leftSolution.Variables["EstimatedValues"].Value;
      var rightValues = (DoubleArray)rightSolution.Variables["EstimatedValues"].Value;

      if (leftValues.Variance().IsAlmost(0) && rightValues.Variance().IsAlmost(0))
        return 1.0;

      OnlineCalculatorError error;
      var r = OnlinePearsonsRCalculator.Calculate(leftValues, rightValues, out error);

      var r2 = error == OnlineCalculatorError.None ? r * r : 0;

      if (r2 > 1.0)
        r2 = 1.0;

      return r2;
    }
  }
}
