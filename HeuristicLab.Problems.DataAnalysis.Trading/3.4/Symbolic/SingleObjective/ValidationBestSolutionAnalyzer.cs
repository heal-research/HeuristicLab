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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  /// <summary>
  /// An operator that analyzes the validation best symbolic trading solution for single objective symbolic trading problems.
  /// </summary>
  [Item("Validation-best Solution Analyzer (symbolic trading)", "An operator that analyzes the validation best symbolic trading solution for single objective symbolic trading problems.")]
  [StorableType("BBDC9398-4D40-4170-B22F-77E36C68709F")]
  public sealed class ValidationBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<ISolution, ISingleObjectiveEvaluator, IProblemData> {

    [StorableConstructor]
    private ValidationBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private ValidationBestSolutionAnalyzer(ValidationBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public ValidationBestSolutionAnalyzer()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ValidationBestSolutionAnalyzer(this, cloner);
    }

    protected override ISolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality) {
      var model = new Model((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue);
      return new SymbolicSolution(model, (IProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
