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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Multi-Objective Heuristic Optimization Problem", "A base class for multi-objective heuristic optimization problems.")]
  [StorableType("C46643E3-7144-4884-A30A-5329BD80DC4E")]
  public abstract class MultiObjectiveHeuristicOptimizationProblem<T> : HeuristicOptimizationProblem<T>, IMultiObjectiveHeuristicOptimizationProblem
    where T : class, IMultiObjectiveEvaluator {
    private const string MaximizationParameterName = "Maximization";

    [StorableConstructor]
    protected MultiObjectiveHeuristicOptimizationProblem(StorableConstructorFlag _) : base(_) { }
    protected MultiObjectiveHeuristicOptimizationProblem(MultiObjectiveHeuristicOptimizationProblem<T> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    protected MultiObjectiveHeuristicOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Determines for each objective whether it should be maximized or minimized."));

      RegisterEventHandlers();
    }

    protected MultiObjectiveHeuristicOptimizationProblem(T evaluator)
      : base(evaluator) {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Determines for each objective whether it should be maximized or minimized."));

      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      MaximizationParameter.ValueChanged += MaximizationParameterOnValueChanged;
    }

    private void MaximizationParameterOnValueChanged(object sender, EventArgs e) {
      OnMaximizationChanged();
    }

    public ValueParameter<BoolArray> MaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters[MaximizationParameterName]; }
    }
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public BoolArray Maximization {
      get { return MaximizationParameter.Value; }
      protected set { MaximizationParameter.Value = value; }
    }

    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }

    public event EventHandler MaximizationChanged;
    protected void OnMaximizationChanged() {
      MaximizationChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
