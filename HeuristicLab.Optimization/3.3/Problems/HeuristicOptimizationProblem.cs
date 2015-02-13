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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Heuristic Optimization Problem", "Represents the base class for a heuristic optimization problem.")]
  [StorableClass]
  public abstract class HeuristicOptimizationProblem<T, U> : Problem, IHeuristicOptimizationProblem
    where T : class,IEvaluator
    where U : class,ISolutionCreator {
    private const string EvaluatorParameterName = "Evaluator";
    private const string SolutionCreateParameterName = "SolutionCreator";

    [StorableConstructor]
    protected HeuristicOptimizationProblem(bool deserializing) : base(deserializing) { }
    protected HeuristicOptimizationProblem(HeuristicOptimizationProblem<T, U> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected HeuristicOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<T>(EvaluatorParameterName, "The operator used to evaluate a solution."));
      Parameters.Add(new ValueParameter<U>(SolutionCreateParameterName, "The operator to create a solution."));
      RegisterEventHandlers();
    }

    protected HeuristicOptimizationProblem(T evaluator, U solutionCreator)
      : base() {
      Parameters.Add(new ValueParameter<T>(EvaluatorParameterName, "The operator used to evaluate a solution.", evaluator));
      Parameters.Add(new ValueParameter<U>(SolutionCreateParameterName, "The operator to create a solution.", solutionCreator));
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
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

    public U SolutionCreator {
      get { return (U)SolutionCreatorParameter.Value; }
      protected set { SolutionCreatorParameter.Value = value; }
    }
    public IValueParameter SolutionCreatorParameter {
      get { return (IValueParameter)Parameters[SolutionCreateParameterName]; }
    }
    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator { get { return SolutionCreator; } }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter { get { return SolutionCreatorParameter; } }
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

    protected virtual void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged();
    }
    public event EventHandler SolutionCreatorChanged;
    protected virtual void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
