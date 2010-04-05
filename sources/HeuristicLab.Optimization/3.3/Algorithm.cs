#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for algorithms.
  /// </summary>
  [Item("Algorithm", "A base class for algorithms.")]
  [StorableClass]
  public abstract class Algorithm : ParameterizedNamedItem, IAlgorithm {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Event; }
    }

    [Storable]
    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      private set {
        if (executionState != value) {
          executionState = value;
          OnExecutionStateChanged();
        }
      }
    }

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      protected set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    public virtual Type ProblemType {
      get { return typeof(IProblem); }
    }

    private IProblem problem;
    [Storable]
    private IProblem ProblemPersistence {
      get { return problem; }
      set {
        if (problem != null) DeregisterProblemEvents();
        problem = value;
        if (problem != null) RegisterProblemEvents();
      }
    }
    public IProblem Problem {
      get { return problem; }
      set {
        if (problem != value) {
          if ((value != null) && !ProblemType.IsInstanceOfType(value)) throw new ArgumentException("Invalid problem type.");
          if (problem != null) DeregisterProblemEvents();
          problem = value;
          if (problem != null) RegisterProblemEvents();
          OnProblemChanged();
          Prepare();
        }
      }
    }

    public abstract ResultCollection Results { get; }

    protected Algorithm()
      : base() {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
    }
    protected Algorithm(string name)
      : base(name) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
    }
    protected Algorithm(string name, ParameterCollection parameters)
      : base(name, parameters) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
    }
    protected Algorithm(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
    }
    protected Algorithm(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Algorithm clone = (Algorithm)base.Clone(cloner);
      clone.executionState = executionState;
      clone.executionTime = executionTime;
      clone.Problem = (IProblem)cloner.Clone(problem);
      return clone;
    }

    public void Prepare() {
      Prepare(true);
    }
    public virtual void Prepare(bool clearResults) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      ExecutionTime = TimeSpan.Zero;
    }
    public virtual void Start() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
    }
    public virtual void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
    }
    public virtual void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
    }

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      base.CollectParameterValues(values);
      Problem.CollectParameterValues(values);
    }
    public virtual void CollectResultValues(IDictionary<string, IItem> values) {
      foreach (IResult result in Results)
        values.Add(result.Name, result.Value != null ? (IItem)result.Value.Clone() : null);
    }

    #region Events
    public event EventHandler ExecutionStateChanged;
    protected virtual void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ProblemChanged;
    protected virtual void OnProblemChanged() {
      EventHandler handler = ProblemChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    protected virtual void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    protected virtual void OnStarted() {
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    protected virtual void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    protected virtual void OnStopped() {
      ExecutionState = ExecutionState.Stopped;
      EventHandler handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    protected virtual void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    protected virtual void DeregisterProblemEvents() {
      problem.SolutionCreatorChanged -= new EventHandler(Problem_SolutionCreatorChanged);
      problem.EvaluatorChanged -= new EventHandler(Problem_EvaluatorChanged);
      problem.VisualizerChanged -= new EventHandler(Problem_VisualizerChanged);
      problem.OperatorsChanged -= new EventHandler(Problem_OperatorsChanged);
    }
    protected virtual void RegisterProblemEvents() {
      problem.SolutionCreatorChanged += new EventHandler(Problem_SolutionCreatorChanged);
      problem.EvaluatorChanged += new EventHandler(Problem_EvaluatorChanged);
      problem.VisualizerChanged += new EventHandler(Problem_VisualizerChanged);
      problem.OperatorsChanged += new EventHandler(Problem_OperatorsChanged);
    }

    protected virtual void Problem_SolutionCreatorChanged(object sender, EventArgs e) { }
    protected virtual void Problem_EvaluatorChanged(object sender, EventArgs e) { }
    protected virtual void Problem_VisualizerChanged(object sender, EventArgs e) { }
    protected virtual void Problem_OperatorsChanged(object sender, EventArgs e) { }
    #endregion
  }
}
