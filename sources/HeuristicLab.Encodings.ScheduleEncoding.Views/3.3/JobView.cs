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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Encodings.ScheduleEncoding.Views {
  [View("JobView")]
  [Content(typeof(Job))]
  public partial class JobView : ItemView {
    private ItemListView<Task> tasksView;
    protected bool SuppressEvents = false;

    public new Job Content {
      get { return (Job)base.Content; }
      set { base.Content = value; }
    }

    public JobView() {
      InitializeComponent();
      tasksView = new ItemListView<Task> { Dock = DockStyle.Fill };
      tasksGroupBox.Controls.Add(tasksView);
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
        indexTextBox.Text = String.Empty;
        dueDateTextBox.Text = String.Empty;
        tasksView.Content = null;
      } else {
        SuppressEvents = true;
        try {
          indexTextBox.Text = Content.Index.ToString();
          dueDateTextBox.Text = Content.DueDate.ToString("r");
        } finally { SuppressEvents = false; }
        tasksView.Content = Content.Tasks;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      indexTextBox.Enabled = !ReadOnly && !Locked && Content != null;
      dueDateTextBox.Enabled = !ReadOnly && !Locked && Content != null;
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      SuppressEvents = true;
      try {
        switch (e.PropertyName) {
          case "Index":
            indexTextBox.Text = Content.Index.ToString();
            break;
          case "DueDate":
            dueDateTextBox.Text = Content.DueDate.ToString("r");
            break;
          case "Tasks":
            tasksView.Content = Content.Tasks;
            break;
          default:
            break;
        }
      } finally {
        SuppressEvents = false;
      }
    }

    private void indexTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && indexTextBox.Enabled) {
        int value;
        if (int.TryParse(indexTextBox.Text, out value)) {
          Content.Index = value;
          errorProvider.SetError(indexTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(indexTextBox, "Please provide a valid integer.");
        }
      }
    }

    private void dueDateTextBox_Validating(object sender, CancelEventArgs e) {
      if (!SuppressEvents && Content != null && dueDateTextBox.Enabled) {
        double value;
        if (double.TryParse(dueDateTextBox.Text, out value)) {
          Content.DueDate = value;
          errorProvider.SetError(dueDateTextBox, String.Empty);
        } else {
          e.Cancel = true;
          errorProvider.SetError(dueDateTextBox, "Please provide a valid double.");
        }
      }
    }
  }
}
