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
using HEAL.Fossil;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("Empty Problem", "A dummy problem which serves as a placeholder and cannot be solved.")]
  [StorableType("10132D57-7177-42A2-97B9-B77D89AADE6A")]
  [NonDiscoverableType]
  public sealed class EmptyProblem : HeuristicOptimizationProblem<EmptyEvaluator, EmptySolutionCreator> {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private EmptyProblem(StorableConstructorFlag _) : base(_) { }
    private EmptyProblem(EmptyProblem original, Cloner cloner) : base(original, cloner) { }
    public EmptyProblem()
      : base() {
      SolutionCreator = new EmptySolutionCreator();
      Evaluator = new EmptyEvaluator();
    }
    public EmptyProblem(string exceptionMessage)
      : base() {
      SolutionCreator = new EmptySolutionCreator(exceptionMessage);
      Evaluator = new EmptyEvaluator(exceptionMessage);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EmptyProblem(this, cloner);
    }
  }
}
