#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Views {
  [View("StateLogGanttChartList View")]
  [Content(typeof(StateLogListList), true)]
  public sealed partial class StateLogGanttChartListView : ItemView {
    public new StateLogListList Content {
      get { return (StateLogListList)base.Content; }
      set { base.Content = value; }
    }

    public StateLogGanttChartListView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      // Deregister your event handlers here
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // Register your event handlers here
    }

    #region Event Handlers (Content)
    // Put event handlers of the content here
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ganttChart.Reset();
      } else {
        ganttChart.Reset();
        SetupCategories(ganttChart);
        if (Content.Count > 0) {
          DateTime maxValue = Content.Max(x => x.Count > 0 ? x.Max(y => y.DateTime) : DateTime.MinValue);
          DateTime minValue = Content.Min(x => x.Count > 0 ? x.Min(y => y.DateTime) : DateTime.MinValue);
          DateTime upperLimit;
          if (Content.All(x => x.Count > 0 ? (x.Last().State == TaskState.Finished || x.Last().State == TaskState.Failed || x.Last().State == TaskState.Aborted) : true)) {
            upperLimit = DateTime.FromOADate(Math.Min(DateTime.Now.AddSeconds(10).ToOADate(), maxValue.AddSeconds(10).ToOADate()));
          } else {
            upperLimit = DateTime.Now;
          }

          if ((upperLimit - minValue) > TimeSpan.FromDays(1)) {
            this.ganttChart.chart.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
          } else {
            this.ganttChart.chart.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
          }

          for (int i = 0; i < Content.Count; i++) {
            for (int j = 0; j < Content[i].Count - 1; j++) {
              if (Content[i][j].State != TaskState.Offline)
                AddData(ganttChart, i.ToString(), Content[i][j], Content[i][j + 1], upperLimit);
            }
            if (Content[i].Count > 0) {
              AddData(ganttChart, i.ToString(), Content[i][Content[i].Count - 1], null, upperLimit);
            }
          }
        }
      }
    }

    public static void SetupCategories(GanttChart ganttChart) {
      ganttChart.AddCategory(TaskState.Offline.ToString(), Color.Gainsboro);
      ganttChart.AddCategory(TaskState.Waiting.ToString(), Color.NavajoWhite);
      ganttChart.AddCategory(TaskState.Paused.ToString(), Color.PaleVioletRed);
      ganttChart.AddCategory(TaskState.Transferring.ToString(), Color.CornflowerBlue);
      ganttChart.AddCategory(TaskState.Calculating.ToString(), Color.DarkGreen);
      ganttChart.AddCategory(TaskState.Finished.ToString(), Color.White);
      ganttChart.AddCategory(TaskState.Aborted.ToString(), Color.Orange);
      ganttChart.AddCategory(TaskState.Failed.ToString(), Color.Red);
    }

    public static void AddData(GanttChart ganttChart, string name, StateLog from, StateLog to, DateTime upperLimit) {
      DateTime until = to != null ? to.DateTime : upperLimit;
      TimeSpan duration = until - from.DateTime;
      string tooltip = string.Format("Task: {0} " + Environment.NewLine + "Task Id: {1}" + Environment.NewLine + "State: {2} " + Environment.NewLine + "Duration: {3} " + Environment.NewLine + "{4} - {5}", from.TaskName, from.TaskId, from.State, duration, from.DateTime, until);
      if (!string.IsNullOrEmpty(from.Exception))
        tooltip += Environment.NewLine + from.Exception;
      ganttChart.AddData(name, from.State.ToString(), from.DateTime, until, tooltip, false);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // Enable or disable controls based on whether the content is null or the view is set readonly
    }

    #region Event Handlers (child controls)
    // Put event handlers of child controls here.
    #endregion
  }
}
