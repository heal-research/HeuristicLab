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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Views {
  [View("HiveTask View")]
  [Content(typeof(HiveTask), true)]
  [Content(typeof(HiveTask<>), false)]
  public partial class HiveTaskView : ItemView {
    public new HiveTask Content {
      get { return (HiveTask)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    public HiveTaskView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemTaskChanged += new EventHandler(Content_TaskItemChanged);
      Content.TaskChanged += new EventHandler(Content_TaskChanged);
      Content.TaskStateChanged += new EventHandler(Content_TaskStateChanged);
      Content.StateLogChanged += new EventHandler(Content_StateLogChanged);
    }

    protected override void DeregisterContentEvents() {
      Content.ItemTaskChanged -= new EventHandler(Content_TaskItemChanged);
      Content.TaskChanged -= new EventHandler(Content_TaskChanged);
      Content.TaskStateChanged -= new EventHandler(Content_TaskStateChanged);
      Content.StateLogChanged -= new EventHandler(Content_StateLogChanged);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null && Content.Task != null) {
        computeInParallelCheckBox.Checked = Content.ItemTask.ComputeInParallel;
      } else {
        computeInParallelCheckBox.Checked = false;
      }
      Content_TaskItemChanged(this, EventArgs.Empty);
      Content_TaskChanged(this, EventArgs.Empty);
      Content_TaskStateChanged(this, EventArgs.Empty);
    }

    protected virtual void RegisterTaskEvents() {
      if (Content != null && Content.Task != null) {
        Content.ItemTask.ComputeInParallelChanged += new EventHandler(OptimizerJob_ComputeInParallelChanged);
        Content.ItemTask.ItemChanged += new EventHandler(Job_ItemChanged);
      }
    }

    protected virtual void DeregisterTaskEvents() {
      if (Content != null && Content.Task != null) {
        Content.ItemTask.ComputeInParallelChanged -= new EventHandler(OptimizerJob_ComputeInParallelChanged);
        Content.ItemTask.ItemChanged -= new EventHandler(Job_ItemChanged);
      }
    }

    #region Content Events
    protected virtual void Content_TaskChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_TaskChanged), sender, e);
      } else {
        if (Content != null && Content.Task != null) {
          this.jobIdTextBox.Text = Content.Task.Id.ToString();
          this.dateCreatedTextBox.Text = Content.Task.DateCreated.HasValue ? Content.Task.DateCreated.ToString() : string.Empty;
          if (Content.Task.Priority >= 0 && Content.Task.Priority < priorityComboBox.Items.Count) {
            this.priorityComboBox.SelectedIndex = Content.Task.Priority;
          } else {
            this.priorityComboBox.SelectedIndex = 1;
          }
          this.coresNeededComboBox.Text = Content.Task.CoresNeeded.ToString();
          this.memoryNeededComboBox.Text = Content.Task.MemoryNeeded.ToString();
        } else {
          this.jobIdTextBox.Text = string.Empty;
          this.dateCreatedTextBox.Text = string.Empty;
          this.priorityComboBox.SelectedIndex = 1;
          this.coresNeededComboBox.Text = "1";
          this.memoryNeededComboBox.Text = "128";
        }
      }
    }

    protected virtual void Content_TaskItemChanged(object sender, EventArgs e) {
      RegisterTaskEvents();
      Job_ItemChanged(this, e);
    }

    protected virtual void Job_ItemChanged(object sender, EventArgs e) {
      if (Content != null && Content.Task != null && Content.ItemTask.Item != null) {
      } else {
      }
    }

    protected virtual void Content_TaskStateChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_TaskStateChanged), sender, e);
      } else {
        if (Content != null && Content.Task != null) {
          this.stateTextBox.Text = Content.Task.State.ToString();
          this.commandTextBox.Text = Content.Task.Command.ToString();
          this.executionTimeTextBox.Text = Content.Task.ExecutionTime.ToString();
          this.dateFinishedTextBox.Text = Content.Task.DateFinished.ToString();
          this.exceptionTextBox.Text = Content.Task.CurrentStateLog != null ? Content.Task.CurrentStateLog.Exception : string.Empty;
          this.lastUpdatedTextBox.Text = Content.Task.LastTaskDataUpdate.ToString();
        } else {
          this.stateTextBox.Text = string.Empty;
          this.commandTextBox.Text = string.Empty;
          this.executionTimeTextBox.Text = string.Empty;
          this.dateCalculatedText.Text = string.Empty;
          this.dateFinishedTextBox.Text = string.Empty;
          this.exceptionTextBox.Text = string.Empty;
          this.lastUpdatedTextBox.Text = string.Empty;
        }
        Content_StateLogChanged(this, EventArgs.Empty);
        SetEnabledStateOfControls();
      }
    }

    protected virtual void OptimizerJob_ComputeInParallelChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(OptimizerJob_ComputeInParallelChanged), sender, e);
      } else {
        computeInParallelCheckBox.Checked = Content.ItemTask.ComputeInParallel;
      }
    }

    protected virtual void Content_StateLogChanged(object sender, EventArgs e) {
      if (Content != null) {
        if (Content.ItemTask.ComputeInParallel) {
          this.stateLogViewHost.Content = Content.ChildStateLogList;
        } else {
          this.stateLogViewHost.Content = Content.StateLog;
        }
      } else {
        this.stateLogViewHost.Content = null;
      }
    }
    #endregion

    #region Child Control Events
    protected virtual void computeInParallelCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null && Content.ItemTask != null) {
        this.Content.ItemTask.ComputeInParallel = this.computeInParallelCheckBox.Checked;
      }
    }

    protected virtual void exceptionTextBox_DoubleClick(object sender, EventArgs e) {
      using (TextDialog dialog = new TextDialog("Exception", exceptionTextBox.Text, ReadOnly || !Content.CanChangeDescription)) {
        if (dialog.ShowDialog(this) == DialogResult.OK)
          Content.Description = dialog.Content;
      }
    }

    protected virtual void modifyItemButton_Click(object sender, EventArgs e) {
      MainFormManager.MainForm.ShowContent(Content.ItemTask.Item);
    }
    #endregion

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      this.jobIdTextBox.ReadOnly = true;
      this.stateTextBox.ReadOnly = true;
      this.commandTextBox.ReadOnly = true;
      this.executionTimeTextBox.ReadOnly = true;
      this.dateCreatedTextBox.ReadOnly = true;
      this.dateCalculatedText.ReadOnly = true;
      this.dateFinishedTextBox.ReadOnly = true;
      this.exceptionTextBox.ReadOnly = true;
      this.lastUpdatedTextBox.ReadOnly = true;

      this.priorityComboBox.Enabled = !this.ReadOnly;
      this.coresNeededComboBox.Enabled = !this.ReadOnly;
      this.memoryNeededComboBox.Enabled = !this.ReadOnly;
      this.computeInParallelCheckBox.Enabled = !this.ReadOnly && this.Content != null && this.Content.ItemTask != null && this.Content.ItemTask.IsParallelizable;

      this.modifyItemButton.Enabled = (Content != null && Content.ItemTask.Item != null && (Content.Task.State == TaskState.Paused || Content.Task.State == TaskState.Offline || Content.Task.State == TaskState.Finished || Content.Task.State == TaskState.Failed || Content.Task.State == TaskState.Aborted));
    }

    private void priorityComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null && Content.Task != null && Content.Task.Priority != priorityComboBox.SelectedIndex) {
        Content.Task.Priority = priorityComboBox.SelectedIndex;
      }
    }

    private void coresNeededComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null && Content.Task != null) {
        int cores;
        if (int.TryParse(coresNeededComboBox.SelectedItem.ToString(), out cores) && Content.Task.CoresNeeded != cores)
          Content.Task.CoresNeeded = cores;
      }
    }
  }
}
