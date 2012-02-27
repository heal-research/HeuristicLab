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
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimizer.Tools {
  [View("MDSView")]
  [Content(typeof(IStringConvertibleMatrix), IsDefaultView = false)]
  public partial class MDSView : StringConvertibleMatrixView {

    public MDSView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.RowsChanged += new EventHandler(Content_Changed);
      Content.ColumnsChanged += new EventHandler(Content_Changed);
    }

    protected override void DeregisterContentEvents() {
      Content.RowsChanged -= new EventHandler(Content_Changed);
      Content.ColumnsChanged -= new EventHandler(Content_Changed);
      base.DeregisterContentEvents();
    }

    private void Content_Changed(object sender, EventArgs e) {
      SetEnabledStateOfControls();
      OnRedraw();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      OnRedraw();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      visualizeCheckBox.Enabled = Content != null && Content.Rows == Content.Columns && Content.Rows > 0;
      mdsPictureBox.Visible = visualizeCheckBox.Enabled && visualizeCheckBox.Checked;
    }

    private void visualizeCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      mdsPictureBox.Visible = visualizeCheckBox.Checked;
      OnRedraw();
    }

    private void OnRedraw() {
      if (InvokeRequired) {
        Invoke((Action)OnRedraw, null);
      } else {
        if (!mdsPictureBox.Visible) return;
        GenerateImage();
      }
    }

    private void GenerateImage() {
      if (Content.Rows == Content.Columns && Content.Rows > 0
       && mdsPictureBox.Width > 0 && mdsPictureBox.Height > 0) {
        Bitmap newBitmap = GenerateDistanceImage();
        if (newBitmap != null) {
          var oldImage = mdsPictureBox.Image;
          mdsPictureBox.Image = newBitmap;
          if (oldImage != null) oldImage.Dispose();
        }
      }
    }

    private Bitmap GenerateDistanceImage() {
      Bitmap newBitmap = new Bitmap(mdsPictureBox.Width, mdsPictureBox.Height);

      DoubleMatrix coordinates;
      double stress = double.NaN;
      DoubleMatrix matrix = new DoubleMatrix(Content.Rows, Content.Columns, Content.ColumnNames, Content.RowNames);
      try {
        for (int i = 0; i < Content.Rows; i++)
          for (int j = 0; j < Content.Columns; j++)
            matrix[i, j] = double.Parse(Content.GetValue(i, j));
        coordinates = MultidimensionalScaling.KruskalShepard(matrix);
        stress = MultidimensionalScaling.CalculateNormalizedStress(matrix, coordinates);
        statisticsTextBox.Text = "Stress: " + stress.ToString("0.00", CultureInfo.CurrentCulture.NumberFormat);
      } catch {
        WriteCenteredTextToBitmap(ref newBitmap, "Matrix is not symmetric");
        statisticsTextBox.Text = "-";
        return newBitmap;
      }
      double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
      double maxDistance = double.MinValue;
      for (int i = 0; i < coordinates.Rows; i++) {
        if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
        if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
        if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
        if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];

        for (int j = i + 1; j < coordinates.Rows; j++) {
          if (matrix[i, j] > maxDistance) maxDistance = matrix[i, j];
          if (matrix[j, i] > maxDistance) maxDistance = matrix[j, i];
        }
      }

      int border = 20;
      double xStep = xMax != xMin ? (mdsPictureBox.Width - 2 * border) / (xMax - xMin) : 1;
      double yStep = yMax != yMin ? (mdsPictureBox.Height - 2 * border) / (yMax - yMin) : 1;

      Point[] points = new Point[coordinates.Rows];
      for (int i = 0; i < coordinates.Rows; i++)
        points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                              newBitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

      Random rand = new Random();
      using (Graphics graphics = Graphics.FromImage(newBitmap)) {
        graphics.Clear(Color.White);
        graphics.DrawString("Showing entities spaced out according to their distances.", Font, Brushes.Black, 5, 2);

        for (int i = 0; i < coordinates.Rows - 1; i++) {
          for (int j = i + 1; j < coordinates.Rows; j++) {
            Point start = points[i], end = points[j];
            string caption = String.Empty;
            double d = Math.Max(matrix[i, j], matrix[j, i]);
            float width = (float)Math.Ceiling(5.0 * d / maxDistance);
            if (d > 0) {
              graphics.DrawLine(new Pen(Color.IndianRed, width), start, end);
              if (matrix[i, j] != matrix[j, i])
                caption = matrix[i, j].ToString(CultureInfo.InvariantCulture.NumberFormat)
                  + " / " + matrix[j, i].ToString(CultureInfo.InvariantCulture.NumberFormat);
              else
                caption = matrix[i, j].ToString(CultureInfo.InvariantCulture.NumberFormat);
            }
            if (!String.IsNullOrEmpty(caption)) {
              double r = rand.NextDouble();
              while (r < 0.2 || r > 0.8) r = rand.NextDouble();
              float x = (float)(start.X + (end.X - start.X) * r + 5);
              float y = (float)(start.Y + (end.Y - start.Y) * r + 5);
              graphics.DrawString(caption, Font, Brushes.Black, x, y);
            }
          }
        }

        for (int i = 0; i < points.Length; i++) {
          Point p = new Point(points[i].X - 3, points[i].Y - 3);
          graphics.FillRectangle(Brushes.Black, p.X, p.Y, 8, 8);
          graphics.DrawString(i.ToString(), Font, Brushes.Black, p.X, p.Y + 10);
        }
      }
      return newBitmap;
    }

    private void WriteCenteredTextToBitmap(ref Bitmap bitmap, string text) {
      if (bitmap == null) return;
      using (Graphics g = Graphics.FromImage(bitmap)) {
        g.Clear(Color.White);

        Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
        SizeF strSize = g.MeasureString(text, font);
        if (strSize.Width + 50 > mdsPictureBox.Width) {
          var m = Regex.Match(text, @"\b\w+[.,]*\b*");
          var builder = new StringBuilder();
          while (m.Success) {
            builder.Append(m.Value + " ");
            Match next = m.NextMatch();
            if (g.MeasureString(builder.ToString() + " " + next.Value, font).Width + 50 > mdsPictureBox.Width)
              builder.AppendLine();
            m = next;
          }
          builder.Remove(builder.Length - 1, 1);
          text = builder.ToString();
          strSize = g.MeasureString(text, font);
        }
        g.DrawString(text, font, Brushes.Black, (float)(mdsPictureBox.Width - strSize.Width) / 2.0f, (float)(mdsPictureBox.Height - strSize.Height) / 2.0f);
      }
    }
  }
}
