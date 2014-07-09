#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.Views {
  [View("OptimizerHiveTask View")]
  [Content(typeof(OptimizerHiveTask), true)]
  public partial class OptimizerHiveTaskView : HiveTaskView {
    public new OptimizerHiveTask Content {
      get { return (OptimizerHiveTask)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    public OptimizerHiveTaskView() {
      InitializeComponent();
    }

    protected override void Job_ItemChanged(object sender, EventArgs e) {
      if (Content != null && Content.Task != null && Content.ItemTask.Item != null) {
        Content.ExecuteReadActionOnItemTask(new Action(delegate() {
          runCollectionViewHost.Content = Content.ItemTask.Item.Runs;
        }));
      } else {
        runCollectionViewHost.Content = null;
      }
    }

    #region Content Events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.IsControllableChanged += new EventHandler(Content_IsControllableChanged);
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, Content.Progress);
    }

    protected override void DeregisterContentEvents() {
      Content.IsControllableChanged -= new EventHandler(Content_IsControllableChanged);
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this, false);
      base.DeregisterContentEvents();
    }

    protected virtual void Content_IsControllableChanged(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }
    #endregion

    #region Child Control Events
    private void restartButton_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(ResumeTaskAsync);
      task.ContinueWith((t) => {
        Content.Progress.Finish();
        ErrorHandling.ShowErrorDialog(this, "An error occured while resuming the task.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void pauseButton_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(PauseTaskAsync);
      task.ContinueWith((t) => {
        Content.Progress.Finish();
        ErrorHandling.ShowErrorDialog(this, "An error occured while pausing the task.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void stopButton_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(StopTaskAsync);
      task.ContinueWith((t) => {
        Content.Progress.Finish();
        ErrorHandling.ShowErrorDialog(this, "An error occured while stopping the task.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }
    #endregion

    private void PauseTaskAsync() {
      Content.Progress.Start("Pausing task. Please be patient for the command to take effect.");
      Content.Pause();
      Content.Progress.Finish();
    }

    private void StopTaskAsync() {
      Content.Progress.Start("Stopping task. Please be patient for the command to take effect.");
      Content.Stop();
      Content.Progress.Finish();
    }

    private void ResumeTaskAsync() {
      Content.Progress.Start("Resuming task. Please be patient for the command to take effect.");
      Content.Restart();
      Content.Progress.Finish();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      this.restartButton.Enabled = Content != null && Content.IsControllable && !Content.Task.Command.HasValue && (Content.Task.State == TaskState.Paused || Content.Task.State == TaskState.Failed || Content.Task.State == TaskState.Aborted);
      this.pauseButton.Enabled = Content != null && Content.IsControllable && !Content.Task.Command.HasValue && Content.Task.State == TaskState.Calculating;
      this.stopButton.Enabled = Content != null && Content.IsControllable && !Content.Task.Command.HasValue && (Content.Task.State == TaskState.Calculating || Content.Task.State == TaskState.Waiting || Content.Task.State == TaskState.Paused);
    }
  }
}
