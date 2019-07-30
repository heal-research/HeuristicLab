#region License Information
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  [View("Jobs View")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class TaskView : ItemView {
    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set { base.Content = value; }
    }

    public TaskView() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.SlaveStatusChanged -= new EventHandler<Common.EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.SlaveStatusChanged += new EventHandler<Common.EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers
    void Content_SlaveStatusChanged(object sender, Common.EventArgs<StatusCommons> e) {
      if (InvokeRequired) {
        Action<object, Common.EventArgs<StatusCommons>> action = new Action<object, Common.EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
        Invoke(action, sender, e);
      } else {
        lstJobs.Items.Clear();
        foreach (TaskStatus job in e.Value.Jobs) {
          ListViewItem item = new ListViewItem();
          item.Text = job.TaskId.ToString();
          item.SubItems.Add(job.ExecutionTime.ToString());
          lstJobs.Items.Add(item);
        }
        if (e.Value.Jobs.Count == 0) {
          lstJobs.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
        } else {
          lstJobs.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }
    #endregion
  }
}
