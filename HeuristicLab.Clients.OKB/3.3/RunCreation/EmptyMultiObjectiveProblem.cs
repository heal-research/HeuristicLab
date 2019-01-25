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
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("Empty Multi-Objective Problem", "A dummy multi-objective problem which serves as a placeholder and cannot be solved.")]
  [StorableType("1AD8A6B9-1B3E-40BA-BAE2-8EAD31793B7D")]
  [NonDiscoverableType]
  public sealed class EmptyMultiObjectiveProblem : MultiObjectiveHeuristicOptimizationProblem<EmptyMultiObjectiveEvaluator, EmptySolutionCreator> {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private EmptyMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    private EmptyMultiObjectiveProblem(EmptyMultiObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public EmptyMultiObjectiveProblem() : base(new EmptyMultiObjectiveEvaluator(), new EmptySolutionCreator()) { }
    public EmptyMultiObjectiveProblem(string exceptionMessage) : base(new EmptyMultiObjectiveEvaluator(exceptionMessage), new EmptySolutionCreator(exceptionMessage)) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptyMultiObjectiveProblem(this, cloner);
    }
  }
}
