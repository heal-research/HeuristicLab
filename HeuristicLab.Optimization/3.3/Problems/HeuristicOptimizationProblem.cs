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
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Heuristic Optimization Problem", "Represents the base class for a heuristic optimization problem.")]
  [StorableType("DE0478BA-3797-4AC3-9A89-3734D2643823")]
  public abstract class HeuristicOptimizationProblem<T> : EncodedProblem, IHeuristicOptimizationProblem
    where T : class, IEvaluator {
    private const string EvaluatorParameterName = "Evaluator";

    [StorableConstructor]
    protected HeuristicOptimizationProblem(StorableConstructorFlag _) : base(_) { }
    protected HeuristicOptimizationProblem(HeuristicOptimizationProblem<T> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected HeuristicOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<T>(EvaluatorParameterName, "The operator used to evaluate a solution."));
      RegisterEventHandlers();
    }

    protected HeuristicOptimizationProblem(T evaluator)
      : base() {
      Parameters.Add(new ValueParameter<T>(EvaluatorParameterName, "The operator used to evaluate a solution.", evaluator));
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    #region properties
    public T Evaluator {
      get { return EvaluatorParameter.Value; }
      protected set { EvaluatorParameter.Value = value; }
    }
    public ValueParameter<T> EvaluatorParameter {
      get { return (ValueParameter<T>)Parameters[EvaluatorParameterName]; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator { get { return Evaluator; } }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter { get { return EvaluatorParameter; } }
    #endregion

    #region events
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged();
    }
    public event EventHandler EvaluatorChanged;
    protected virtual void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
