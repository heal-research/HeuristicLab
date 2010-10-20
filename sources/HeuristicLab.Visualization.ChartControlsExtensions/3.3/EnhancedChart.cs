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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public partial class EnhancedChart : Chart {
    public EnhancedChart()
      : base() {
      InitializeComponent();
      EnableDoubleClickResetsZoom = true;
      EnableMiddleClickPanning = true;
    }

    [DefaultValue(true)]
    public bool EnableDoubleClickResetsZoom { get; set; }

    [DefaultValue(true)]
    public bool EnableMiddleClickPanning { get; set; }

    public void InitializeChartAreas() {
      ChartAreas.Clear();
      ChartAreas.Add(CreateDefaultChartArea("ChartArea"));
    }

    public static ChartArea CreateDefaultChartArea(string name) {
      ChartArea chartArea = new ChartArea(name);
      chartArea.AxisX.MajorGrid.LineColor = SystemColors.GradientInactiveCaption;
      chartArea.AxisY.MajorGrid.LineColor = SystemColors.GradientInactiveCaption;
      chartArea.AxisX.MajorTickMark.TickMarkStyle = TickMarkStyle.AcrossAxis;
      chartArea.AxisY.MajorTickMark.TickMarkStyle = TickMarkStyle.AcrossAxis;
      chartArea.AxisX.ScrollBar.BackColor = Color.Transparent;
      chartArea.AxisY.ScrollBar.BackColor = Color.Transparent;
      chartArea.AxisX.ScrollBar.LineColor = Color.Transparent;
      chartArea.AxisY.ScrollBar.LineColor = Color.Transparent;
      chartArea.AxisX.ScrollBar.ButtonColor = SystemColors.GradientInactiveCaption;
      chartArea.AxisY.ScrollBar.ButtonColor = SystemColors.GradientInactiveCaption;
      chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
      chartArea.AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
      chartArea.AxisX.ScrollBar.Size = 12;
      chartArea.AxisY.ScrollBar.Size = 12;
      chartArea.CursorX.Interval = 0;
      chartArea.CursorY.Interval = 0;
      chartArea.CursorX.IsUserSelectionEnabled = true;
      chartArea.CursorY.IsUserSelectionEnabled = true;
      chartArea.CursorX.SelectionColor = SystemColors.GradientActiveCaption;
      chartArea.CursorY.SelectionColor = SystemColors.GradientActiveCaption;
      return chartArea;
    }

    #region Mouse event ehancements

    protected override void OnMouseDoubleClick(MouseEventArgs e) {
      if (EnableDoubleClickResetsZoom) {
        HitTestResult result = HitTest(e.X, e.Y);
        if (result.ChartArea != null && result.ChartElementType == ChartElementType.PlottingArea) {
          foreach (var axis in result.ChartArea.Axes)
            axis.ScaleView.ZoomReset();
        }
      }
      base.OnMouseDoubleClick(e);
    }

    #region panning

    private class PanningSupport {
      public ChartArea ChartArea { get; private set; }

      private Point PixelStartPosition;
      private PointF ChartStartPosition;
      private SizeF Pixel2ChartScale;

      public PanningSupport(Point pixelStartPos, ChartArea chartArea, Size size) {
        PixelStartPosition = pixelStartPos;
        ChartArea = chartArea;
        ChartStartPosition = new PointF(
          (float)chartArea.AxisX.ScaleView.Position,
          (float)chartArea.AxisY.ScaleView.Position);
        Pixel2ChartScale = new SizeF(
          (float)chartArea.AxisX.ScaleView.Size /
            (size.Width * chartArea.Position.Width * chartArea.InnerPlotPosition.Width / 100 / 100),
          (float)chartArea.AxisY.ScaleView.Size /
            (size.Height * chartArea.Position.Height * chartArea.InnerPlotPosition.Height / 100 / 100));
      }

      public double ChartX(double pixelX) {
        return ChartStartPosition.X - (pixelX - PixelStartPosition.X) * Pixel2ChartScale.Width;
      }

      public double ChartY(double pixelY) {
        return ChartStartPosition.Y + (pixelY - PixelStartPosition.Y) * Pixel2ChartScale.Height;
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
        panning.ChartArea.AxisX.ScaleView.Scroll(panning.ChartX(e.Location.X));
        panning.ChartArea.AxisY.ScaleView.Scroll(panning.ChartY(e.Location.Y));
      }
      base.OnMouseMove(e);
    }

    #endregion

    #endregion

    private void saveImageToolStripMenuItem_Click(object sender, System.EventArgs e) {
      SaveFileDialog saveFileDialog = new SaveFileDialog();

      // Sets the current file name filter string, which determines 
      // the choices that appear in the "Save as file type" or 
      // "Files of type" box in the dialog box.
      saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";
      saveFileDialog.FilterIndex = 2;
      saveFileDialog.RestoreDirectory = true;

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

    private void copyImageToClipboardBitmapToolStripMenuItem_Click(object sender, System.EventArgs e) {
      System.IO.MemoryStream stream = new System.IO.MemoryStream();
      SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);
      Bitmap bmp = new Bitmap(stream);
      Clipboard.SetDataObject(bmp);
    }
  }
}
