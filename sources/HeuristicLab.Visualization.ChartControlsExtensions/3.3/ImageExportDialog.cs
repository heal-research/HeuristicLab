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
  public enum LengthUnit { Centimeters = 1, Inches = 2 };

  public sealed partial class ImageExportDialog : Form {
    private const float CMPERINCH = 2.54f;
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
      SuppressEvents = true;
      titleComboBox.Text = "12";
      resolutionUnitComboBox.SelectedIndex = 0;
      lengthUnitComboBox.SelectedIndex = 1;
      resolutionComboBox.Text = "300";
      SuppressEvents = false;
      splitContainer.Panel2Collapsed = true;
      Width = 305;
      Height = 550;
    }

    private void UpdateFields() {
      ChartArea area = GetCurrentChartArea();

      try {
        SuppressEvents = true;

        if (workingChart.Titles.Count == 0) titleComboBox.Text = "12";
        else {
          titleTextBox.Text = workingChart.Titles[0].Text;
          titleComboBox.Text = workingChart.Titles[0].Font.SizeInPoints.ToString();
        }

        primaryXTextBox.Text = area.AxisX.Title;
        primaryYTextBox.Text = area.AxisY.Title;
        secondaryXTextBox.Text = area.AxisX2.Title;
        secondaryYTextBox.Text = area.AxisY2.Title;

        axisComboBox.Text = area.AxisX.TitleFont.SizeInPoints.ToString();
        scalesComboBox.Text = area.AxisX.LabelStyle.Font.SizeInPoints.ToString();
      } finally {
        SuppressEvents = false;
      }
    }

    private ChartArea GetCurrentChartArea() {
      return workingChart.ChartAreas[chartAreaComboBox.Text];
    }

    private void UpdatePreview() {
      float dpi = float.Parse(resolutionComboBox.Text);
      if (resolutionUnitComboBox.SelectedIndex == 1) dpi *= CMPERINCH;
      float width = (float)widthNumericUD.Value;
      float height = (float)heightNumericUD.Value;
      if (lengthUnitComboBox.SelectedIndex == 1) {
        width /= CMPERINCH; height /= CMPERINCH;
      }
      width *= dpi; height *= dpi;
      if (previewPictureBox.Image != null) previewPictureBox.Image.Dispose();
      int previewWidth, previewHeight;
      if (width / height >= 1.0) {
        previewWidth = previewPictureBox.Width;
        previewHeight = (int)Math.Round(height / width * previewWidth);
      } else {
        previewHeight = previewPictureBox.Height;
        previewWidth = (int)Math.Round(width / height * previewHeight);
      }
      Bitmap image = new Bitmap(previewWidth, previewHeight);
      image.SetResolution(dpi, dpi);
      using (Graphics graphics = Graphics.FromImage(image)) {
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        float scaleFactor = (float)Math.Min(image.Width / width, image.Height / height);
        previewZoomLabel.Text = (scaleFactor * 100).ToString("0") + "%";
        graphics.ScaleTransform(scaleFactor, scaleFactor);
        workingChart.Printing.PrintPaint(graphics, new Rectangle(0, 0, (int)width, (int)height));
      }
      previewPictureBox.Image = image;
    }

    protected override void OnShown(EventArgs e) {
      var prev = originalChart.Serializer.Content;
      originalChart.Serializer.Content = SerializationContents.Default;
      MemoryStream ms = new MemoryStream();
      originalChart.Serializer.Save(ms);
      originalChart.Serializer.Content = prev;

      ms.Seek(0, SeekOrigin.Begin);
      workingChart = new EnhancedChart();
      workingChart.Serializer.Load(ms);
      ms.Close();

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

    private void titleComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(titleComboBox.Text, out fontSize)) {
          if (workingChart.Titles.Count > 0) {
            workingChart.Titles[0].Font = ChangeFontSizePt(workingChart.Titles[0].Font, fontSize);
            if (togglePreviewCheckBox.Checked) UpdatePreview();
          }
        }
      }
    }

    private void titleComboBox_Validating(object sender, CancelEventArgs e) {
      float number;
      e.Cancel = !float.TryParse(titleComboBox.Text, out number);
    }

    private void axisComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(axisComboBox.Text, out fontSize)) {
          ChartArea area = GetCurrentChartArea();
          foreach (Axis a in area.Axes) {
            a.TitleFont = ChangeFontSizePt(a.TitleFont, fontSize);
          }
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void axisComboBox_Validating(object sender, CancelEventArgs e) {
      float number;
      e.Cancel = !float.TryParse(axisComboBox.Text, out number);
    }

    private void scalesComboBox_TextChanged(object sender, EventArgs e) {
      if (!SuppressEvents) {
        float fontSize;
        if (float.TryParse(scalesComboBox.Text, out fontSize)) {
          ChartArea area = GetCurrentChartArea();
          foreach (Axis a in area.Axes) {
            a.LabelStyle.Font = ChangeFontSizePt(a.LabelStyle.Font, fontSize);
          }
        }
        if (togglePreviewCheckBox.Checked) UpdatePreview();
      }
    }

    private void scalesComboBox_Validating(object sender, CancelEventArgs e) {
      float number;
      e.Cancel = !float.TryParse(scalesComboBox.Text, out number);
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
      float dpi = float.Parse(resolutionComboBox.Text);
      if (resolutionUnitComboBox.SelectedIndex == 1) dpi *= CMPERINCH;
      float width = (float)widthNumericUD.Value;
      float height = (float)heightNumericUD.Value;
      if (lengthUnitComboBox.SelectedIndex == 1) {
        width /= CMPERINCH;
        height /= CMPERINCH;
      }
      Bitmap image = new Bitmap((int)Math.Round(width * dpi), (int)Math.Round(height * dpi));
      image.SetResolution(dpi, dpi);
      using (Graphics graphics = Graphics.FromImage(image)) {
        workingChart.Printing.PrintPaint(graphics, new Rectangle(0, 0, image.Width, image.Height));
      }

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
