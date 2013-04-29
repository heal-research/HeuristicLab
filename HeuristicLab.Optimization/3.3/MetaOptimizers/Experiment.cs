#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// An experiment which contains multiple algorithms, batch runs or other experiments.
  /// </summary>
  [Item("Experiment", "An experiment which contains multiple algorithms, batch runs or other experiments.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public sealed class Experiment : NamedItem, IOptimizer, IStorableContent {
    public string Filename { get; set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public override Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VSImageLibrary.ExperimentPrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VSImageLibrary.ExperimentStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.ExperimentPaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VSImageLibrary.ExperimentStopped;
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
      private set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    [Storable]
    private OptimizerList optimizers;
    public OptimizerList Optimizers {
      get { return optimizers; }
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
        if (Optimizers == null) yield break;

        foreach (IOptimizer opt in Optimizers) {
          yield return opt;
          foreach (IOptimizer nestedOpt in opt.NestedOptimizers)
            yield return nestedOpt;
        }
      }
    }

    private bool experimentStarted = false;
    private bool experimentStopped = false;

    public Experiment()
      : base() {
      name = ItemName;
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      optimizers = new OptimizerList();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    public Experiment(string name)
      : base(name) {
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      optimizers = new OptimizerList();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    public Experiment(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      optimizers = new OptimizerList();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    [StorableConstructor]
    private Experiment(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private Experiment(Experiment original, Cloner cloner)
      : base(original, cloner) {
      executionState = original.executionState;
      executionTime = original.executionTime;
      optimizers = cloner.Clone(original.optimizers);
      runs = cloner.Clone(original.runs);

      experimentStarted = original.experimentStarted;
      experimentStopped = original.experimentStopped;
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      return new Experiment(this, cloner);
    }

    private void Initialize() {
      RegisterOptimizersEvents();
      foreach (IOptimizer optimizer in optimizers)
        RegisterOptimizerEvents(optimizer);
      if (runs != null) RegisterRunsEvents();
    }

    public void Prepare() {
      Prepare(false);
    }
    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count == 0) return;

      if (clearRuns) runs.Clear();

      experimentStarted = false;
      experimentStopped = false;
      foreach (IOptimizer optimizer in Optimizers.Where(x => x.ExecutionState != ExecutionState.Started)) {
        // a race-condition may occur when the optimizer has changed the state by itself in the meantime
        try { optimizer.Prepare(clearRuns); } catch (InvalidOperationException) { }
      }
    }
    public void Start() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count == 0) return;

      experimentStarted = true;
      experimentStopped = false;
      IOptimizer optimizer = Optimizers.FirstOrDefault(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused));
      if (optimizer != null) {
        // a race-condition may occur when the optimizer has changed the state by itself in the meantime
        try { optimizer.Start(); } catch (InvalidOperationException) { }
      }
    }
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count == 0) return;

      experimentStarted = false;
      experimentStopped = false;
      foreach (IOptimizer optimizer in Optimizers.Where(x => x.ExecutionState == ExecutionState.Started)) {
        // a race-condition may occur when the optimizer has changed the state by itself in the meantime
        try { optimizer.Pause(); } catch (InvalidOperationException) { }
      }
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count == 0) return;

      experimentStarted = false;
      experimentStopped = true;
      if (Optimizers.Any(x => (x.ExecutionState == ExecutionState.Started) || (x.ExecutionState == ExecutionState.Paused))) {
        foreach (var optimizer in Optimizers.Where(x => (x.ExecutionState == ExecutionState.Started) || (x.ExecutionState == ExecutionState.Paused))) {
          // a race-condition may occur when the optimizer has changed the state by itself in the meantime
          try { optimizer.Stop(); } catch (InvalidOperationException) { }
        }
      } else {
        OnStopped();
      }
    }

    #region Events
    protected override void OnNameChanged() {
      base.OnNameChanged();
      Runs.OptimizerName = Name;
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
    public event EventHandler Prepared;
    private void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      ExecutionState = ExecutionState.Stopped;
      EventHandler handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    private void RegisterOptimizersEvents() {
      Optimizers.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      Optimizers.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
      Optimizers.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
      Optimizers.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
    }
    private void DeregisterOptimizersEvents() {
      Optimizers.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      Optimizers.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
      Optimizers.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
      Optimizers.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
    }
    private void Optimizers_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.OldItems)
        RemoveOptimizer(item.Value);
      foreach (IndexedItem<IOptimizer> item in e.Items)
        AddOptimizer(item.Value);
    }
    private void Optimizers_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.Items)
        AddOptimizer(item.Value);
    }
    private void Optimizers_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.Items)
        RemoveOptimizer(item.Value);
    }
    private void Optimizers_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.OldItems)
        RemoveOptimizer(item.Value);
      foreach (IndexedItem<IOptimizer> item in e.Items)
        AddOptimizer(item.Value);
    }
    private void AddOptimizer(IOptimizer optimizer) {
      RegisterOptimizerEvents(optimizer);
      Runs.AddRange(optimizer.Runs);
      optimizer.Prepare();
      if (ExecutionState == ExecutionState.Stopped && optimizer.ExecutionState == ExecutionState.Prepared)
        OnPrepared();
    }
    private void RemoveOptimizer(IOptimizer optimizer) {
      DeregisterOptimizerEvents(optimizer);
      Runs.RemoveRange(optimizer.Runs);
      if (ExecutionState == ExecutionState.Prepared && !optimizers.Any(opt => opt.ExecutionState == ExecutionState.Prepared))
        OnStopped();
    }

    private void RegisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged += new EventHandler(optimizer_ExecutionTimeChanged);
      optimizer.Paused += new EventHandler(optimizer_Paused);
      optimizer.Prepared += new EventHandler(optimizer_Prepared);
      optimizer.Started += new EventHandler(optimizer_Started);
      optimizer.Stopped += new EventHandler(optimizer_Stopped);
      optimizer.Runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_CollectionReset);
      optimizer.Runs.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_ItemsAdded);
      optimizer.Runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_ItemsRemoved);
    }
    private void DeregisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged -= new EventHandler(optimizer_ExecutionTimeChanged);
      optimizer.Paused -= new EventHandler(optimizer_Paused);
      optimizer.Prepared -= new EventHandler(optimizer_Prepared);
      optimizer.Started -= new EventHandler(optimizer_Started);
      optimizer.Stopped -= new EventHandler(optimizer_Stopped);
      optimizer.Runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_CollectionReset);
      optimizer.Runs.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_ItemsAdded);
      optimizer.Runs.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(optimizer_Runs_ItemsRemoved);
    }

    private readonly object locker = new object();
    private void optimizer_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      lock (locker)
        OnExceptionOccurred(e.Value);
    }
    private void optimizer_ExecutionTimeChanged(object sender, EventArgs e) {
      lock (locker)
        ExecutionTime = Optimizers.Aggregate(TimeSpan.Zero, (t, o) => t + o.ExecutionTime);
    }
    private void optimizer_Paused(object sender, EventArgs e) {
      lock (locker)
        if (Optimizers.All(x => x.ExecutionState != ExecutionState.Started)) OnPaused();
    }
    private void optimizer_Prepared(object sender, EventArgs e) {
      lock (locker)
        if (Optimizers.All(x => x.ExecutionState == ExecutionState.Prepared)) OnPrepared();
    }
    private void optimizer_Started(object sender, EventArgs e) {
      lock (locker)
        if (ExecutionState != ExecutionState.Started) OnStarted();
    }
    private void optimizer_Stopped(object sender, EventArgs e) {
      lock (locker) {
        if (experimentStopped) {
          if (Optimizers.All(x => (x.ExecutionState == ExecutionState.Stopped) || (x.ExecutionState == ExecutionState.Prepared))) OnStopped();
        } else {
          if (experimentStarted && Optimizers.Any(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused))) {
            Optimizers.First(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused)).Start();
          } else if (Optimizers.All(x => x.ExecutionState == ExecutionState.Stopped)) OnStopped();
          else if (Optimizers.Any(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused)) && Optimizers.All(o => o.ExecutionState != ExecutionState.Started)) OnPaused();
        }
      }
    }
    private void optimizer_Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      lock (locker) {
        Runs.RemoveRange(e.OldItems);
        Runs.AddRange(e.Items);
      }
    }
    private void optimizer_Runs_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      lock (locker)
        Runs.AddRange(e.Items);
    }
    private void optimizer_Runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      lock (locker)
        Runs.RemoveRange(e.Items);
    }

    private void RegisterRunsEvents() {
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
      runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsRemoved);
    }
    private void DeregisterRunsEvents() {
      runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
      runs.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Runs_ItemsRemoved);
    }
    private void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (IOptimizer optimizer in Optimizers)
        optimizer.Runs.RemoveRange(e.OldItems);
    }
    private void Runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (IOptimizer optimizer in Optimizers)
        optimizer.Runs.RemoveRange(e.Items);
    }
    #endregion
  }
}
