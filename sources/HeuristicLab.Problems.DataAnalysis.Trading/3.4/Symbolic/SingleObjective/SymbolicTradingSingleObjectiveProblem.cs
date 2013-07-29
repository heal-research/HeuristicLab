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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Trading {
  [Item("Symbolic Trading Problem (single objective)", "Represents a single objective symbolic trading problem.")]
  [StorableClass]
  [Creatable("Problems")]
  public class SymbolicTradingSingleObjectiveProblem : SymbolicDataAnalysisSingleObjectiveProblem<ITradingProblemData, ISymbolicTradingSingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, ITradingProblem {
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;

    [StorableConstructor]
    protected SymbolicTradingSingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected SymbolicTradingSingleObjectiveProblem(SymbolicTradingSingleObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicTradingSingleObjectiveProblem(this, cloner); }

    public SymbolicTradingSingleObjectiveProblem()
      : base(new TradingProblemData(), new SymbolicTradingSingleObjectiveSharpeRatioEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
      Maximization.Value = true;
      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      InitializeOperators();
    }

    private void InitializeOperators() {
      Operators.Add(new SymbolicTradingSingleObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicTradingSingleObjectiveValidationBestSolutionAnalyzer());
      ParameterizeOperators();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
    }

    public override void ImportProblemDataFromFile(string fileName) {
      TradingProblemData problemData = TradingProblemData.ImportFromFile(fileName);
      ProblemData = problemData;
    }
  }
}
