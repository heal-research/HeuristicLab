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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Clients.Hive.Views {
  public partial class GanttChart : UserControl {

    private IDictionary<string, Color> categories = new Dictionary<string, Color>();
    private IDictionary<string, int> rowNames = new Dictionary<string, int>();

    public GanttChart() {
      InitializeComponent();
    }

    public void AddCategory(string name, Color color) {
      categories[name] = color;
      chart.Legends[0].CustomItems.Add(color, name);
    }

    public void AddData(string rowName, string category, DateTime start, DateTime end, string tooltip, bool showLabel = true) {
      AddRowName(rowName);
      var point = CreateDataPoint(rowNames[rowName], rowName, start, end, showLabel ? category : string.Empty, categories[category]);
      point.ToolTip = tooltip;
      chart.Series[0].Points.Add(point);
    }

    private void AddRowName(string rowName) {
      if (!rowNames.ContainsKey(rowName)) {
        int nextId = rowNames.Count == 0 ? 1 : rowNames.Values.Max() + 1;
        rowNames.Add(rowName, nextId);
      }
    }

    private static DataPoint CreateDataPoint(double x, string axisLabel, DateTime start, DateTime end, string text, Color color) {
      var point = new DataPoint(x, new double[] { start.ToOADate(), end.ToOADate() });
      point.Color = color;
      point.Label = text;
      point.AxisLabel = axisLabel;
      return point;
    }

    public void Reset() {
      chart.Series[0].Points.Clear();
      categories.Clear();
      chart.Legends[0].CustomItems.Clear();
      rowNames.Clear();
    }
  }
}
