#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public partial class EnhancedChart : Chart {
    private ImageExportDialog exportDialog;

    public EnhancedChart()
      : base() {
      InitializeComponent();
      exportDialog = new ImageExportDialog(this);
      EnableDoubleClickResetsZoom = true;
      EnableMiddleClickPanning = true;
      CustomizeAllChartAreas();
    }

    [DefaultValue(true)]
    public bool EnableDoubleClickResetsZoom { get; set; }
    [DefaultValue(true)]
    public bool EnableMiddleClickPanning { get; set; }

    public static void CustomizeChartArea(ChartArea chartArea) {
      foreach (Axis axis in chartArea.Axes) {
        axis.MajorGrid.LineColor = SystemColors.GradientInactiveCaption;
        axis.MajorTickMark.TickMarkStyle = TickMarkStyle.AcrossAxis;
        axis.ScrollBar.BackColor = Color.Transparent;
        axis.ScrollBar.LineColor = Color.Gray;
        axis.ScrollBar.ButtonColor = SystemColors.GradientInactiveCaption;
        axis.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
        axis.ScrollBar.Size = 12;
        axis.TitleFont = new Font(axis.TitleFont.FontFamily, 10);
      }
      chartArea.CursorX.IsUserSelectionEnabled = true;
      chartArea.CursorY.IsUserSelectionEnabled = true;
      chartArea.CursorX.IsUserEnabled = false;
      chartArea.CursorY.IsUserEnabled = false;
      chartArea.CursorX.SelectionColor = Color.Gray;
      chartArea.CursorY.SelectionColor = Color.Gray;
    }

    public void CustomizeAllChartAreas() {
      foreach (ChartArea chartArea in ChartAreas) {
        CustomizeChartArea(chartArea);
      }
    }

    #region Mouse Event Ehancements
    protected override void OnMouseDoubleClick(MouseEventArgs e) {
      if (EnableDoubleClickResetsZoom) {
        HitTestResult result = HitTest(e.X, e.Y);
        if (result.ChartArea != null && (result.ChartElementType == ChartElementType.PlottingArea || result.ChartElementType == ChartElementType.Gridlines)) {
          foreach (var axis in result.ChartArea.Axes)
            axis.ScaleView.ZoomReset(int.MaxValue);
        }
      }
      base.OnMouseDoubleClick(e);
    }

    #region Panning
    private class PanningSupport {
      public ChartArea ChartArea { get; private set; }

      private Point PixelStartPosition;
      private PointF ChartStartPosition;

      public PanningSupport(Point pixelStartPos, ChartArea chartArea, Size size) {
        PixelStartPosition = pixelStartPos;
        ChartArea = chartArea;
        ChartStartPosition = new PointF(
          (float)chartArea.AxisX.ScaleView.Position,
          (float)chartArea.AxisY.ScaleView.Position);
      }

      public double ChartX(double pixelX, int width) {
        return ChartStartPosition.X - (pixelX - PixelStartPosition.X) *
          (ChartArea.AxisX.ScaleView.ViewMaximum - ChartArea.AxisX.ScaleView.ViewMinimum) /
            (width * ChartArea.Position.Width * ChartArea.InnerPlotPosition.Width / 100 / 100);
      }
      public double ChartY(double pixelY, int height) {
        return ChartStartPosition.Y + (pixelY - PixelStartPosition.Y) *
          (ChartArea.AxisY.ScaleView.ViewMaximum - ChartArea.AxisY.ScaleView.ViewMinimum) /
            (height * ChartArea.Position.Height * ChartArea.InnerPlotPosition.Height / 100 / 100);
      }
    }

    private PanningSupport panning = null;

    protected override void OnMouseDown(MouseEventArgs e) {
      if (EnableMiddleClickPanning && e.Button == MouseButtons.Middle) {
        HitTestResult result = HitTest(e.X, e.Y);
        if (result.ChartArea != null)
          panning = new PanningSupport(e.Location, result.ChartArea, Size);
      }
      base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e) {
      if (e.Button == MouseButtons.Middle && panning != null)
        panning = null;
      base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e) {
      if (panning != null) {
        double x = panning.ChartX(e.Location.X, Width);
        double y = panning.ChartY(e.Location.Y, Height);
        if (panning.ChartArea.CursorX.Interval > 0) {
          x = Math.Round(x / panning.ChartArea.CursorX.Interval) * panning.ChartArea.CursorX.Interval;
          y = Math.Round(y / panning.ChartArea.CursorY.Interval) * panning.ChartArea.CursorY.Interval;
        }
        panning.ChartArea.AxisX.ScaleView.Scroll(x);
        panning.ChartArea.AxisY.ScaleView.Scroll(y);
      }
      base.OnMouseMove(e);
    }
    #endregion
    #endregion

    private void exportChartToolStripMenuItem_Click(object sender, EventArgs e) {
      // Set image file format
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        ChartImageFormat format = ChartImageFormat.Bmp;
        string filename = saveFileDialog.FileName.ToLower();
        if (filename.EndsWith("bmp")) {
          format = ChartImageFormat.Bmp;
        } else if (filename.EndsWith("jpg")) {
          format = ChartImageFormat.Jpeg;
        } else if (filename.EndsWith("emf")) {
          format = ChartImageFormat.EmfDual;
        } else if (filename.EndsWith("gif")) {
          format = ChartImageFormat.Gif;
        } else if (filename.EndsWith("png")) {
          format = ChartImageFormat.Png;
        } else if (filename.EndsWith("tif")) {
          format = ChartImageFormat.Tiff;
        }

        // Save image
        SaveImage(saveFileDialog.FileName, format);
      }
    }

    private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
      exportDialog.ShowDialog();
    }

    private void copyImageToClipboardBitmapToolStripMenuItem_Click(object sender, EventArgs e) {
      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);
      Bitmap bmp = new Bitmap(stream);
      Clipboard.SetDataObject(bmp);
    }
  }
}
