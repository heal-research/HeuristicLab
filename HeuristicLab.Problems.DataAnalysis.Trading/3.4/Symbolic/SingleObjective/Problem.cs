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
 */
#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  [Item("Symbolic Trading Problem (single-objective)", "Represents a single-objective symbolic trading problem.")]
  [StorableType("4E1FFD34-5720-4578-8D9E-AF657E157109")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class SingleObjectiveProblem : SymbolicDataAnalysisSingleObjectiveProblem<IProblemData, ISingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, IProblem {
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;

    [StorableConstructor]
    protected SingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveProblem(SingleObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SingleObjectiveProblem(this, cloner); }

    public SingleObjectiveProblem()
      : base(new ProblemData(), new SharpeRatioEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
      Maximization.Value = true;
      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      InitializeOperators();
      ConfigureGrammarSymbols();
    }

    private void ConfigureGrammarSymbols() {
      var grammar = SymbolicExpressionTreeGrammar as TypeCoherentExpressionGrammar;
      if (grammar != null) grammar.ConfigureAsDefaultTimeSeriesPrognosisGrammar();
    }

    private void InitializeOperators() {
      Operators.Add(new TrainingBestSolutionAnalyzer());
      Operators.Add(new ValidationBestSolutionAnalyzer());
      ParameterizeOperators();
    }
  }
}
