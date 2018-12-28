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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Clients.Hive.Jobs {
  [Item("Optimizer Task", "Represents Task which executes a IOptimizer object.")]
  [StorableType("9A59FAAF-3F5C-4629-B22D-714488DC2678")]
  public class OptimizerTask : ItemTask {
    public override HiveTask CreateHiveTask() {
      return new OptimizerHiveTask(this);
    }

    public override bool IsParallelizable {
      get { return this.Item is Experiment || this.Item is BatchRun; }
    }

    public new IOptimizer Item {
      get { return (IOptimizer)base.Item; }
      set { base.Item = value; }
    }

    [Storable]
    private int indexInParentOptimizerList = -1;
    public int IndexInParentOptimizerList {
      get { return indexInParentOptimizerList; }
      set { this.indexInParentOptimizerList = value; }
    }

    public OptimizerTask(IOptimizer optimizer)
      : base(optimizer) {

      if (optimizer is Experiment || optimizer is BatchRun) {
        this.ComputeInParallel = true;
      } else {
        this.ComputeInParallel = false;
      }
    }
    [StorableConstructor]
    protected OptimizerTask(StorableConstructorFlag _) : base(_) { }
    protected OptimizerTask(OptimizerTask original, Cloner cloner)
      : base(original, cloner) {
      this.IndexInParentOptimizerList = original.IndexInParentOptimizerList;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OptimizerTask(this, cloner);
    }

    /// <summary>
    /// Casts the Optimizer to an Experiment. Returns null if cast was not successfull.
    /// </summary>
    public Experiment OptimizerAsExperiment {
      get { return Item as Experiment; }
    }

    /// <summary>
    /// Casts the Optimizer to an BatchRun. Returns null if cast was not successfull.
    /// </summary>
    public BatchRun OptimizerAsBatchRun {
      get { return Item as BatchRun; }
    }

    #region ITask Members
    public override ExecutionState ExecutionState {
      get { return Item.ExecutionState; }
    }

    public override TimeSpan ExecutionTime {
      get { return Item.ExecutionTime; }
    }

    public override void Prepare() {
      Item.Prepare();
    }

    public override async void Start() {
      if ((Item is Experiment && OptimizerAsExperiment.Optimizers.Count == 0) || // experiment would not fire OnStopped if it has 0 optimizers
          (Item is BatchRun && OptimizerAsBatchRun.Optimizer == null)) { // batchrun would not fire OnStopped if algorithm == null
        OnTaskStopped();
      } else {
        await Item.StartAsync();
      }
    }

    public override void Pause() {
      Item.Pause();
    }

    public override void Stop() {
      Item.Stop();
    }
    #endregion

    #region Optimizer Events
    protected override void RegisterItemEvents() {
      base.RegisterItemEvents();
      Item.Stopped += new EventHandler(optimizer_Stopped);
      Item.Paused += new EventHandler(optimizer_Paused);
      Item.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      Item.DescriptionChanged += new EventHandler(optimizer_DescriptionChanged);
      Item.NameChanged += new EventHandler(optimizer_NameChanged);
      Item.NameChanging += new EventHandler<CancelEventArgs<string>>(optimizer_NameChanging);
      Item.ExecutionStateChanged += new EventHandler(optimizer_ExecutionStateChanged);
      Item.ExecutionTimeChanged += new EventHandler(optimizer_ExecutionTimeChanged);
      Item.Started += new EventHandler(optimizer_Started);
    }

    protected virtual void DeregisterOptimizerEvents() {
      Item.Stopped -= new EventHandler(optimizer_Stopped);
      Item.Paused -= new EventHandler(optimizer_Paused);
      Item.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(optimizer_ExceptionOccurred);
      Item.DescriptionChanged -= new EventHandler(optimizer_DescriptionChanged);
      Item.NameChanged -= new EventHandler(optimizer_NameChanged);
      Item.NameChanging -= new EventHandler<CancelEventArgs<string>>(optimizer_NameChanging);
      Item.ExecutionStateChanged -= new EventHandler(optimizer_ExecutionStateChanged);
      Item.ExecutionTimeChanged -= new EventHandler(optimizer_ExecutionTimeChanged);
      Item.Started -= new EventHandler(optimizer_Started);
      base.DeregisterItemEvents();
    }

    protected void optimizer_NameChanging(object sender, CancelEventArgs<string> e) {
      OnNameChanging(new CancelEventArgs<string>(e.Value, e.Cancel));
    }

    protected void optimizer_NameChanged(object sender, EventArgs e) {
      this.OnNameChanged();
    }

    protected void optimizer_DescriptionChanged(object sender, EventArgs e) {
      this.OnDescriptionChanged();
    }

    protected virtual void optimizer_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnTaskFailed(e);
    }

    protected virtual void optimizer_Started(object sender, EventArgs e) {
      OnTaskStarted();
    }

    protected virtual void optimizer_Stopped(object sender, EventArgs e) {
      OnTaskStopped();
    }

    protected virtual void optimizer_Paused(object sender, EventArgs e) {
      OnTaskPaused();
    }

    protected virtual void optimizer_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }

    protected virtual void optimizer_ExecutionStateChanged(object sender, EventArgs e) {
      OnExecutionStateChanged();
    }
    #endregion

    #region INamedItem Members
    public override bool CanChangeDescription {
      get {
        if (Item == null)
          return false;
        else
          return Item.CanChangeDescription;
      }
    }

    public override bool CanChangeName {
      get {
        if (Item == null)
          return false;
        else
          return Item.CanChangeName;
      }
    }

    public override string Description {
      get {
        if (Item == null)
          return string.Empty;
        else
          return Item.Description;
      }
      set { Item.Description = value; }
    }

    public override string Name {
      get {
        if (Item == null)
          return "Optimizer Task";
        else
          return Item.Name;
      }
      set { Item.Name = value; }
    }
    #endregion
  }
}
