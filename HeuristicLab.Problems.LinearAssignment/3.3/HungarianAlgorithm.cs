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

using System;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.LinearAssignment {
  /// <summary>
  /// A genetic algorithm.
  /// </summary>
  [Item("Hungarian Algorithm", "The Hungarian algorithm can be used to solve the linear assignment problem in O(n^3). It is also known as the Kuhn–Munkres algorithm or Munkres assignment algorithm.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 170)]
  [StorableType("2A2C1C1B-E0C3-4757-873B-C3F7C6D11A01")]
  public sealed class HungarianAlgorithm : BasicAlgorithm {
    public override bool SupportsPause => false;

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(LinearAssignmentProblem); }
    }
    public new LinearAssignmentProblem Problem {
      get { return (LinearAssignmentProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    [StorableConstructor]
    private HungarianAlgorithm(StorableConstructorFlag _) : base(_) { }
    private HungarianAlgorithm(HungarianAlgorithm original, Cloner cloner) : base(original, cloner) { }
    public HungarianAlgorithm()
      : base() {
      Problem = new LinearAssignmentProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HungarianAlgorithm(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      var assignment = LinearAssignmentProblemSolver.Solve(Problem.Costs, out double quality);

      var context = new SingleObjectiveSolutionContext<Permutation>(new Permutation(PermutationTypes.Absolute, assignment));
      context.EvaluationResult = new SingleObjectiveEvaluationResult(quality);

      var random = new FastRandom(42);
      Problem.Analyze(new[] { context }, random);
    }
  }
}
