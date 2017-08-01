#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive {
  public class OptimizerHiveTask : HiveTask<OptimizerTask> {
    public Progress Progress { get; private set; }

    #region Constructors and Cloning
    public OptimizerHiveTask() {
      Progress = new Progress();
    }
    public OptimizerHiveTask(IOptimizer optimizer)
      : this() {
      this.ItemTask = new OptimizerTask(optimizer);
    }
    public OptimizerHiveTask(OptimizerTask optimizerJob)
      : this() {
      this.ItemTask = optimizerJob;
    }
    protected OptimizerHiveTask(OptimizerHiveTask original, Cloner cloner)
      : base(original, cloner) {
      Progress = new Progress();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OptimizerHiveTask(this, cloner);
    }
    #endregion

    /// <summary>
    /// if this.Optimizer is an experiment
    ///   Uses the child-optimizers of this.HiveTask and creates HiveTask-childs
    /// if this.Optimizer is a batchrun
    ///   Creates a number of child-jobs according to repetitions
    /// </summary>
    protected override void UpdateChildHiveTasks() {
      base.UpdateChildHiveTasks();
      childHiveTasksLock.EnterWriteLock();
      try {
        if (Task != null && syncTasksWithOptimizers) {
          if (!ItemTask.ComputeInParallel) {
            this.childHiveTasks.Clear();
          } else {
            if (ItemTask.Item is Optimization.Experiment) {
              Optimization.Experiment experiment = (Optimization.Experiment)ItemTask.Item;
              foreach (IOptimizer childOpt in experiment.Optimizers) {
                var optimizerHiveTask = new OptimizerHiveTask(childOpt);
                optimizerHiveTask.Task.Priority = Task.Priority; //inherit priority from parent
                this.childHiveTasks.Add(optimizerHiveTask);
              }
            } else if (ItemTask.Item is Optimization.BatchRun) {
              Optimization.BatchRun batchRun = ItemTask.OptimizerAsBatchRun;
              if (batchRun.Optimizer != null) {
                while (this.childHiveTasks.Count < batchRun.Repetitions) {
                  var optimizerHiveTask = new OptimizerHiveTask(batchRun.Optimizer);
                  optimizerHiveTask.Task.Priority = Task.Priority;
                  this.childHiveTasks.Add(optimizerHiveTask);
                }
                while (this.childHiveTasks.Count > batchRun.Repetitions) {
                  this.childHiveTasks.Remove(this.childHiveTasks.Last());
                }
              }
            }
          }
        }
      }
      finally {
        childHiveTasksLock.ExitWriteLock();
      }
    }

    protected override void RegisterItemTaskEvents() {
      base.RegisterItemTaskEvents();
      if (ItemTask != null) {
        if (ItemTask.Item is Optimization.Experiment) {
          Optimization.Experiment experiment = ItemTask.OptimizerAsExperiment;
          experiment.Optimizers.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
          experiment.Optimizers.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
          experiment.Optimizers.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
          experiment.Optimizers.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
        } else if (ItemTask.Item is Optimization.BatchRun) {
          Optimization.BatchRun batchRun = ItemTask.OptimizerAsBatchRun;
          batchRun.RepetitionsChanged += new EventHandler(batchRun_RepetitionsChanged);
          batchRun.OptimizerChanged += new EventHandler(batchRun_OptimizerChanged);
        }
      }
    }
    protected override void DeregisterItemTaskEvents() {
      base.DeregisterItemTaskEvents();
      if (ItemTask != null) {
        if (ItemTask.Item is Optimization.Experiment) {
          Optimization.Experiment experiment = ItemTask.OptimizerAsExperiment;
          experiment.Optimizers.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
          experiment.Optimizers.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
          experiment.Optimizers.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
          experiment.Optimizers.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
        } else if (ItemTask.Item is Optimization.BatchRun) {
          Optimization.BatchRun batchRun = ItemTask.OptimizerAsBatchRun;
          batchRun.RepetitionsChanged -= new EventHandler(batchRun_RepetitionsChanged);
          batchRun.OptimizerChanged -= new EventHandler(batchRun_OptimizerChanged);
        }
      }
    }

    private void batchRun_OptimizerChanged(object sender, EventArgs e) {
      if (syncTasksWithOptimizers) {
        UpdateChildHiveTasks();
      }
    }

    private void batchRun_RepetitionsChanged(object sender, EventArgs e) {
      if (syncTasksWithOptimizers) {
        UpdateChildHiveTasks();
      }
    }

    private void Optimizers_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (syncTasksWithOptimizers && this.ItemTask.ComputeInParallel) {
        childHiveTasksLock.EnterWriteLock();
        try {
          foreach (var item in e.Items) {
            if (GetChildByOptimizer(item.Value) == null && item.Value.Name != "Placeholder") {
              this.childHiveTasks.Add(new OptimizerHiveTask(item.Value));
            }
          }
        }
        finally { childHiveTasksLock.ExitWriteLock(); }
      }
    }
    private void Optimizers_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (syncTasksWithOptimizers && this.ItemTask.ComputeInParallel) {
        childHiveTasksLock.EnterWriteLock();
        try {
          foreach (var item in e.OldItems) {
            this.childHiveTasks.Remove(this.GetChildByOptimizer(item.Value));
          }
          foreach (var item in e.Items) {
            if (GetChildByOptimizer(item.Value) == null && item.Value.Name != "Placeholder") {
              this.childHiveTasks.Add(new OptimizerHiveTask(item.Value));
            }
          }
        }
        finally { childHiveTasksLock.ExitWriteLock(); }
      }
    }
    private void Optimizers_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (syncTasksWithOptimizers && this.ItemTask.ComputeInParallel) {
        childHiveTasksLock.EnterWriteLock();
        try {
          foreach (var item in e.Items) {
            this.childHiveTasks.Remove(this.GetChildByOptimizer(item.Value));
          }
        }
        finally { childHiveTasksLock.ExitWriteLock(); }
      }
    }
    private void Optimizers_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (syncTasksWithOptimizers && this.ItemTask.ComputeInParallel) {
        childHiveTasksLock.EnterWriteLock();
        try {
          foreach (var item in e.Items) {
            this.childHiveTasks.Remove(this.GetChildByOptimizer(item.Value));
          }
        }
        finally { childHiveTasksLock.ExitWriteLock(); }
      }
    }

    /// <summary>
    /// if this.Optimizer is Experiment
    ///   replace the child-optimizer in the experiment
    /// if this.Optimizer is BatchRun
    ///   add the runs from the optimizerTask to the batchrun and replace the Optimizer
    /// </summary>
    public override void IntegrateChild(ItemTask task, Guid childTaskId) {
      var optimizerTask = (OptimizerTask)task;
      syncTasksWithOptimizers = false; // don't sync with optimizers during this method

      if (this.ItemTask != null && this.ItemTask.Item != null) {
        if (this.ItemTask.Item is Optimization.Experiment) {
          UpdateOptimizerInExperiment(this.ItemTask.OptimizerAsExperiment, optimizerTask);
        } else if (this.ItemTask.Item is Optimization.BatchRun) {
          UpdateOptimizerInBatchRun(this.ItemTask.OptimizerAsBatchRun, optimizerTask);
        }
      }

      IEnumerable<HiveTask> childs = this.ChildHiveTasks.Where(j => j.Task.Id == childTaskId);
      //TODO: in very rare cases childs is empty. This shouldn't be the case and should be further investigated. 
      if (childs.Count() > 0) {
        OptimizerHiveTask child = childs.First() as OptimizerHiveTask;

        if (child != null && !optimizerTask.ComputeInParallel) {
          child.syncTasksWithOptimizers = false;
          child.ItemTask = optimizerTask;
          child.syncTasksWithOptimizers = true;
        }
      }

      syncTasksWithOptimizers = true;
    }

    /// <summary>
    /// Adds the runs from the optimizerTask to the batchrun and replaces the Optimizer 
    /// Sideeffect: the optimizerTask.Optimizer will be prepared (scopes are deleted and executionstate will be reset)
    /// </summary>
    private void UpdateOptimizerInBatchRun(BatchRun batchRun, OptimizerTask optimizerTask) {
      itemTaskLock.EnterWriteLock();
      try {
        if (optimizerTask.Item is IAlgorithm) {
          // only set the first optimizer as Optimizer. if every time the Optimizer would be set, the runs would be cleared each time
          if (batchRun.Optimizer == null) {
            batchRun.Optimizer = (IOptimizer)optimizerTask.Item.Clone();
            batchRun.Optimizer.Runs.Clear();
          }

          foreach (IRun run in optimizerTask.Item.Runs) {
            run.Name = GetNewRunName(run, batchRun.Runs);
            batchRun.Optimizer.Runs.Add(run);
          }
        } else {
          // only set the first optimizer as Optimizer. if every time the Optimizer would be set, the runs would be cleared each time
          if (batchRun.Optimizer == null) {
            batchRun.Optimizer = optimizerTask.Item;
          }
          foreach (IRun run in optimizerTask.Item.Runs) {
            if (batchRun.Runs.Contains(run)) continue;
            run.Name = GetNewRunName(run, batchRun.Runs);
            batchRun.Runs.Add(run);
          }
        }
      }
      finally {
        itemTaskLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// replace the child-optimizer in the experiment
    /// Sideeffect: the optimizerTask.Optimizer will be prepared (scopes are deleted and executionstate will be reset)
    /// </summary>
    private void UpdateOptimizerInExperiment(Optimization.Experiment experiment, OptimizerTask optimizerTask) {
      itemTaskLock.EnterWriteLock();
      try {
        if (optimizerTask.IndexInParentOptimizerList < 0)
          throw new IndexOutOfRangeException("IndexInParentOptimizerList must be equal or greater than zero! The Task is invalid and the optimizer-tree cannot be reassembled.");

        while (experiment.Optimizers.Count < optimizerTask.IndexInParentOptimizerList) {
          experiment.Optimizers.Add(new UserDefinedAlgorithm("Placeholder")); // add dummy-entries to Optimizers so that its possible to insert the optimizerTask at the correct position
        }
        if (experiment.Optimizers.Count < optimizerTask.IndexInParentOptimizerList + 1) {
          experiment.Optimizers.Add(optimizerTask.Item);
        } else {
          // if ComputeInParallel==true, don't replace the optimizer (except it is still a Placeholder)
          // this is because Jobs with ComputeInParallel get submitted to hive with their child-optimizers deleted
          if (!optimizerTask.ComputeInParallel || experiment.Optimizers[optimizerTask.IndexInParentOptimizerList].Name == "Placeholder") {
            experiment.Optimizers[optimizerTask.IndexInParentOptimizerList] = optimizerTask.Item;
          }
        }
      }
      finally {
        itemTaskLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Sets the IndexInParentOptimizerList property of the OptimizerJob
    /// according to the position in the OptimizerList of the parentHiveTask.Task
    /// Recursively updates all the child-jobs as well
    /// </summary>
    internal void SetIndexInParentOptimizerList(OptimizerHiveTask parentHiveTask) {
      if (parentHiveTask != null) {
        if (parentHiveTask.ItemTask.Item is Optimization.Experiment) {
          this.ItemTask.IndexInParentOptimizerList = parentHiveTask.ItemTask.OptimizerAsExperiment.Optimizers.IndexOf(this.ItemTask.Item);
        } else if (parentHiveTask.ItemTask.Item is Optimization.BatchRun) {
          this.ItemTask.IndexInParentOptimizerList = 0;
        } else {
          throw new NotSupportedException("Only Experiment and BatchRuns are supported");
        }
      }
      childHiveTasksLock.EnterReadLock();
      try {
        foreach (OptimizerHiveTask child in childHiveTasks) {
          child.SetIndexInParentOptimizerList(this);
        }
      }
      finally { childHiveTasksLock.ExitReadLock(); }
    }

    public override void AddChildHiveTask(HiveTask hiveTask) {
      base.AddChildHiveTask(hiveTask);
      var optimizerHiveTask = (OptimizerHiveTask)hiveTask;
      syncTasksWithOptimizers = false;
      if (this.ItemTask != null && optimizerHiveTask.ItemTask != null) {
        OptimizerTask optimizerTaskClone = (OptimizerTask)optimizerHiveTask.ItemTask.Clone();

        if (this.ItemTask.Item is Optimization.Experiment) {
          if (!this.ItemTask.OptimizerAsExperiment.Optimizers.Contains(optimizerHiveTask.ItemTask.Item)) {
            UpdateOptimizerInExperiment(this.ItemTask.OptimizerAsExperiment, optimizerHiveTask.ItemTask);
          }
        } else if (this.ItemTask.Item is Optimization.BatchRun) {
          UpdateOptimizerInBatchRun(this.ItemTask.OptimizerAsBatchRun, optimizerHiveTask.ItemTask);
        }

        optimizerHiveTask.syncTasksWithOptimizers = false;
        optimizerHiveTask.ItemTask = optimizerTaskClone;
        optimizerHiveTask.syncTasksWithOptimizers = true;
      }
      syncTasksWithOptimizers = true;
    }

    /// <summary>
    /// Creates a TaskData object containing the Task and the IJob-Object as byte[]
    /// </summary>
    /// <param name="withoutChildOptimizers">
    ///   if true the Child-Optimizers will not be serialized (if the task contains an Experiment)
    /// </param>
    public override TaskData GetAsTaskData(bool withoutChildOptimizers, out List<IPluginDescription> plugins) {
      if (ItemTask == null) {
        plugins = new List<IPluginDescription>();
        return null;
      }

      IEnumerable<Type> usedTypes;
      byte[] jobByteArray;
      if (withoutChildOptimizers && ItemTask.Item is Optimization.Experiment) {
        OptimizerTask clonedJob = (OptimizerTask)ItemTask.Clone(); // use a cloned task, so that the childHiveJob don't get confused
        clonedJob.OptimizerAsExperiment.Optimizers.Clear();
        jobByteArray = PersistenceUtil.Serialize(clonedJob, out usedTypes);
      } else if (withoutChildOptimizers && ItemTask.Item is Optimization.BatchRun) {
        OptimizerTask clonedJob = (OptimizerTask)ItemTask.Clone();
        clonedJob.OptimizerAsBatchRun.Optimizer = null;
        jobByteArray = PersistenceUtil.Serialize(clonedJob, out usedTypes);
      } else if (ItemTask.Item is IAlgorithm) {
        ((IAlgorithm)ItemTask.Item).StoreAlgorithmInEachRun = false; // avoid storing the algorithm in runs to reduce size
        jobByteArray = PersistenceUtil.Serialize(ItemTask, out usedTypes);
      } else {
        jobByteArray = PersistenceUtil.Serialize(ItemTask, out usedTypes);
      }

      TaskData jobData = new TaskData() { TaskId = task.Id, Data = jobByteArray };
      plugins = PluginUtil.GetPluginsForTask(usedTypes, ItemTask);
      return jobData;
    }

    public OptimizerHiveTask GetChildByOptimizerTask(OptimizerTask optimizerTask) {
      childHiveTasksLock.EnterReadLock();
      try {
        foreach (OptimizerHiveTask child in childHiveTasks) {
          if (child.ItemTask == optimizerTask)
            return child;
        }
        return null;
      }
      finally { childHiveTasksLock.ExitReadLock(); }
    }

    public HiveTask<OptimizerTask> GetChildByOptimizer(IOptimizer optimizer) {
      childHiveTasksLock.EnterReadLock();
      try {
        foreach (OptimizerHiveTask child in childHiveTasks) {
          if (child.ItemTask.Item == optimizer)
            return child;
        }
        return null;
      }
      finally { childHiveTasksLock.ExitReadLock(); }
    }

    public void ExecuteReadActionOnItemTask(Action action) {
      itemTaskLock.EnterReadLock();
      try {
        action();
      }
      finally {
        itemTaskLock.ExitReadLock();
      }
    }

    #region Helpers
    /// <summary>
    /// Parses the run numbers out of runs and renames the run to the next number
    /// </summary>
    private static string GetNewRunName(IRun run, RunCollection runs) {
      int idx = run.Name.IndexOf("Run ") + 4;

      if (idx == 3 || runs.Count == 0)
        return run.Name;

      int maxRunNumber = int.MinValue;
      foreach (IRun r in runs) {
        int number = GetRunNumber(r.Name);
        maxRunNumber = Math.Max(maxRunNumber, number);
      }

      return run.Name.Substring(0, idx) + (maxRunNumber + 1).ToString();
    }

    /// <summary>
    /// Parses the number of a Run out of its name. Example "Genetic Algorithm Run 3" -> 3
    /// </summary>
    private static int GetRunNumber(string runName) {
      int idx = runName.IndexOf("Run ") + 4;
      if (idx == 3) {
        return 0;
      } else {
        return int.Parse(runName.Substring(idx, runName.Length - idx));
      }
    }
    #endregion
  }
}
