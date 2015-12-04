#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.ScheduleEncoding.Views {
  [View("Schedule View")]
  [Content(typeof(Schedule), true)]
  public partial class ScheduleView : NamedItemView {
    public ScheduleView() {
      InitializeComponent();
    }


    protected override void DeregisterContentEvents() {
      Content.QualityChanged -= new EventHandler(Content_QualityChanged);
      Content.ResourcesChanged -= new EventHandler(Content_ResourcesChanged);

      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.QualityChanged += new EventHandler(Content_QualityChanged);
      Content.ResourcesChanged += new EventHandler(Content_ResourcesChanged);
    }


    public new Schedule Content {
      get { return (Schedule)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ResetGanttChart();
      } else {
        RedrawGanttChart(Content);
      }
    }

    private void ResetGanttChart() {
      ganttChart.Reset();
    }

    private void RedrawGanttChart(Schedule content) {
      ResetGanttChart();
      foreach (Resource resource in content.Resources) {
        foreach (ScheduledTask task in resource.Tasks) {
          int categoryNr = task.JobNr;
          string categoryName = "Job" + categoryNr;
          string toolTip = categoryName + " - " + "Task#" + task.TaskNr;

          ganttChart.AddData("Resource" + resource.Index,
            categoryNr,
            task.TaskNr,
            task.StartTime,
            task.EndTime,
            toolTip);
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      ganttChart.Enabled = Content != null;
    }


    private void Content_QualityChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_QualityChanged), sender, e);
      else {
        if (Content == null) {
          ResetGanttChart();
        } else {
          RedrawGanttChart(Content);
        }
      }
    }

    private void Content_ResourcesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ResourcesChanged), sender, e);
      else {
        if (Content == null) {
          ResetGanttChart();
        } else {
          RedrawGanttChart(Content);
        }
      }
    }
  }
}
