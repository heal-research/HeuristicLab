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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("HeatMap View")]
  [Content(typeof(HeatMap), true)]
  public partial class HeatMapView : ItemView {
    protected static Color[] colors = new Color[256];
    public new HeatMap Content {
      get { return (HeatMap)base.Content; }
      set { base.Content = value; }
    }

    public HeatMapView() {
      InitializeComponent();
      chart.CustomizeAllChartAreas();
    }

    protected override void DeregisterContentEvents() {
      Content.TitleChanged -= new EventHandler(Content_TitleChanged);
      Content.MinimumChanged -= new EventHandler(Content_MinimumChanged);
      Content.MaximumChanged -= new EventHandler(Content_MaximumChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.TitleChanged += new EventHandler(Content_TitleChanged);
      Content.MinimumChanged += new EventHandler(Content_MinimumChanged);
      Content.MaximumChanged += new EventHandler(Content_MaximumChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        chart.Series.Clear();
        chart.Titles[0].Text = "Heat Map";
        minimumLabel.Text = "0.0";
        maximumLabel.Text = "1.0";
      } else {
        chart.Titles[0].Text = Content.Title;
        minimumLabel.Text = Content.Minimum.ToString();
        maximumLabel.Text = Content.Maximum.ToString();
        UpdatePoints();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chart.Enabled = Content != null;
      grayscaleCheckBox.Enabled = Content != null;
    }

    protected virtual void UpdatePoints() {
      chart.Series.Clear();
      Series series = new Series();
      series.ChartType = SeriesChartType.Point;
      series.XValueType = ChartValueType.Int32;
      series.YValueType = ChartValueType.Int32;
      series.YAxisType = AxisType.Primary;
      for (int i = 1; i < Content.Rows + 1; i++)
        for (int j = 1; j < Content.Columns + 1; j++)
          series.Points.Add(CreateDataPoint(j, i, Content[i - 1, j - 1]));
      chart.ChartAreas[0].AxisX.Minimum = 0;
      chart.ChartAreas[0].AxisX.Maximum = Content.Columns + 1;
      chart.ChartAreas[0].AxisY.Minimum = 0;
      chart.ChartAreas[0].AxisY.Maximum = Content.Rows + 1;
      chart.Series.Add(series);
    }

    protected virtual DataPoint CreateDataPoint(int index1, int index2, double value) {
      DataPoint p = new DataPoint(index1, index2);
      p.Color = GetDataPointColor(value, Content.Minimum, Content.Maximum, grayscaleCheckBox.Checked);
      p.MarkerStyle = MarkerStyle.Square;
      return p;
    }

    protected virtual Color GetDataPointColor(double value, double min, double max, bool grayscale) {
      IList<Color> colors = grayscale ? ColorGradient.GrayscaledColors : ColorGradient.Colors;
      int index = (int)((colors.Count - 1) * (value - min) / (max - min));
      if (index >= colors.Count) index = colors.Count - 1;
      if (index < 0) index = 0;
      return colors[index];
    }

    #region Content Events
    protected virtual void Content_TitleChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_TitleChanged), sender, e);
      else {
        chart.Titles[0].Text = Content.Title;
      }
    }
    protected virtual void Content_MinimumChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_MinimumChanged), sender, e);
      else {
        minimumLabel.Text = Content.Minimum.ToString();
        UpdatePoints();
      }
    }
    protected virtual void Content_MaximumChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_MaximumChanged), sender, e);
      else {
        maximumLabel.Text = Content.Maximum.ToString();
        UpdatePoints();
      }
    }
    #endregion

    #region Control Events
    protected virtual void grayscaledImagesCheckBox_CheckedChanged(object sender, EventArgs e) {
      grayscalesPictureBox.Visible = grayscaleCheckBox.Checked;
      colorsPictureBox.Visible = !grayscaleCheckBox.Checked;
      UpdatePoints();
    }
    #endregion
  }
}
