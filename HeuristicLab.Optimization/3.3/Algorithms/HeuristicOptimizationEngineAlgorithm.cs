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
  /// <summary>
  /// A base class for heuristic optimization algorithms using an execution engine.
  /// </summary>
  [Item("Heuristic Optimization Enigne Algorithm", "A base class for heuristic optimization algorithms using an execution engine.")]
  [StorableType("A741CA8C-D4DC-4917-8F71-95EA31C97890")]
  public abstract class HeuristicOptimizationEngineAlgorithm : EngineAlgorithm {
    public new IHeuristicOptimizationProblem Problem {
      get { return (IHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    [Storable] public IConstrainedValueParameter<ISolutionCreator> SolutionCreatorParameter { get; private set; }
    public ISolutionCreator SolutionCreator {
      get => SolutionCreatorParameter.Value;
      set {
        if (!SolutionCreatorParameter.ValidValues.Contains(value))
          SolutionCreatorParameter.ValidValues.Add(value);
        SolutionCreatorParameter.Value = value;
      }
    }

    protected HeuristicOptimizationEngineAlgorithm() : base() {
      Parameters.Add(SolutionCreatorParameter = new ConstrainedValueParameter<ISolutionCreator>("SolutionCreator", "An operator that creates a solution for a given problem."));

      RegisterEventHandlers();
    }
    protected HeuristicOptimizationEngineAlgorithm(string name) : base(name) {
      Parameters.Add(SolutionCreatorParameter = new ConstrainedValueParameter<ISolutionCreator>("SolutionCreator", "An operator that creates a solution for a given problem."));

      RegisterEventHandlers();
    }
    protected HeuristicOptimizationEngineAlgorithm(string name, ParameterCollection parameters) : base(name, parameters) {
      Parameters.Add(SolutionCreatorParameter = new ConstrainedValueParameter<ISolutionCreator>("SolutionCreator", "An operator that creates a solution for a given problem."));

      RegisterEventHandlers();
    }
    protected HeuristicOptimizationEngineAlgorithm(string name, string description) : base(name, description) {
      Parameters.Add(SolutionCreatorParameter = new ConstrainedValueParameter<ISolutionCreator>("SolutionCreator", "An operator that creates a solution for a given problem."));

      RegisterEventHandlers();
    }
    protected HeuristicOptimizationEngineAlgorithm(string name, string description, ParameterCollection parameters) : base(name, description, parameters) {
      Parameters.Add(SolutionCreatorParameter = new ConstrainedValueParameter<ISolutionCreator>("SolutionCreator", "An operator that creates a solution for a given problem."));

      RegisterEventHandlers();
    }

    [StorableConstructor]
    protected HeuristicOptimizationEngineAlgorithm(StorableConstructorFlag _) : base(_) { }
    protected HeuristicOptimizationEngineAlgorithm(HeuristicOptimizationEngineAlgorithm original, Cloner cloner) : base(original, cloner) {
      SolutionCreatorParameter = cloner.Clone(original.SolutionCreatorParameter);

      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    #region Events
    private void RegisterEventHandlers() {
      ParameterChangeHandler<ISolutionCreator>.Create(SolutionCreatorParameter, SolutionCreatorOnChanged);
    }
    protected override void DeregisterProblemEvents() {
      Problem.EvaluatorChanged -= Problem_EvaluatorChanged;
      base.DeregisterProblemEvents();
    }
    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      Problem.EvaluatorChanged += Problem_EvaluatorChanged;
    }
    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      SolutionCreatorParameter.Repopulate(Problem.Operators);
    }

    protected virtual void SolutionCreatorOnChanged() { }
    protected virtual void Problem_EvaluatorChanged(object sender, EventArgs e) { }

    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
      SolutionCreatorParameter.Repopulate(Problem.Operators);
    }
    #endregion
  }
}
