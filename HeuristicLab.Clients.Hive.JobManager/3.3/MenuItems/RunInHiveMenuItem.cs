﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.JobManager.Views;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager {
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
          return;
        }
      }
      ToolStripItem.Enabled = false;
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
      rJob.Job.Name = content.ToString();
      rJob.HiveTasks.Add(task);
      task.ItemTask.ComputeInParallel = content is Experiment || content is BatchRun;

      var hiveResourceSelectorDialog = new HiveResourceSelectorDialog(rJob.Job.Id, rJob.Job.ProjectId);

      if (HiveClient.Instance.Projects.Count == 1) {
        var project = HiveClient.Instance.Projects.FirstOrDefault();
        if (project != null && project.Id != Guid.Empty)
          hiveResourceSelectorDialog.SelectedProjectId = project.Id;
      }

      if (hiveResourceSelectorDialog.ShowDialog((UserControl)activeView) == DialogResult.OK) {
        var selectedProject = hiveResourceSelectorDialog.SelectedProject;
        if (selectedProject != null) {
          rJob.Job.ProjectId = selectedProject.Id;
          rJob.Job.ResourceIds = hiveResourceSelectorDialog.SelectedResources.Select(x => x.Id).ToList();

          progress = Progress.Show(this.content, "Uploading to Hive...", ProgressMode.Indeterminate);
          rJob.Progress = progress;
          progress.ProgressStateChanged += progress_ProgressStateChanged;

          HiveClient.StartJob(new Action<Exception>(HandleEx), rJob, new CancellationToken());
        }
      }
    }

    private void progress_ProgressStateChanged(object sender, EventArgs e) {
      if (progress.ProgressState != ProgressState.Started) {
        Progress.Hide(content);
        progress.ProgressStateChanged -= progress_ProgressStateChanged;
      }
    }

    private void HandleEx(Exception ex) {
      Progress.Hide(content);
      progress.ProgressStateChanged -= progress_ProgressStateChanged;
      ErrorHandlingUI.ShowErrorDialog("Error uploading tasks", ex);
    }
  }
}
