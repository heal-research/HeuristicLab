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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("3d385be2-5c83-43f2-a693-f984acac44f6")]
  internal enum BatchRunAction { None, Prepare, Start, Pause, Stop };

  /// <summary>
  /// A run in which an optimizer is executed a given number of times.
  /// </summary>
  [Item("Batch Run", "A run in which an optimizer is executed a given number of times.")]
  [Creatable(CreatableAttribute.Categories.TestingAndAnalysis, Priority = 110)]
  [StorableType("E85407E0-18EC-4198-8321-9CF030FDF6D7")]
  public sealed class BatchRun : NamedItem, IOptimizer, IStorableContent {
    public string Filename { get; set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public override Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VSImageLibrary.BatchRunPrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VSImageLibrary.BatchRunStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.BatchRunPaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VSImageLibrary.BatchRunStopped;
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
      get {
        if ((Optimizer != null) && (Optimizer.ExecutionState != ExecutionState.Stopped))
          return executionTime + Optimizer.ExecutionTime;
        else
          return executionTime;
      }
      private set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    [Storable]
    private TimeSpan runsExecutionTime;

    [Storable]
    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        if (optimizer != value) {
          if (optimizer != null) {
            DeregisterOptimizerEvents();
            IEnumerable<IRun> runs = optimizer.Runs;
            optimizer = null; //necessary to avoid removing the runs from the old optimizer
            Runs.RemoveRange(runs);
          }
          optimizer = value;
          if (optimizer != null) {
            RegisterOptimizerEvents();
            Runs.AddRange(optimizer.Runs);
          }
          OnOptimizerChanged();
          Prepare();
        }
      }
    }
    // BackwardsCompatibility3.3
    #region Backwards compatible code (remove with 3.4)
    [Storable(OldName = "algorithm")]
    private IAlgorithm algorithm {
      set { optimizer = value; }
    }
    #endregion

    [Storable]
    private int repetitions;
    public int Repetitions {
      get { return repetitions; }
      set {
        if (repetitions != value) {
          repetitions = value;
          OnRepetitionsChanged();
          if ((Optimizer != null) && (Optimizer.ExecutionState == ExecutionState.Stopped))
            Prepare();
        }
      }
    }
    [Storable]
    private int repetitionsCounter;
    public int RepetitionsCounter {
      get { return repetitionsCounter; }
      private set {
        if (value != repetitionsCounter) {
          repetitionsCounter = value;
          OnRepetitionsCounterChanged();
        }
      }
    }

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
      private set {
        if (value == null) throw new ArgumentNullException();
        if (runs != value) {
          if (runs != null) DeregisterRunsEvents();
          runs = value;
          if (runs != null) RegisterRunsEvents();
        }
      }
    }

    public IEnumerable<IOptimizer> NestedOptimizers {
      get {
        if (Optimizer == null) yield break;

        yield return Optimizer;
        foreach (IOptimizer opt in Optimizer.NestedOptimizers)
          yield return opt;
      }
    }

    private BatchRunAction batchRunAction = BatchRunAction.None;

    public BatchRun()
      : base() {
      name = ItemName;
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      runsExecutionTime = TimeSpan.Zero;
      repetitions = 10;
      repetitionsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    public BatchRun(string name)
      : base(name) {
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      runsExecutionTime = TimeSpan.Zero;
      repetitions = 10;
      repetitionsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    public BatchRun(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      runsExecutionTime = TimeSpan.Zero;
      repetitions = 10;
      repetitionsCounter = 0;
      Runs = new RunCollection { OptimizerName = Name };
    }
    [StorableConstructor]
    private BatchRun(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private BatchRun(BatchRun original, Cloner cloner)
      : base(original, cloner) {
      executionState = original.executionState;
      executionTime = original.executionTime;
      runsExecutionTime = original.runsExecutionTime;
      optimizer = cloner.Clone(original.optimizer);
      repetitions = original.repetitions;
      repetitionsCounter = original.repetitionsCounter;
      runs = cloner.Clone(original.runs);
      batchRunAction = original.batchRunAction;
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      return new BatchRun(this, cloner);
    }

    private void Initialize() {
      if (optimizer != null) RegisterOptimizerEvents();
      if (runs != null) RegisterRunsEvents();
    }

    public void Prepare() {
      Prepare(false);
    }
    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizer != null) {
        ExecutionTime = TimeSpan.Zero;
        RepetitionsCounter = 0;
        if (clearRuns) runs.Clear();
        batchRunAction = BatchRunAction.Prepare;
        // a race-condition may occur when the optimizer has changed the state by itself in the meantime
        try { Optimizer.Prepare(clearRuns); } catch (InvalidOperationException) { }
      } else {
        ExecutionState = ExecutionState.Stopped;
      }
    }
    public void Start() {
      Start(CancellationToken.None);
    }
    public void Start(CancellationToken cancellationToken) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizer == null) return;
      batchRunAction = BatchRunAction.Start;
      if (Optimizer.ExecutionState == ExecutionState.Stopped) Optimizer.Prepare();
      for (int i = repetitionsCounter; i < repetitions; i++) {
        // a race-condition may occur when the optimizer has changed the state by itself in the meantime
        try { Optimizer.Start(cancellationToken); } catch (InvalidOperationException) { }
        if (ExecutionState == ExecutionState.Paused || ExecutionState == ExecutionState.Stopped) break;
        Optimizer.Prepare();
        if (ExecutionState == ExecutionState.Paused || ExecutionState == ExecutionState.Stopped) break;
      }
    }
    public async Task StartAsync() { await StartAsync(CancellationToken.None); }
    public async Task StartAsync(CancellationToken cancellationToken) {
      await AsyncHelper.DoAsync(Start, cancellationToken);
    }
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizer == null) return;
      batchRunAction = BatchRunAction.Pause;
      if (Optimizer.ExecutionState != ExecutionState.Started) return;
      // a race-condition may occur when the optimizer has changed the state by itself in the meantime
      try { Optimizer.Pause(); } catch (InvalidOperationException) { } catch (NotSupportedException) { }
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizer == null) return;
      batchRunAction = BatchRunAction.Stop;
      if (Optimizer.ExecutionState != ExecutionState.Started && Optimizer.ExecutionState != ExecutionState.Paused) {
        OnStopped();
        return;
      }
      // a race-condition may occur when the optimizer has changed the state by itself in the meantime
      try { Optimizer.Stop(); } catch (InvalidOperationException) { }
    }

    #region Events
    protected override void OnNameChanged() {
      base.OnNameChanged();
      runs.OptimizerName = Name;
    }

    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OptimizerChanged;
    private void OnOptimizerChanged() {
      EventHandler handler = OptimizerChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler RepetitionsChanged;
    private void OnRepetitionsChanged() {
      EventHandler handler = RepetitionsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler RepetetionsCounterChanged;
    private void OnRepetitionsCounterChanged() {
      EventHandler handler = RepetetionsCounterChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      batchRunAction = BatchRunAction.None;
      ExecutionState = ExecutionState.Prepared;
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      // no reset of BatchRunAction.Started, because we need to differ which of the two was started by the user
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      batchRunAction = BatchRunAction.None;
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      batchRunAction = BatchRunAction.None;
      ExecutionState = ExecutionState.Stopped;
      EventHandler handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    private void RegisterOptimizerEvents() {
      optimizer.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged += new EventHandler(Optimizer_ExecutionTimeChanged);
      optimizer.Paused += new EventHandler(Optimizer_Paused);
      optimizer.Prepared += new EventHandler(Optimizer_Prepared);
      optimizer.Started += new EventHandler(Optimizer_Started);
      optimizer.Stopped += new EventHandler(Optimizer_Stopped);
      optimizer.Runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_CollectionReset);
      optimizer.Runs.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_ItemsAdded);
      optimizer.Runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_ItemsRemoved);
    }
    private void DeregisterOptimizerEvents() {
      optimizer.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged -= new EventHandler(Optimizer_ExecutionTimeChanged);
      optimizer.Paused -= new EventHandler(Optimizer_Paused);
      optimizer.Prepared -= new EventHandler(Optimizer_Prepared);
      optimizer.Started -= new EventHandler(Optimizer_Started);
      optimizer.Stopped -= new EventHandler(Optimizer_Stopped);
      optimizer.Runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_CollectionReset);
      optimizer.Runs.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_ItemsAdded);
      optimizer.Runs.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Optimizer_Runs_ItemsRemoved);
    }
    private void Optimizer_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Optimizer_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }
    private void Optimizer_Paused(object sender, EventArgs e) {
      if (ExecutionState == ExecutionState.Started) {
        OnPaused();
      }
    }
    private void Optimizer_Prepared(object sender, EventArgs e) {
      if (batchRunAction == BatchRunAction.Prepare || ExecutionState == ExecutionState.Stopped) {
        ExecutionTime = TimeSpan.Zero;
        runsExecutionTime = TimeSpan.Zero;
        RepetitionsCounter = 0;
        OnPrepared();
      }
    }
    private void Optimizer_Started(object sender, EventArgs e) {
      if (ExecutionState != ExecutionState.Started)
        OnStarted();
    }
    private void Optimizer_Stopped(object sender, EventArgs e) {
      RepetitionsCounter++;
      ExecutionTime += runsExecutionTime;
      runsExecutionTime = TimeSpan.Zero;

      if (batchRunAction == BatchRunAction.Stop) OnStopped();
      else if (repetitionsCounter >= repetitions) OnStopped();
      else if (batchRunAction == BatchRunAction.Pause) OnPaused();
      else if (batchRunAction == BatchRunAction.Start) return;
      else if (executionState == ExecutionState.Started) {
        // if the batch run hasn't been started but the inner optimizer was run then pause
        OnPaused();
      } else OnStopped();
    }
    private void Optimizer_Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.RemoveRange(e.OldItems);
      Runs.AddRange(e.Items);
    }
    private void Optimizer_Runs_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.AddRange(e.Items);
    }
    private void Optimizer_Runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.RemoveRange(e.Items);
    }

    private void RegisterRunsEvents() {
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
      runs.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsAdded);
      runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsRemoved);
    }

    private void DeregisterRunsEvents() {
      runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
      runs.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsAdded);
      runs.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsRemoved);
    }
    private void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (Optimizer != null) Optimizer.Runs.RemoveRange(e.OldItems);
      foreach (IRun run in e.Items) {
        IItem item;
        run.Results.TryGetValue("Execution Time", out item);
        TimeSpanValue executionTime = item as TimeSpanValue;
        if (executionTime != null) ExecutionTime += executionTime.Value;
      }
    }
    private void Runs_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (IRun run in e.Items) {
        IItem item;
        run.Results.TryGetValue("Execution Time", out item);
        TimeSpanValue executionTime = item as TimeSpanValue;
        if (executionTime != null) {
          if (Optimizer.ExecutionState == ExecutionState.Started)
            runsExecutionTime += executionTime.Value;
          else
            ExecutionTime += executionTime.Value;
        }
      }
    }
    private void Runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (Optimizer != null) Optimizer.Runs.RemoveRange(e.Items);
    }
    #endregion
  }
}
