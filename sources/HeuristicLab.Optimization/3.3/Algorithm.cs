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

    public abstract TimeSpan ExecutionTime { get; }

    private bool running;
    public bool Running {
      get { return running; }
      protected set {
        if (running != value) {
          running = value;
          OnRunningChanged();
        }
      }
    }

    public abstract bool Finished { get; }

    private bool canceled;
    protected bool Canceled {
      get { return canceled; }
      private set {
        if (canceled != value) {
          canceled = value;
          OnCanceledChanged();
        }
      }
    }

    protected Algorithm() : base() { }
    protected Algorithm(string name) : base(name) { }
    protected Algorithm(string name, ParameterCollection parameters) : base(name, parameters) { }
    protected Algorithm(string name, string description) : base(name, description) { }
    protected Algorithm(string name, string description, ParameterCollection parameters) : base(name, description, parameters) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      Algorithm clone = (Algorithm)base.Clone(cloner);
      clone.Problem = (IProblem)cloner.Clone(problem);
      clone.running = running;
      clone.canceled = canceled;
      return clone;
    }

    public void Prepare() {
      OnPrepared();
    }
    public void Start() {
      OnStarted();
      Running = true;
      Canceled = false;
    }
    public void Stop() {
      Canceled = true;
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
    public event EventHandler ProblemChanged;
    protected virtual void OnProblemChanged() {
      if (ProblemChanged != null)
        ProblemChanged(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      if (ExecutionTimeChanged != null)
        ExecutionTimeChanged(this, EventArgs.Empty);
    }
    public event EventHandler RunningChanged;
    protected virtual void OnRunningChanged() {
      if (RunningChanged != null)
        RunningChanged(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    protected virtual void OnPrepared() {
      if (Prepared != null)
        Prepared(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    protected virtual void OnStarted() {
      if (Started != null)
        Started(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    protected virtual void OnStopped() {
      Canceled = false;
      Running = false;
      if (Stopped != null)
        Stopped(this, EventArgs.Empty);
    }
    protected virtual void OnCanceledChanged() { }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    protected virtual void OnExceptionOccurred(Exception exception) {
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new EventArgs<Exception>(exception));
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
