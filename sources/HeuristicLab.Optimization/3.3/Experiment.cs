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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// An experiment which contains multiple batch runs of algorithms.
  /// </summary>
  [Item("Experiment", "An experiment which contains multiple batch runs of algorithms.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public sealed class Experiment : NamedItem, IOptimizer {
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
      private set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    private OptimizerList optimizers;
    [Storable]
    public OptimizerList Optimizers {
      get { return optimizers; }
      private set {
        if (optimizers != value) {
          if (optimizers != null) DeregisterOptimizersEvents();
          optimizers = value;
          if (optimizers != null) RegisterOptimizersEvents();
          foreach (IOptimizer optimizer in optimizers)
            RegisterOptimizerEvents(optimizer);
        }
      }
    }

    private bool stopPending;

    public Experiment()
      : base() {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      Optimizers = new OptimizerList();
      stopPending = false;
    }
    public Experiment(string name) : base(name) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      Optimizers = new OptimizerList();
      stopPending = false;
    }
    public Experiment(string name, string description) : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      Optimizers = new OptimizerList();
      stopPending = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Experiment clone = (Experiment)base.Clone(cloner);
      clone.executionState = executionState;
      clone.executionTime = executionTime;
      clone.Optimizers = (OptimizerList)cloner.Clone(optimizers);
      clone.stopPending = stopPending;
      return clone;
    }

    public void Prepare() {
      Prepare(true);
    }
    public void Prepare(bool clearResults) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count > 0) {
        foreach (IOptimizer optimizer in Optimizers.Where(x => x.ExecutionState != ExecutionState.Started))
          optimizer.Prepare(clearResults);
      }
    }
    public void Start() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
      stopPending = false;
      if (Optimizers.Count > 0) {
        IOptimizer optimizer = Optimizers.FirstOrDefault(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused));
        if (optimizer != null) optimizer.Start();
      }
    }
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
      if (Optimizers.Count > 0) {
        foreach (IOptimizer optimizer in Optimizers.Where(x => x.ExecutionState == ExecutionState.Started))
          optimizer.Pause();
      }
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      stopPending = true;
      if (Optimizers.Count > 0) {
        foreach (IOptimizer optimizer in Optimizers.Where(x => (x.ExecutionState == ExecutionState.Started) || (x.ExecutionState == ExecutionState.Paused)))
          optimizer.Stop();
      }
    }

    #region Events
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
      foreach (IndexedItem<IOptimizer> item in e.OldItems) {
        DeregisterOptimizerEvents(item.Value);
      }
      foreach (IndexedItem<IOptimizer> item in e.Items) {
        RegisterOptimizerEvents(item.Value);
      }
    }
    private void Optimizers_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.Items) {
        RegisterOptimizerEvents(item.Value);
      }
    }
    private void Optimizers_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.Items) {
        DeregisterOptimizerEvents(item.Value);
      }
    }
    private void Optimizers_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      foreach (IndexedItem<IOptimizer> item in e.OldItems) {
        DeregisterOptimizerEvents(item.Value);
      }
      foreach (IndexedItem<IOptimizer> item in e.Items) {
        RegisterOptimizerEvents(item.Value);
      }
    }

    private void RegisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged += new EventHandler(optimizer_ExecutionTimeChanged);
      optimizer.Paused += new EventHandler(optimizer_Paused);
      optimizer.Prepared += new EventHandler(optimizer_Prepared);
      optimizer.Started += new EventHandler(optimizer_Started);
      optimizer.Stopped += new EventHandler(optimizer_Stopped);
    }

    private void DeregisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      optimizer.ExecutionTimeChanged -= new EventHandler(optimizer_ExecutionTimeChanged);
      optimizer.Paused -= new EventHandler(optimizer_Paused);
      optimizer.Prepared -= new EventHandler(optimizer_Prepared);
      optimizer.Started -= new EventHandler(optimizer_Started);
      optimizer.Stopped -= new EventHandler(optimizer_Stopped);
    }
    private void optimizer_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void optimizer_ExecutionTimeChanged(object sender, EventArgs e) {
      ExecutionTime = Optimizers.Aggregate(TimeSpan.Zero, (t, o) => t + o.ExecutionTime);
    }
    private void optimizer_Paused(object sender, EventArgs e) {
      if (Optimizers.All(x => x.ExecutionState != ExecutionState.Started))
        OnPaused();
    }
    private void optimizer_Prepared(object sender, EventArgs e) {
      if (ExecutionState == ExecutionState.Stopped)
        OnPrepared();
    }
    private void optimizer_Started(object sender, EventArgs e) {
      if (ExecutionState != ExecutionState.Started)
        OnStarted();
    }
    private void optimizer_Stopped(object sender, EventArgs e) {
      bool stop = stopPending;

      if (Optimizers.All(x => (x.ExecutionState != ExecutionState.Started) && (x.ExecutionState != ExecutionState.Paused))) {
        stopPending = false;
        OnStopped();
      }

      if (!stop) {
        IOptimizer next = Optimizers.FirstOrDefault(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused));
        if (next != null)
          next.Start();
      }
    }
    #endregion
  }
}
