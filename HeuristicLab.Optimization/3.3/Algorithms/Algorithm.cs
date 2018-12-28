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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for algorithms.
  /// </summary>
  [Item("Algorithm", "A base class for algorithms.")]
  [StorableType("7879C550-E8C2-4121-84C3-DADB06BD5C96")]
  public abstract class Algorithm : ParameterizedNamedItem, IAlgorithm {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public override Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStopped;
        else return base.ItemImage;
      }
    }

    [Storable]
    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      private set {
        if (executionState != value) {
          executionState = value;
          OnExecutionStateChanged();
          OnItemImageChanged();
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

    [Storable]
    private IProblem problem;
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

    [Storable]
    private bool storeAlgorithmInEachRun;
    public bool StoreAlgorithmInEachRun {
      get { return storeAlgorithmInEachRun; }
      set {
        if (storeAlgorithmInEachRun != value) {
          storeAlgorithmInEachRun = value;
          OnStoreAlgorithmInEachRunChanged();
        }
      }
    }

    [Storable]
    protected int runsCounter;

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
      protected set {
        if (value == null) throw new ArgumentNullException();
        if (runs != value) {
          if (runs != null) DeregisterRunsEvents();
          runs = value;
          if (runs != null) RegisterRunsEvents();
        }
      }
    }

    public virtual IEnumerable<IOptimizer> NestedOptimizers {
      get { return Enumerable.Empty<IOptimizer>(); }
    }

    protected Algorithm()
      : base() {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    protected Algorithm(string name)
      : base(name) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    protected Algorithm(string name, ParameterCollection parameters)
      : base(name, parameters) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    protected Algorithm(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    protected Algorithm(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    [StorableConstructor]
    protected Algorithm(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    protected Algorithm(Algorithm original, Cloner cloner)
      : base(original, cloner) {
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      executionState = original.executionState;
      executionTime = original.executionTime;
      problem = cloner.Clone(original.problem);
      storeAlgorithmInEachRun = original.storeAlgorithmInEachRun;
      runsCounter = original.runsCounter;
      runs = cloner.Clone(original.runs);
      Initialize();
    }

    private void Initialize() {
      if (problem != null) RegisterProblemEvents();
      if (runs != null) RegisterRunsEvents();
    }

    public virtual void Prepare() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
    }
    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (clearRuns) runs.Clear();
      Prepare();
    }
    public virtual void Start() {
      Start(CancellationToken.None);
    }
    public virtual void Start(CancellationToken cancellationToken) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
    }
    public virtual async Task StartAsync() { await StartAsync(CancellationToken.None); }
    public virtual async Task StartAsync(CancellationToken cancellationToken) {
      await AsyncHelper.DoAsync(Start, cancellationToken);
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
      values.Add("Algorithm Name", new StringValue(Name));
      values.Add("Algorithm Type", new StringValue(this.GetType().GetPrettyName()));
      if (Problem != null) {
        Problem.CollectParameterValues(values);
        values.Add("Problem Name", new StringValue(Problem.Name));
        values.Add("Problem Type", new StringValue(Problem.GetType().GetPrettyName()));
      }
    }
    public virtual void CollectResultValues(IDictionary<string, IItem> values) {
      values.Add("Execution Time", new TimeSpanValue(ExecutionTime));
      Results.CollectResultValues(values);
    }

    protected override IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      var children = base.GetCollectedValues(param);
      foreach (var child in children) {
        if (child.Value is IOperator)
          yield return new KeyValuePair<string, IItem>(child.Key, new StringValue(((IOperator)child.Value).Name));
        else yield return child;
      }
    }

    #region Events
    protected override void OnNameChanged() {
      base.OnNameChanged();
      Runs.OptimizerName = Name;
    }

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
    public event EventHandler StoreAlgorithmInEachRunChanged;
    protected virtual void OnStoreAlgorithmInEachRunChanged() {
      EventHandler handler = StoreAlgorithmInEachRunChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    protected virtual void OnPrepared() {
      ExecutionTime = TimeSpan.Zero;
      foreach (IStatefulItem statefulObject in this.GetObjectGraphObjects(new HashSet<object>() { Runs }).OfType<IStatefulItem>()) {
        statefulObject.InitializeState();
      }
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
      try {
        foreach (
          IStatefulItem statefulObject in
          this.GetObjectGraphObjects(new HashSet<object>() {Runs}).OfType<IStatefulItem>()) {
          statefulObject.ClearState();
        }
        runsCounter++;
        try {
          runs.Add(new Run(string.Format("{0} Run {1}", Name, runsCounter), this));
        }
        catch (ArgumentException e) {
          OnExceptionOccurred(new InvalidOperationException("Run creation failed.", e));
        }
      }    
      finally {
        ExecutionState = ExecutionState.Stopped;
        EventHandler handler = Stopped;
        if (handler != null) handler(this, EventArgs.Empty);
      }
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    protected virtual void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    protected virtual void DeregisterProblemEvents() {
      problem.OperatorsChanged -= new EventHandler(Problem_OperatorsChanged);
      problem.Reset -= new EventHandler(Problem_Reset);
    }
    protected virtual void RegisterProblemEvents() {
      problem.OperatorsChanged += new EventHandler(Problem_OperatorsChanged);
      problem.Reset += new EventHandler(Problem_Reset);
    }
    protected virtual void Problem_OperatorsChanged(object sender, EventArgs e) { }
    protected virtual void Problem_Reset(object sender, EventArgs e) {
      Prepare();
    }

    protected virtual void DeregisterRunsEvents() {
      runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    protected virtual void RegisterRunsEvents() {
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    protected virtual void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      runsCounter = runs.Count;
    }
    #endregion
  }
}
