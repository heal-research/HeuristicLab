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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("Empty Single-Objective Problem", "A dummy single-objective problem which serves as a placeholder and cannot be solved.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class EmptySingleObjectiveProblem : SingleObjectiveHeuristicOptimizationProblem<EmptySingleObjectiveEvaluator, EmptySolutionCreator> {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private EmptySingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    private EmptySingleObjectiveProblem(EmptySingleObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public EmptySingleObjectiveProblem() : base(new EmptySingleObjectiveEvaluator(), new EmptySolutionCreator()) { }
    public EmptySingleObjectiveProblem(string exceptionMessage) : base(new EmptySingleObjectiveEvaluator(exceptionMessage), new EmptySolutionCreator(exceptionMessage)) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptySingleObjectiveProblem(this, cloner);
    }
  }
}
