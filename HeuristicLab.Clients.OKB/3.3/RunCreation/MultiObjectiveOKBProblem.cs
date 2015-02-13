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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("Multi-Objective OKB Problem", "Represents a multi-objective problem which is stored in the OKB.")]
  [Creatable("Optimization Knowledge Base (OKB)")]
  [StorableClass]
  public sealed class MultiObjectiveOKBProblem : OKBProblem, IMultiObjectiveHeuristicOptimizationProblem, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType {
      get { return typeof(IMultiObjectiveHeuristicOptimizationProblem); }
    }
    public new IMultiObjectiveHeuristicOptimizationProblem Problem {
      get { return base.Problem as IMultiObjectiveHeuristicOptimizationProblem; }
    }
    public IParameter MaximizationParameter {
      get { return Problem.MaximizationParameter; }
    }
    public new IMultiObjectiveEvaluator Evaluator {
      get { return Problem.Evaluator; }
    }

    [StorableConstructor]
    private MultiObjectiveOKBProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveOKBProblem(MultiObjectiveOKBProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveOKBProblem() : base(new EmptyMultiObjectiveProblem("No problem selected. Please choose a multi-objective problem instance from the OKB.")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveOKBProblem(this, cloner);
    }
  }
}
