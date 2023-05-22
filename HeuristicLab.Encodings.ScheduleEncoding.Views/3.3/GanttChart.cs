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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Views {
  public partial class GanttChart : UserControl {

    private IDictionary<int, Color> jobColors = new SortedDictionary<int, Color>();
    private IDictionary<string, int> rowNames = new Dictionary<string, int>();


    public GanttChart() {
      InitializeComponent();
      chart.Series[0].YValueType = ChartValueType.Double;
    }

    public void AddJobColor(int jobNr) {
      if (!jobColors.ContainsKey(jobNr)) {
        System.Random r = new System.Random(jobNr + 1);
        jobColors[jobNr] = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
        chart.Legends[0].CustomItems.Clear();
        if (jobColors.Count > 1) {
          int i = 0;
          foreach (Color c in jobColors.Values) {
            chart.Legends[0].CustomItems.Add(c, "Job#" + (i++));
          }
        }
      }
    }

    private void AddRowName(string rowName) {
      if (!rowNames.ContainsKey(rowName)) {
        int nextId = rowNames.Count == 0 ? 1 : rowNames.Values.Max() + 1;
        rowNames.Add(rowName, nextId);
      }
    }

    public void AddData(string rowName, int jobNr, int taskNr, double start, double end, string tooltip, bool showLabel = true) {
      AddRowName(rowName);
      AddJobColor(jobNr);
      JobDataPoint point = new JobDataPoint(rowNames[rowName], new double[] { start, end }, jobNr, taskNr);
      point.Color = jobColors[jobNr];
      point.AxisLabel = rowName;
      point.ToolTip = tooltip;
      chart.Series[0].Points.Add(point);
    }

    public void Reset() {
      chart.Series[0].Points.Clear();
      jobColors.Clear();
      chart.Legends[0].CustomItems.Clear();
      rowNames.Clear();
    }

    void chart_MouseClick(object sender, MouseEventArgs e) {
      Point pos = e.Location;
      HitTestResult[] results = chart.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
      ResetDataColors();

      foreach (HitTestResult result in results) {
        if (result.ChartElementType == ChartElementType.DataPoint && result.Object.GetType() == typeof(JobDataPoint)) {
          int currentJobNr = (result.Object as JobDataPoint).JobNr;
          int currentTaskNr = (result.Object as JobDataPoint).TaskNr;

          HighlightTaskAndJob(currentJobNr, currentTaskNr);
        }
      }
    }

    public void ResetDataColors() {
      foreach (DataPoint dp in chart.Series[0].Points) {
        if (dp.GetType() == typeof(JobDataPoint))
          dp.Color = jobColors[(dp as JobDataPoint).JobNr];
      }
    }

    public void HighlightTaskAndJob(int jobNr, int taskNr) {
      foreach (DataPoint dp in chart.Series[0].Points) {
        if (dp.GetType() == typeof(JobDataPoint) && ((dp as JobDataPoint).JobNr == jobNr) && ((dp as JobDataPoint).TaskNr == taskNr)) {
          Color newColor = Color.FromArgb(255, dp.Color);
          dp.Color = newColor;
        } else if (dp.GetType() == typeof(JobDataPoint) && ((dp as JobDataPoint).JobNr == jobNr)) {
          Color newColor = Color.FromArgb(180, dp.Color);
          dp.Color = newColor;
        } else if (dp.GetType() == typeof(JobDataPoint) && !((dp as JobDataPoint).JobNr == jobNr)) {
          Color newColor = Color.FromArgb(0, dp.Color);
          dp.Color = newColor;
        }
      }
    }

  }
}
