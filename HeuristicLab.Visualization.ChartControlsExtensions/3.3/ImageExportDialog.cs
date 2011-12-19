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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public sealed partial class ImageExportDialog : Form {
    private const float CMPERINCH = 2.54f;
    private static readonly string DPI = "dpi", DPCM = "dpcm", INCH = "inch", CM = "cm";
    private Chart originalChart, workingChart;
    private bool SuppressEvents { get; set; }

    /// <summary>
    /// Initializes a new ImageExportDialog.
    /// </summary>
    /// <remarks>
    /// Throws an ArgumentNullException if <paramref name="chart"/> is null.
    /// </remarks>
    /// <param name="chart">The chart for which the export should be generated.</param>
    public ImageExportDialog(Chart chart) {
      if (chart == null) throw new ArgumentNullException("chart");
      this.originalChart = chart;
      InitializeComponent();
      #region Custom Initialization
      SuppressEvents = true;
      try {
        resolutionUnitComboBox.Items.Add(DPI);
        resolutionUnitComboBox.Items.Add(DPCM);
        lengthUnitComboBox.Items.Add(INCH);
        lengthUnitComboBox.Items.Add(CM);
        resolutionUnitComboBox.SelectedIndex = 0;
        if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
          lengthUnitComboBox.SelectedIndex = 1;
        else lengthUnitComboBox.SelectedIndex = 0;

        titleFontSizeComboBox.Text = "12";
        axisFontSizeComboBox.Text = "8";
        scalesFontSizeComboBox.Text = "6";
        legendFontSizeComboBox.Text = "6";
        resolutionComboBox.Text = "300";
        SuppressEvents = false;
        splitContainer.Panel2Collapsed = true;
        Width = 305;
        Height = 550;
      } finally { SuppressEvents = false; }
      #endregion
    }

    private void UpdateFields() {
      ChartArea area = GetCurrentChartArea();

      try {
        SuppressEvents = true;

        if (workingChart.Titles.Count == 0) titleFontSizeComboBox.Text = "12";
        else {
          titleTextBox.Text = workingChart.Titles[0].Text;
          titleFontSizeComboBox.Text = workingChart.Titles[0].Font.SizeInPoints.ToString();
        }

        primaryXTextBox.Text = area.AxisX.Title;
        primaryYTextBox.Text = area.AxisY.Title;
        secondaryXTextBox.Text = area.AxisX2.Title;
        secondaryYTextBox.Text = area.AxisY2.Title;

        axisFontSizeComboBox.Text = area.AxisX.TitleFont.SizeInPoints.ToString();
        scalesFontSizeComboBox.Text = area.AxisX.LabelStyle.Font.SizeInPoints.ToString();
        if (workingChart.Legends.Count == 0) legendFontSizeComboBox.Text = "8";
        else legendFontSizeComboBox.Text = workingChart.Legends[0].Font.SizeInPoints.ToString();
      } finally {
        SuppressEvents = false;
      }
    }

    private ChartArea GetCurrentChartArea() {
      return workingChart.ChartAreas[chartAreaComboBox.Text];
    }

    private void UpdatePreview() {
      float dpi;
      float width;
      float height;
      GetImageParameters(out dpi, out width, out height);

      if (previewPictureBox.Image != null) {
        previewPictureBox.Image.Dispose();
        previewPictureBox.Image = null;
      }

      int previewWidth, previewHeight;
      if (width / height >= 1.0) {
        previewWidth = previewPictureBox.Width;
        previewHeight = (int)Math.Round(height / width * previewWidth);
      } else {
        previewHeight = previewPictureBox.Height;
        previewWidth = (int)Math.Round(width / height * previewHeight);
      }

      float scaleFactor = (float)Math.Min(previewWidth / width, previewHeight / height);
      if (scaleFactor >= 1) {
        previewZoomLabel.Text = "100%";
        previewWidth = (int)Math.Round(width);
        previewHeight = (int)Math.Round(height);
      } else previewZoomLabel.Text = (scaleFactor * 100).ToString("0") + "%";

      Bitmap image = new Bitmap(previewWidth, previewHeight);
      image.SetResolution(dpi, dpi);
      using (Graphics graphics = Graphics.FromImage(image)) {
        if (scaleFactor < 1) graphics.ScaleTransform(scaleFactor, scaleFactor);
        workingChart.Printing.PrintPaint(graphics, new Rectangle(0, 0, (int)Math.Round(width), (int)Math.Round(height)));
      }
      previewPictureBox.Image = image;
    }

    private void GetImageParameters(out float dpi, out float width, out float height) {
      dpi = float.Parse(resolutionComboBox.Text);
      if (resolutionUnitComboBox.Text == DPCM) dpi *= CMPERINCH;
      width = (float)widthNumericUD.Value;
      height = (float)heightNumericUD.Value;
      if (lengthUnitComboBox.Text == CM) {
        width /= CMPERINCH; height /= CMPERINCH;
      }
      width *= dpi; height *= dpi;
    }

    protected override void OnShown(EventArgs e) {
      #region Create copy of chart
      var prevContent = originalChart.Serializer.Content;
      var prevFormat = originalChart.Serializer.Format;
      originalChart.Serializer.Content = SerializationContents.Default;
      originalChart.Serializer.Format = SerializationFormat.Binary;
      MemoryStream ms = new MemoryStream();
      originalChart.Serializer.Save(ms);

      ms.Seek(0, SeekOrigin.Begin);
      workingChart = new EnhancedChart();
      workingChart.Serializer.Format = originalChart.Serializer.Format;
      workingChart.Serializer.Load(ms);
      ms.Close();

      originalChart.Serializer.Content = prevContent;
      originalChart.Serializer.Format = prevFormat;
      #endregion

      chartAreaComboBox.Items.Clear();
      foreach (ChartArea area in originalChart.ChartAreas) {
        chartAreaComboBox.Items.Add(area.Name);
      }
      chartAreaComboBox.SelectedIndex = 0;
      base.OnShown(e);

      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void togglePreviewCheckBox_CheckedChanged(object sender, EventArgs e) {
      splitContainer.Panel2Collapsed = !togglePreviewCheckBox.Checked;
      togglePreviewCheckBox.Text = togglePreviewCheckBox.Checked ? "<" : ">";
      if (splitContainer.Panel2Collapsed)
        Width = cancelButton.Right + cancelButton.Margin.Right + Margin.Right + 10;
      else
        Width = splitContainer.Right + splitContainer.Margin.Right + Margin.Right;
      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void chartAreaComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (chartAreaComboBox.SelectedIndex >= 0)
        UpdateFields();
    }

    private void titleTextBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        if (workingChart.Titles.Count > 0) {
          workingChart.Titles[0].Text = titleTextBox.Text;
        } else {
          Title t = new Title(titleTextBox.Text);
          t.Font = ChangeFontSizePt(t.Font, float.Parse(titleFontSizeComboBox.Text));
          workingChart.Titles.Add(t);
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void primaryXTextBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        ChartArea area = GetCurrentChartArea();
        area.AxisX.Title = primaryXTextBox.Text;
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void primaryYTextBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        ChartArea area = GetCurrentChartArea();
        area.AxisY.Title = primaryYTextBox.Text;
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void secondaryXTextBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        ChartArea area = GetCurrentChartArea();
        area.AxisX2.Title = secondaryXTextBox.Text;
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void secondaryYTextBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        ChartArea area = GetCurrentChartArea();
        area.AxisY2.Title = secondaryYTextBox.Text;
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void widthNumericUD_ValueChanged(object sender, EventArgs e) {
      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void heightNumericUD_ValueChanged(object sender, EventArgs e) {
      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void titleFontSizeComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(titleFontSizeComboBox.Text, out fontSize)) {
          if (workingChart.Titles.Count > 0) {
            workingChart.Titles[0].Font = ChangeFontSizePt(workingChart.Titles[0].Font, fontSize);
            if (togglePreviewCheckBox.Checked) UpdatePreview();
          }
        }
      }
    }

    private void axisFontSizeComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(axisFontSizeComboBox.Text, out fontSize)) {
          ChartArea area = GetCurrentChartArea();
          foreach (Axis a in area.Axes) {
            a.TitleFont = ChangeFontSizePt(a.TitleFont, fontSize);
          }
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void scalesFontSizeComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(scalesFontSizeComboBox.Text, out fontSize)) {
          ChartArea area = GetCurrentChartArea();
          foreach (Axis a in area.Axes) {
            a.LabelStyle.Font = ChangeFontSizePt(a.LabelStyle.Font, fontSize);
          }
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void legendFontSizeComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(legendFontSizeComboBox.Text, out fontSize)) {
          foreach (Legend l in workingChart.Legends) {
            l.Font = ChangeFontSizePt(l.Font, fontSize);
          }
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void numericComboBox_Validating(object sender, CancelEventArgs e) {
      if (!(sender is ComboBox)) return;
      float number;
      e.Cancel = !float.TryParse((sender as ComboBox).Text, out number);
    }

    private void resolutionComboBox_TextChanged(object sender, EventArgs e) {
      float resolution;
      if (float.TryParse(resolutionComboBox.Text, out resolution)) {
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void resolutionComboBox_Validating(object sender, CancelEventArgs e) {
      float resolution;
      e.Cancel = !float.TryParse(resolutionComboBox.Text, out resolution);
    }

    private void resolutionUnitComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void lengthUnitComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (togglePreviewCheckBox.Checked) UpdatePreview();
    }

    private void okButton_Click(object sender, EventArgs e) {
      float dpi;
      float width;
      float height;
      GetImageParameters(out dpi, out width, out height);

      Bitmap image = new Bitmap((int)Math.Round(width), (int)Math.Round(height));
      image.SetResolution(dpi, dpi);
      using (Graphics graphics = Graphics.FromImage(image)) {
        workingChart.Printing.PrintPaint(graphics, new Rectangle(0, 0, image.Width, image.Height));
      }

      if (titleTextBox.Text.Trim() != String.Empty) saveFileDialog.FileName = titleTextBox.Text.Trim();
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        ImageFormat format = ImageFormat.Bmp;
        string filename = saveFileDialog.FileName.ToLower();
        if (filename.EndsWith("jpg")) {
          format = ImageFormat.Jpeg;
        } else if (filename.EndsWith("emf")) {
          format = ImageFormat.Emf;
        } else if (filename.EndsWith("gif")) {
          format = ImageFormat.Gif;
        } else if (filename.EndsWith("png")) {
          format = ImageFormat.Png;
        } else if (filename.EndsWith("tif")) {
          format = ImageFormat.Tiff;
        }
        image.Save(saveFileDialog.FileName, format);
      }

      image.Dispose();

      Cleanup();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Cleanup();
    }

    private void Cleanup() {
      if (previewPictureBox.Image != null) previewPictureBox.Image.Dispose();
      previewPictureBox.Image = null;
      workingChart = null;
    }

    private static Font ChangeFontSizePt(Font font, float fontSize) {
      if (font != null) {
        float currentSize = font.Size;
        if (currentSize != fontSize) {
          font = new Font(font.Name, fontSize, font.Style, GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
        }
      }
      return font;
    }

  }
}
