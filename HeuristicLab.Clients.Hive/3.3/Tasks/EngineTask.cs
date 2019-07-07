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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Clients.Hive {
  [StorableType("FFDE8A4B-FDEC-47AA-B8E2-B16E5B513055")]
  public class EngineTask : ItemTask {
    public override HiveTask CreateHiveTask() {
      //only used when deserializing, so no problem with parentscope
      return new EngineHiveTask(this, null);
    }

    [Storable]
    protected IOperation initialOperation;
    public IOperation InitialOperation {
      get { return initialOperation; }
      set { initialOperation = value; }
    }

    public new IEngine Item {
      get { return (IEngine)base.Item; }
      set { base.Item = value; }
    }

    public override TimeSpan ExecutionTime {
      get { return Item.ExecutionTime; }
    }

    public override ExecutionState ExecutionState {
      get { return Item.ExecutionState; }
    }

    #region constructors and cloning
    public EngineTask(IOperation initialOperation, IEngine engine) {
      this.initialOperation = initialOperation;
      this.Item = engine;
    }

    public EngineTask(IEngine engine) : base(engine) { }

    [StorableConstructor]
    protected EngineTask(StorableConstructorFlag _) : base(_) { }
    protected EngineTask(EngineTask original, Cloner cloner)
      : base(original, cloner) {
      this.initialOperation = cloner.Clone(original.initialOperation);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EngineTask(this, cloner);
    }
    #endregion

    public override bool IsParallelizable {
      get { return false; }
    }

    public override void Prepare() { }

    public override async void Start() {
      Item.Prepare(initialOperation);
      await Item.StartAsync();
    }

    public override void Pause() {
      Item.Pause();
    }

    public override void Stop() {
      Item.Stop();
    }

    protected override void RegisterItemEvents() {
      base.RegisterItemEvents();
      Item.Stopped += new EventHandler(engine_Stopped);
      Item.Paused += new EventHandler(Item_Paused);
      Item.Started += new EventHandler(Item_Started);
      Item.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(engine_ExceptionOccurred);
      Item.ExecutionStateChanged += new EventHandler(Item_ExecutionStateChanged);
      Item.ExecutionTimeChanged += new EventHandler(Item_ExecutionTimeChanged);
    }

    protected override void DeregisterItemEvents() {
      Item.Stopped -= new EventHandler(engine_Stopped);
      Item.Paused -= new EventHandler(Item_Paused);
      Item.Started -= new EventHandler(Item_Started);
      Item.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(engine_ExceptionOccurred);
      Item.ExecutionStateChanged -= new EventHandler(Item_ExecutionStateChanged);
      Item.ExecutionTimeChanged -= new EventHandler(Item_ExecutionTimeChanged);
      base.DeregisterItemEvents();
    }

    private void engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnTaskFailed(e);
    }

    private void engine_Stopped(object sender, EventArgs e) {
      OnTaskStopped();
    }

    private void Item_Paused(object sender, EventArgs e) {
      OnTaskPaused();
    }

    private void Item_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }

    private void Item_ExecutionStateChanged(object sender, EventArgs e) {
      OnExecutionStateChanged();
    }

    private void Item_Started(object sender, EventArgs e) {
      OnTaskStarted();
    }

    public override bool CanChangeDescription {
      get { return false; }
    }

    public override bool CanChangeName {
      get { return false; }
    }

    public override string Description {
      get { return string.Empty; }
      set { throw new NotSupportedException(); }
    }

    public override string Name {
      get { return Item != null ? Item.ToString() : "Engine Task"; }
      set { throw new NotSupportedException(); }
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Operator; }
    }

    public override string ItemName {
      get { return "Engine Task"; }
    }
  }
}
