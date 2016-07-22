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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Clients.Hive.Views {
  [View("StateLogGanttChartList View")]
  [Content(typeof(StateLogListList), true)]
  public sealed partial class StateLogGanttChartListView : ItemView {
    private IList<LegendItem> invisibleLegendItems;

    public new StateLogListList Content {
      get { return (StateLogListList)base.Content; }
      set { base.Content = value; }
    }

    public StateLogGanttChartListView() {
      InitializeComponent();
      invisibleLegendItems = new List<LegendItem>();
      ganttChart.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(chart_MouseMove);
      ganttChart.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(chart_MouseDown);
      ganttChart.chart.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(chart_CustomizeLegend);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      ganttChart.Reset();
      if (Content != null) {
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

          for (int i = Content.Count - 1; i >= 0; i--) {
            for (int j = 0; j < Content[i].Count - 1; j++) {
              if (Content[i][j].State != TaskState.Offline && invisibleLegendItems.All(x => x.Name != Content[i][j].State.ToString()))
                AddData(ganttChart, i.ToString(), Content[i][j], Content[i][j + 1], upperLimit);
            }
            if (Content[i].Count > 0 && invisibleLegendItems.All(x => x.Name != Content[i][Content[i].Count - 1].State.ToString())) {
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
      string tooltip = string.Format("Task: {0} " + Environment.NewLine + "Task Id: {1}" + Environment.NewLine + "State: {2} " + Environment.NewLine + "Duration: {3} " + Environment.NewLine + "{4} - {5}" + Environment.NewLine, from.TaskName, from.TaskId, from.State, duration, from.DateTime, until);

      if (to != null && to.SlaveId != null)
        tooltip += "Slave: " + to.SlaveId;

      if (!string.IsNullOrEmpty(from.Exception))
        tooltip += Environment.NewLine + from.Exception;
      ganttChart.AddData(name, from.State.ToString(), from.DateTime, until, tooltip, false);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Events
    void chart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
      HitTestResult result = ganttChart.chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        Cursor = Cursors.Hand;
      else
        Cursor = Cursors.Default;
    }

    private void chart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      HitTestResult result = ganttChart.chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        ToggleLegendItemVisibility(result.Object as LegendItem);
      ganttChart.Reset();
      OnContentChanged();
    }

    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      foreach (var item in e.LegendItems)
        foreach (var cell in item.Cells)
          cell.ForeColor = invisibleLegendItems.Any(x => x.Name == item.Name) ? Color.Gray : Color.Black;
    }
    #endregion

    #region Helpers
    private void ToggleLegendItemVisibility(LegendItem legendItem) {
      var item = invisibleLegendItems.FirstOrDefault(x => x.Name == legendItem.Name);
      if (item != null) invisibleLegendItems.Remove(item);
      else invisibleLegendItems.Add(legendItem);
    }
    #endregion
  }
}
