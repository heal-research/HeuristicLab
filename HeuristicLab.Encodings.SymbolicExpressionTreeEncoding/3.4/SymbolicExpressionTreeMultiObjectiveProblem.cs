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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("f4819c68-b6fc-469f-bcb5-cb5b2a9d8aff")]
  public abstract class SymbolicExpressionTreeMultiObjectiveProblem : MultiObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    // persistence
    [StorableConstructor]
    protected SymbolicExpressionTreeMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }


    // cloning
    protected SymbolicExpressionTreeMultiObjectiveProblem(SymbolicExpressionTreeMultiObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicExpressionTreeMultiObjectiveProblem(SymbolicExpressionTreeEncoding encoding)
      : base(encoding) {
      EncodingParameter.ReadOnly = true;
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[][] qualities, ResultCollection results,
      IRandom random) {
      base.Analyze(trees, qualities, results, random);

      var fronts = DominationCalculator.CalculateAllParetoFrontsIndices(trees, qualities, Maximization);
      var plot = new ParetoFrontScatterPlot<ISymbolicExpressionTree>(fronts, trees, qualities, Objectives, BestKnownFront);
      results.AddOrUpdateResult("Pareto Front Scatter Plot", plot);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualitiesParameter.ActualName;
      }
    }
  }
}
