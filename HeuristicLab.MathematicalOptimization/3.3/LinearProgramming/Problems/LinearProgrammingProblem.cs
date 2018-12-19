#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  [Item("Linear/Mixed Integer Programming Problem (LP/MIP)", "Represents a linear/mixed integer problem.")]
  [StorableClass]
  public sealed class LinearProgrammingProblem : Problem {
    [Storable]
    private ILinearProgrammingProblemDefinition problemDefinition;

    public LinearProgrammingProblem() {
      Parameters.Remove(Parameters["Operators"]);
    }

    private LinearProgrammingProblem(LinearProgrammingProblem original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    private LinearProgrammingProblem(bool deserializing) : base(deserializing) {
    }

    public event EventHandler ProblemDefinitionChanged;

    public ILinearProgrammingProblemDefinition ProblemDefinition {
      get => problemDefinition;
      set {
        if (problemDefinition == value)
          return;
        problemDefinition = value;
        ProblemDefinitionChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) => new LinearProgrammingProblem(this, cloner);

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    private void OnProblemDefinitionChanged() {
      OnOperatorsChanged();
      OnReset();
    }
  }
}
