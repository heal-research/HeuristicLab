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
  public sealed class Experiment : NamedItem, IExecutable {
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

    private BatchRunList batchRuns;
    [Storable]
    public BatchRunList BatchRuns {
      get { return batchRuns; }
      private set {
        if (batchRuns != value) {
          if (batchRuns != null) DeregisterBatchRunsEvents();
          batchRuns = value;
          if (batchRuns != null) RegisterBatchRunsEvents();
          foreach (BatchRun batchRun in batchRuns)
            RegisterBatchRunEvents(batchRun);
        }
      }
    }

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
    }

    private bool stopPending;

    public Experiment()
      : base() {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      BatchRuns = new BatchRunList();
      runs = new RunCollection();
      stopPending = false;
    }
    public Experiment(string name) : base(name) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      BatchRuns = new BatchRunList();
      runs = new RunCollection();
      stopPending = false;
    }
    public Experiment(string name, string description) : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      BatchRuns = new BatchRunList();
      runs = new RunCollection();
      stopPending = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Experiment clone = (Experiment)base.Clone(cloner);
      clone.executionState = executionState;
      clone.executionTime = executionTime;
      clone.BatchRuns = (BatchRunList)cloner.Clone(batchRuns);
      clone.runs = (RunCollection)cloner.Clone(runs);
      clone.stopPending = stopPending;
      return clone;
    }

    public void Prepare() {
      Prepare(true);
    }
    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (BatchRuns.Count > 0) {
        if (clearRuns) {
          ExecutionTime = TimeSpan.Zero;
          runs.Clear();
        }
        foreach (BatchRun batchRun in BatchRuns.Where(x => x.ExecutionState != ExecutionState.Started))
          batchRun.Prepare(clearRuns);
      }
    }
    public void Start() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
      stopPending = false;
      if (BatchRuns.Count > 0) {
        BatchRun batchRun = BatchRuns.FirstOrDefault(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused));
        if (batchRun != null) batchRun.Start();
      }
    }
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
      if (BatchRuns.Count > 0) {
        foreach (BatchRun batchRun in BatchRuns.Where(x => x.ExecutionState == ExecutionState.Started))
          batchRun.Pause();
      }
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      stopPending = true;
      if (BatchRuns.Count > 0) {
        foreach (BatchRun batchRun in BatchRuns.Where(x => (x.ExecutionState == ExecutionState.Started) || (x.ExecutionState == ExecutionState.Paused)))
          batchRun.Stop();
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

    private void RegisterBatchRunsEvents() {
      BatchRuns.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_CollectionReset);
      BatchRuns.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsAdded);
      BatchRuns.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsRemoved);
      BatchRuns.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsReplaced);
    }
    private void DeregisterBatchRunsEvents() {
      BatchRuns.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_CollectionReset);
      BatchRuns.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsAdded);
      BatchRuns.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsRemoved);
      BatchRuns.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<BatchRun>>(BatchRuns_ItemsReplaced);
    }
    private void BatchRuns_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<BatchRun>> e) {
      foreach (IndexedItem<BatchRun> item in e.OldItems) {
        DeregisterBatchRunEvents(item.Value);
      }
      foreach (IndexedItem<BatchRun> item in e.Items) {
        RegisterBatchRunEvents(item.Value);
      }
    }
    private void BatchRuns_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<BatchRun>> e) {
      foreach (IndexedItem<BatchRun> item in e.Items) {
        RegisterBatchRunEvents(item.Value);
      }
    }
    private void BatchRuns_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<BatchRun>> e) {
      foreach (IndexedItem<BatchRun> item in e.Items) {
        DeregisterBatchRunEvents(item.Value);
      }
    }
    private void BatchRuns_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<BatchRun>> e) {
      foreach (IndexedItem<BatchRun> item in e.OldItems) {
        DeregisterBatchRunEvents(item.Value);
      }
      foreach (IndexedItem<BatchRun> item in e.Items) {
        RegisterBatchRunEvents(item.Value);
      }
    }

    private void RegisterBatchRunEvents(BatchRun batchRun) {
      batchRun.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(batchRun_ExceptionOccurred);
      batchRun.Paused += new EventHandler(batchRun_Paused);
      batchRun.Prepared += new EventHandler(batchRun_Prepared);
      batchRun.Started += new EventHandler(batchRun_Started);
      batchRun.Stopped += new EventHandler(batchRun_Stopped);
    }
    private void DeregisterBatchRunEvents(BatchRun batchRun) {
      batchRun.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(batchRun_ExceptionOccurred);
      batchRun.Paused -= new EventHandler(batchRun_Paused);
      batchRun.Prepared -= new EventHandler(batchRun_Prepared);
      batchRun.Started -= new EventHandler(batchRun_Started);
      batchRun.Stopped -= new EventHandler(batchRun_Stopped);
    }
    private void batchRun_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void batchRun_Paused(object sender, EventArgs e) {
      if (BatchRuns.All(x => x.ExecutionState != ExecutionState.Started))
        OnPaused();
    }
    void batchRun_Prepared(object sender, EventArgs e) {
      if (ExecutionState == ExecutionState.Stopped)
        OnPrepared();
    }
    private void batchRun_Started(object sender, EventArgs e) {
      if (ExecutionState != ExecutionState.Started)
        OnStarted();
    }
    private void batchRun_Stopped(object sender, EventArgs e) {
      bool stop = stopPending;
      BatchRun batchRun = (BatchRun)sender;
      ExecutionTime += batchRun.ExecutionTime;
      runs.AddRange(batchRun.Runs);

      if (BatchRuns.All(x => (x.ExecutionState != ExecutionState.Started) && (x.ExecutionState != ExecutionState.Paused))) {
        stopPending = false;
        OnStopped();
      }

      if (!stop) {
        BatchRun next = BatchRuns.FirstOrDefault(x => (x.ExecutionState == ExecutionState.Prepared) || (x.ExecutionState == ExecutionState.Paused));
        if (next != null)
          next.Start();
      }
    }
    #endregion
  }
}
