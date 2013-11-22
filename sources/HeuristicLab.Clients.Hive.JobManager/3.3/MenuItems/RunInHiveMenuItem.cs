#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer.MenuItems {
  public class RunInHiveMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Run In Hive"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Hive" }; }
    }

    public override int Position {
      get { return 10002; }
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;

      if (activeView != null && activeView.Content != null && !activeView.Locked) {
        var content = activeView.Content as IItem;
        if (content != null) {
          Type contentType = content.GetType();
          ToolStripItem.Enabled = ItemTask.IsTypeSupported(contentType);
        } else {
          ToolStripItem.Enabled = false;
        }
      }
    }

    private IProgress progress;
    private IItem content;
    public override void Execute() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      content = activeView.Content as IItem;

      //IOptimizer and IExecutables need some special care
      if (content is IOptimizer) {
        ((IOptimizer)content).Runs.Clear();
      }
      if (content is IExecutable) {
        IExecutable exec = content as IExecutable;
        if (exec.ExecutionState != ExecutionState.Prepared) {
          exec.Prepare();
        }
      }

      HiveClient.Instance.Refresh();

      ItemTask hiveTask = ItemTask.GetItemTaskForItem(content);
      HiveTask task = hiveTask.CreateHiveTask();
      RefreshableJob rJob = new RefreshableJob();
      rJob.Job.Name = content.ItemName;
      rJob.HiveTasks.Add(task);
      task.ItemTask.ComputeInParallel = content is Experiment || content is BatchRun;

      progress = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().AddOperationProgressToContent(this.content, "Uploading to Hive...");
      rJob.Progress = progress;
      progress.ProgressStateChanged += progress_ProgressStateChanged;

      HiveClient.StartJob(new Action<Exception>(HandleEx), rJob, new CancellationToken());
    }

    private void progress_ProgressStateChanged(object sender, EventArgs e) {
      if (progress.ProgressState != ProgressState.Started) {
        MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromContent(content);
        progress.ProgressStateChanged -= progress_ProgressStateChanged;
      }
    }

    private void HandleEx(Exception ex) {
      MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromContent(content);
      progress.ProgressStateChanged -= progress_ProgressStateChanged;
      ErrorHandling.ShowErrorDialog("Error uploading tasks", ex);
    }
  }
}
