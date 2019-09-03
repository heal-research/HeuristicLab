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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("Scatter Plot")]
  [Content(typeof(ParetoFrontScatterPlot<>))]
  public partial class ParetoFrontScatterPlotView<T> : ItemView where T : class, IItem {

    private readonly ScatterPlot scatterPlot;
    private LinkedList<ScatterPlotDataRow> frontsRow;
    private ScatterPlotDataRow bestKnownFrontRow;

    private int oldObjectives = -1;

    private bool suppressEvents;

    public new ParetoFrontScatterPlot<T> Content {
      get { return (ParetoFrontScatterPlot<T>)base.Content; }
      set { base.Content = value; }
    }

    public ParetoFrontScatterPlotView() {
      InitializeComponent();

      scatterPlot = new ScatterPlot();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        scatterPlotView.Content = null;
        xAxisComboBox.Items.Clear();
        xAxisComboBox.SelectedIndex = -1;
        yAxisComboBox.Items.Clear();
        yAxisComboBox.SelectedIndex = -1;
        return;
      }

      scatterPlotView.Content = scatterPlot;

      if (oldObjectives != Content.Objectives)
        UpdateAxisComboBoxes();

      UpdateChartData();

      oldObjectives = Content.Objectives;
    }

    private void UpdateChartData() {
      if (InvokeRequired) {
        Invoke((Action)UpdateChartData);
        return;
      }

      int xDimIndex = xAxisComboBox.SelectedIndex;
      int yDimIndex = yAxisComboBox.SelectedIndex;

      if (Content.BestKnownFront != null && Content.BestKnownFront.Count > 0) {
        if (bestKnownFrontRow == null) {
          bestKnownFrontRow = new ScatterPlotDataRow("Best Known Pareto Front", string.Empty, Enumerable.Empty<Point2D<double>>()) {
            VisualProperties = {
            PointSize = 4,
            PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Square,
            Color = Color.Gray
          }
          };
          scatterPlot.Rows.Add(bestKnownFrontRow);
        }
        bestKnownFrontRow.Points.Replace(CreatePoints(Content.BestKnownFront, null, xDimIndex, yDimIndex));
      } else if (bestKnownFrontRow != null) {
        scatterPlot.Rows.Remove(bestKnownFrontRow);
        bestKnownFrontRow = null;
      }

      if (Content.Fronts == null || Content.Fronts.Count == 0) {
        if (frontsRow != null) {
          foreach (var row in frontsRow) scatterPlot.Rows.Remove(row);
          frontsRow = null;
        }
      } else {
        if (frontsRow == null) frontsRow = new LinkedList<ScatterPlotDataRow>();
        var row = frontsRow.First;
        var front = 0;
        while (front < Content.Fronts.Count) {
          if (Content.Fronts[front].Length == 0) break;
          if(row == null) {
            row = frontsRow.AddLast(new ScatterPlotDataRow("Front " + front, "Points on Front #" + front, Enumerable.Empty<Point2D<double>>()) {
              VisualProperties = {
                  PointSize = 8,
                  PointStyle = front == 0 ? ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Diamond : ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Circle,
                  Color = front == 0 ? Color.Goldenrod : Color.DodgerBlue
                }
            });
            scatterPlot.Rows.Add(row.Value);
          }
          row.Value.Points.Replace(CreatePoints(Content.Fronts[front], Content.Items[front], xDimIndex, yDimIndex));
          row = row.Next;
          front++;
        }
        while (row != null) {
          scatterPlot.Rows.Remove(row.Value);
          var next = row.Next;
          frontsRow.Remove(row);
          row = next;
        }
      }
    }

    private void UpdateAxisComboBoxes() {
      try {
        suppressEvents = true;

        string prevSelectedX = (string)xAxisComboBox.SelectedItem;
        string prevSelectedY = (string)yAxisComboBox.SelectedItem;

        xAxisComboBox.Items.Clear();
        yAxisComboBox.Items.Clear();

        // Add Objectives first
        for (int i = 0; i < Content.Objectives; i++) {
          xAxisComboBox.Items.Add("Objective " + i);
          yAxisComboBox.Items.Add("Objective " + i);
        }

        // Selection
        int count = xAxisComboBox.Items.Count;
        if (count > 0) {
          if (prevSelectedX != null && xAxisComboBox.Items.Contains(prevSelectedX))
            xAxisComboBox.SelectedItem = prevSelectedX;
          else xAxisComboBox.SelectedIndex = 0;

          if (prevSelectedY != null && yAxisComboBox.Items.Contains(prevSelectedY))
            yAxisComboBox.SelectedItem = prevSelectedY;
          else yAxisComboBox.SelectedIndex = Math.Min(1, count - 1);
        } else {
          xAxisComboBox.SelectedIndex = -1;
          yAxisComboBox.SelectedIndex = -1;
        }

        UpdateAxisDescription();
      } finally {
        suppressEvents = false;
      }
    }

    private void UpdateAxisDescription() {
      scatterPlot.VisualProperties.XAxisTitle = (string)xAxisComboBox.SelectedItem;
      scatterPlot.VisualProperties.YAxisTitle = (string)yAxisComboBox.SelectedItem;
    }

    private static Point2D<double>[] CreatePoints(IReadOnlyList<double[]> front, T[] solutions, int xDimIndex, int yDimIndex) {
      if (front == null || front.Count == 0) return new Point2D<double>[0];
      
      var points = new Point2D<double>[front.Count];
      for (int i = 0; i < front.Count; i++) {
        points[i] = new Point2D<double>(front[i][xDimIndex], front[i][yDimIndex], solutions != null ? solutions[i] : null);
      }
      return points;
    }

    #region Event Handler
    private void axisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (suppressEvents) return;
      UpdateAxisDescription();
      UpdateChartData();
    }
    #endregion
  }
}