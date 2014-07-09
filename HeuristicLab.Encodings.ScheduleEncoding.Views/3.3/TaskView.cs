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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Encodings.ScheduleEncoding.Views {
  [View("TaskView")]
  [Content(typeof(Task))]
  public partial class TaskView : ItemView {
    protected bool SuppressEvents = false;

    public new Task Content {
      get { return (Task)base.Content; }
      set { base.Content = value; }
    }

    public TaskView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= Content_PropertyChanged;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        taskNrTextBox.Text = String.Empty;
        resourceNrTextBox.Text = String.Empty;
        jobNrTextBox.Text = String.Empty;
        durationTextBox.Text = String.Empty;
      } else {
        SuppressEvents = true;
        try {
          taskNrTextBox.Text = Content.TaskNr.ToString();
          resourceNrTextBox.Text = Content.ResourceNr.ToString();
          jobNrTextBox.Text = Content.JobNr.ToString();
          durationTextBox.Text = Content.Duration.ToString();
        } finally { SuppressEvents = false; }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      taskNrTextBox.Enabled = !ReadOnly && !Locked && Content != null && !Content.IsScheduled;
      resourceNrTextBox.Enabled = !ReadOnly && !Locked && Content != null && !Content.IsScheduled;
      jobNrTextBox.Enabled = !ReadOnly && !Locked && Content != null && !Content.IsScheduled;
      durationTextBox.Enabled = !ReadOnly && !Locked && Content != null && !Content.IsScheduled;
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      SuppressEvents = true;
      try {
        switch (e.PropertyName) {
          case "TaskNr":
            taskNrTextBox.Text = Content.TaskNr.ToString();
            break;
          case "ResourceNr":
            resourceNrTextBox.Text = Content.ResourceNr.ToString();
            break;
          case "JobNr":
            jobNrTextBox.Text = Content.JobNr.ToString();
            break;
          case "Duration":
            durationTextBox.Text = Content.Duration.ToString();
            break;
          case "IsScheduled":
            SetEnabledStateOfControls();
            break;
          default:
            break;
        }
      } finally {
        SuppressEvents = false;
      }
    }

    private void taskNrTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && taskNrTextBox.Enabled) {
        int value;
        if (int.TryParse(taskNrTextBox.Text, out value)) {
          Content.TaskNr = value;
          errorProvider.SetError(taskNrTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(taskNrTextBox, "Please provide a valid integer.");
        }
      }
    }

    private void resourceNrTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && taskNrTextBox.Enabled) {
        int value;
        if (int.TryParse(resourceNrTextBox.Text, out value)) {
          Content.ResourceNr = value;
          errorProvider.SetError(resourceNrTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(resourceNrTextBox, "Please provide a valid integer.");
        }
      }
    }

    private void jobNrTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && taskNrTextBox.Enabled) {
        int value;
        if (int.TryParse(jobNrTextBox.Text, out value)) {
          Content.JobNr = value;
          errorProvider.SetError(jobNrTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(jobNrTextBox, "Please provide a valid integer.");
        }
      }
    }

    private void durationTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && resourceNrTextBox.Enabled) {
        double value;
        if (double.TryParse(durationTextBox.Text, out value)) {
          Content.Duration = value;
          errorProvider.SetError(durationTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(durationTextBox, "Please provide a valid double.");
        }
      }
    }
  }
}
