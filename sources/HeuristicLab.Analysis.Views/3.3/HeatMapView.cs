#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("HeatMap View")]
  [Content(typeof(HeatMap), true)]
  public partial class HeatMapView : ItemView {

    private static Color[] Colors;
    private static int ColorsCount = 500;
    private static Color[] GrayscaleColors = new Color[256];

    #region InitializeColors
    static HeatMapView() {
      int stepWidth = (255 * 6) / ColorsCount;
      Color[] colors = new Color[ColorsCount];
      int currentValue;
      int currentClass;
      for (int i = 0; i < ColorsCount; i++) {
        currentValue = (i * stepWidth) % 255;
        currentClass = (i * stepWidth) / 255;
        switch (currentClass) {
          case 0: { colors[i] = Color.FromArgb(255, currentValue, 0); break; }
          case 1: { colors[i] = Color.FromArgb(255 - currentValue, 255, 0); break; }
          case 2: { colors[i] = Color.FromArgb(0, 255, currentValue); break; }
          case 3: { colors[i] = Color.FromArgb(0, 255 - currentValue, 255); break; }
          case 4: { colors[i] = Color.FromArgb(currentValue, 0, 255); break; }
          case 5: { colors[i] = Color.FromArgb(255, 0, 255 - currentValue); break; }
        }
      }
      int n = (int)(ColorsCount * 0.7);
      Colors = new Color[n];
      for (int i = 0; i < n; i++)
        Colors[i] = colors[i];
      for (int i = 0; i < 256; i++)
        GrayscaleColors[i] = Color.FromArgb(i, i, i);
    }
    #endregion

    public new HeatMap Content {
      get { return (HeatMap)base.Content; }
      set { base.Content = value; }
    }

    public HeatMapView() {
      InitializeComponent();
      chart.CustomizeAllChartAreas();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        chart.Series.Clear();
      } else {
        UpdateChart();
      }
    }

    private void UpdateChart() {
      chart.Series.Clear();
      Series series = new Series();
      series.ChartType = SeriesChartType.Point;
      series.XValueType = ChartValueType.Int32;
      series.YValueType = ChartValueType.Int32;
      series.YAxisType = AxisType.Primary;
      for (int i = 0; i < Content.Rows; i++)
        for (int j = 0; j < Content.Columns; j++)
          series.Points.Add(CreateDataPoint(i, j, Content[i, j]));
      chart.ChartAreas[0].AxisY.Minimum = 0;
      chart.ChartAreas[0].AxisY.Maximum = Content.Rows;
      chart.Series.Add(series);
      chart.Legends.Clear();
    }

    private DataPoint CreateDataPoint(int index1, int index2, double value) {
      bool grayScaleModus = grayscaledImagesCheckBox.Checked;
      int n = grayScaleModus ? GrayscaleColors.Length : Colors.Length;
      int colorIndex = (int)((n - 1) * value);
      if (colorIndex >= n) colorIndex = n - 1;
      if (colorIndex < 0) colorIndex = 0;
      // invert so that red is 1, blue 0 / black is 1, white 0
      colorIndex = n - colorIndex - 1;
      Color color = grayScaleModus ? GrayscaleColors[colorIndex] : Colors[colorIndex];

      DataPoint p = new DataPoint(index1, index2);
      p.Color = color;
      p.MarkerStyle = MarkerStyle.Square;
      p.ToolTip = string.Format("Solution {0} vs. solution {1}: {2}",
                                index1, index2, value);
      return p;
    }

    #region Chart Events

    private void grayscaledImagesCheckBox_CheckedChanged(object sender, EventArgs e) {
      GrayscalesPictureBox.Visible = grayscaledImagesCheckBox.Checked;
      ColorsPictureBox.Visible = !grayscaledImagesCheckBox.Checked;
      UpdateChart();
    }

    #endregion

  }

}
