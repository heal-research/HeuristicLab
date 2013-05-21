#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Single-Objective OKB Problem", "Represents a single-objective problem which is stored in the OKB.")]
  [Creatable("Optimization Knowledge Base (OKB)")]
  [StorableClass]
  public sealed class SingleObjectiveOKBProblem : OKBProblem, ISingleObjectiveHeuristicOptimizationProblem, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }
    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return base.Problem as ISingleObjectiveHeuristicOptimizationProblem; }
    }
    public IParameter BestKnownQualityParameter {
      get { return Problem.BestKnownQualityParameter; }
    }
    public IParameter MaximizationParameter {
      get { return Problem.MaximizationParameter; }
    }
    public new ISingleObjectiveEvaluator Evaluator {
      get { return Problem.Evaluator; }
    }

    [StorableConstructor]
    private SingleObjectiveOKBProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveOKBProblem(SingleObjectiveOKBProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveOKBProblem() : base(new EmptySingleObjectiveProblem("No problem selected. Please choose a single-objective problem instance from the OKB.")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveOKBProblem(this, cloner);
    }
  }
}
