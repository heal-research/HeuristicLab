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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("Scatter Plot")]
#pragma warning disable CS0618 // Type or member is obsolete
  [Content(typeof(ParetoFrontScatterPlot))]
#pragma warning restore CS0618 // Type or member is obsolete
  public partial class ParetoFrontScatterPlotView : ItemView {

    private readonly ScatterPlot scatterPlot;
    private readonly ScatterPlotDataRow qualitiesRow;
    private readonly ScatterPlotDataRow paretoFrontRow;

    private int oldObjectives = -1;
    private int oldProblemSize = -1;

    private bool suppressEvents;

#pragma warning disable CS0618 // Type or member is obsolete
    public new ParetoFrontScatterPlot Content {
      get { return (ParetoFrontScatterPlot)base.Content; }
      set { base.Content = value; }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public ParetoFrontScatterPlotView() {
      InitializeComponent();

      scatterPlot = new ScatterPlot();

      qualitiesRow = new ScatterPlotDataRow("Qualities", string.Empty, Enumerable.Empty<Point2D<double>>()) {
        VisualProperties = {
          PointSize = 8 ,
          PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Circle
        }
      };
      scatterPlot.Rows.Add(qualitiesRow);

      paretoFrontRow = new ScatterPlotDataRow("Best Known Pareto Front", string.Empty, Enumerable.Empty<Point2D<double>>()) {
        VisualProperties = {
            PointSize = 4,
            PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Square
          }
      };
      scatterPlot.Rows.Add(paretoFrontRow);
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

      if (oldObjectives != Content.Objectives || oldProblemSize != Content.ProblemSize)
        UpdateAxisComboBoxes();

      UpdateChartData();

      oldObjectives = Content.Objectives;
      oldProblemSize = Content.ProblemSize;
    }

    private void UpdateChartData() {
      if (InvokeRequired) {
        Invoke((Action)UpdateChartData);
        return;
      }

      int xDimGlobal = xAxisComboBox.SelectedIndex;
      int yDimGlobal = yAxisComboBox.SelectedIndex;

      qualitiesRow.Points.Replace(CreatePoints(Content.Qualities, Content.Solutions, xDimGlobal, yDimGlobal));

      paretoFrontRow.Points.Replace(CreatePoints(Content.ParetoFront, null, xDimGlobal, yDimGlobal));
      paretoFrontRow.VisualProperties.IsVisibleInLegend = paretoFrontRow.Points.Count > 0; // hide if empty
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

        // Add Problem Dimension
        for (int i = 0; i < Content.ProblemSize; i++) {
          xAxisComboBox.Items.Add("Problem Dimension " + i);
          yAxisComboBox.Items.Add("Problem Dimension " + i);
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

    private static Point2D<double>[] CreatePoints(double[][] qualities, double[][] solutions, int xDimGlobal, int yDimGlobal) {
      if (qualities == null || qualities.Length == 0) return new Point2D<double>[0];

      int objectives = qualities[0].Length;

      // "Global" dimension index describes the index as if the qualities and solutions would be in a single array
      // If the global dimension index is too long for the qualities, use solutions
      var xDimArray = xDimGlobal < objectives ? qualities : solutions;
      var yDimArray = yDimGlobal < objectives ? qualities : solutions;
      var xDimIndex = xDimGlobal < objectives ? xDimGlobal : xDimGlobal - objectives;
      var yDimIndex = yDimGlobal < objectives ? yDimGlobal : yDimGlobal - objectives;

      if (xDimArray == null || yDimArray == null)
        return new Point2D<double>[0];

      var points = new Point2D<double>[xDimArray.Length];
      for (int i = 0; i < xDimArray.Length; i++) {
        points[i] = new Point2D<double>(xDimArray[i][xDimIndex], yDimArray[i][yDimIndex]);
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