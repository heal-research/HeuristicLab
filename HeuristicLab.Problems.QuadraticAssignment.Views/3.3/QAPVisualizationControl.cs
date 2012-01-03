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
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  public partial class QAPVisualizationControl : UserControl {
    private Bitmap bitmap;
    private Bitmap defaultBitmap;
    private bool showingMessage;

    #region Properties
    private DoubleMatrix distances;
    public DoubleMatrix Distances {
      get { return distances; }
      set {
        DeregisterDistancesEvents();
        distances = value;
        RegisterDistancesEvents();
        OnRedraw();
      }
    }

    private DoubleMatrix weights;
    public DoubleMatrix Weights {
      get { return weights; }
      set {
        DeregisterWeightsEvents();
        weights = value;
        RegisterWeightsEvents();
        OnRedraw();
      }
    }

    private Permutation assignment;
    public Permutation Assignment {
      get { return assignment; }
      set {
        DeregisterAssignmentEvents();
        assignment = value;
        RegisterAssignmentEvents();
        OnRedraw();
      }
    }
    #endregion

    #region Event Handling
    private void DeregisterDistancesEvents() {
      if (Distances != null) {
        Distances.Reset -= new EventHandler(RedrawNecessary);
        Distances.RowsChanged -= new EventHandler(RedrawNecessary);
        Distances.ColumnsChanged -= new EventHandler(RedrawNecessary);
        Distances.ItemChanged -= new EventHandler<EventArgs<int, int>>(RedrawNecessary);
      }
    }

    private void RegisterDistancesEvents() {
      if (Distances != null) {
        Distances.Reset += new EventHandler(RedrawNecessary);
        Distances.RowsChanged += new EventHandler(RedrawNecessary);
        Distances.ColumnsChanged += new EventHandler(RedrawNecessary);
        Distances.ItemChanged += new EventHandler<EventArgs<int, int>>(RedrawNecessary);
      }
    }

    private void DeregisterWeightsEvents() {
      if (Weights != null) {
        Weights.Reset -= new EventHandler(RedrawNecessary);
        Weights.RowsChanged -= new EventHandler(RedrawNecessary);
        Weights.ColumnsChanged -= new EventHandler(RedrawNecessary);
        Weights.ItemChanged -= new EventHandler<EventArgs<int, int>>(RedrawNecessary);
      }
    }

    private void RegisterWeightsEvents() {
      if (Weights != null) {
        Weights.Reset += new EventHandler(RedrawNecessary);
        Weights.RowsChanged += new EventHandler(RedrawNecessary);
        Weights.ColumnsChanged += new EventHandler(RedrawNecessary);
        Weights.ItemChanged += new EventHandler<EventArgs<int, int>>(RedrawNecessary);
      }
    }

    private void DeregisterAssignmentEvents() {
      if (Assignment != null) {
        Assignment.Reset -= new EventHandler(RedrawNecessary);
        Assignment.ItemChanged -= new EventHandler<EventArgs<int>>(RedrawNecessary);
      }
    }

    private void RegisterAssignmentEvents() {
      if (Assignment != null) {
        Assignment.Reset += new EventHandler(RedrawNecessary);
        Assignment.ItemChanged += new EventHandler<EventArgs<int>>(RedrawNecessary);
      }
    }

    private void redrawButton_Click(object sender, EventArgs e) {
      OnRedraw();
    }

    private void radio_CheckedChanged(object sender, EventArgs e) {
      RadioButton rb = (sender as RadioButton);
      if (rb != null && !rb.Checked) return;
      else OnRedraw();
    }

    private void RedrawNecessary(object sender, EventArgs e) {
      MarkRedrawNecessary();
    }

    private void RedrawNecessary(object sender, EventArgs<int, int> e) {
      MarkRedrawNecessary();
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      SetupDefaultBitmap();
      if (!showingMessage) MarkRedrawNecessary();
      else OnRedraw();
    }
    #endregion

    public QAPVisualizationControl() {
      InitializeComponent();
      showingMessage = false;
      redrawButton.Text = String.Empty;
      redrawButton.Image = VSImageLibrary.Refresh;
      SetupDefaultBitmap();
    }

    private void SetupDefaultBitmap() {
      if (defaultBitmap != null) {
        defaultBitmap.Dispose();
        defaultBitmap = null;
      }
      if (pictureBox.Width > 0 && pictureBox.Height > 0) {
        defaultBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
        WriteCenteredTextToBitmap(ref defaultBitmap, "No visualization available");
      }
    }

    private void WriteCenteredTextToBitmap(ref Bitmap bitmap, string text) {
      if (bitmap == null) return;
      using (Graphics g = Graphics.FromImage(bitmap)) {
        g.Clear(Color.White);

        Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
        SizeF strSize = g.MeasureString(text, font);
        if (strSize.Width + 50 > pictureBox.Width) {
          Match m = Regex.Match(text, @"\b\w+[.,]*\b*");
          StringBuilder builder = new StringBuilder();
          while (m.Success) {
            builder.Append(m.Value + " ");
            Match next = m.NextMatch();
            if (g.MeasureString(builder.ToString() + " " + next.Value, font).Width + 50 > pictureBox.Width)
              builder.AppendLine();
            m = next;
          }
          builder.Remove(builder.Length - 1, 1);
          text = builder.ToString();
          strSize = g.MeasureString(text, font);
        }
        g.DrawString(text, font, Brushes.Black, (float)(pictureBox.Width - strSize.Width) / 2.0f, (float)(pictureBox.Height - strSize.Height) / 2.0f);
      }
    }

    private void OnRedraw() {
      if (InvokeRequired) {
        Invoke((Action)OnRedraw, null);
      } else {
        GenerateImage();
      }
    }

    private void GenerateImage() {
      if (pictureBox.Width > 0 && pictureBox.Height > 0) {
        Bitmap newBitmap = null;
        stressLabel.Text = "-";
        if (distancesRadioButton.Checked && Distances != null && Distances.Rows > 0
          && Distances.Rows == Distances.Columns) {
          if (Distances.Rows > 35) {
            newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            WriteCenteredTextToBitmap(ref newBitmap, "Problem dimension is too large for visualization.");
            showingMessage = true;
          } else newBitmap = GenerateDistanceImage();
        } else if (weightsRadioButton.Checked && Weights != null && Weights.Rows > 0
          && Weights.Rows == Weights.Columns) {
          if (Weights.Rows > 35) {
            newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            WriteCenteredTextToBitmap(ref newBitmap, "Problem dimension is too large for visualization.");
            showingMessage = true;
          } else newBitmap = GenerateWeightsImage();
        } else if (assignmentRadioButton.Checked
          && Assignment != null && Assignment.Length > 0
          && Weights != null && Weights.Rows > 0
          && Distances != null && Distances.Rows > 0
          && Weights.Rows == Weights.Columns
          && Distances.Rows == Distances.Columns
          && Assignment.Length == Weights.Rows
          && Assignment.Length == Distances.Rows
          && Assignment.Validate()) {
          if (Assignment.Length > 35) {
            newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            WriteCenteredTextToBitmap(ref newBitmap, "Problem dimension is too large for visualization.");
            showingMessage = true;
          } else newBitmap = GenerateAssignmentImage();
        }

        pictureBox.Image = newBitmap != null ? newBitmap : defaultBitmap;
        if (bitmap != null) bitmap.Dispose();
        if (newBitmap != null) bitmap = newBitmap;
        else {
          bitmap = null;
          showingMessage = true;
        }
      }
    }

    private void MarkRedrawNecessary() {
      if (pictureBox.Width > 0 && pictureBox.Height > 0) {
        Bitmap newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
        stressLabel.Text = "-";
        WriteCenteredTextToBitmap(ref newBitmap, "Please refresh view.");
        showingMessage = false; // we're showing a message, but we should be showing the visualization, so this is false

        pictureBox.Image = newBitmap != null ? newBitmap : defaultBitmap;
        if (bitmap != null) bitmap.Dispose();
        if (newBitmap != null) bitmap = newBitmap;
        else bitmap = null;
      }
    }

    #region Draw distances
    private Bitmap GenerateDistanceImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        Bitmap newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

        DoubleMatrix coordinates;
        double stress = double.NaN;
        try {
          coordinates = MultidimensionalScaling.KruskalShepard(distances);
          stress = MultidimensionalScaling.CalculateNormalizedStress(distances, coordinates);
          stressLabel.Text = stress.ToString("0.00", CultureInfo.CurrentCulture.NumberFormat);
        } catch {
          WriteCenteredTextToBitmap(ref newBitmap, "Distance matrix is not symmetric");
          showingMessage = true;
          stressLabel.Text = "-";
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
            if (distances[i, j] > maxDistance) maxDistance = distances[i, j];
            if (distances[j, i] > maxDistance) maxDistance = distances[j, i];
          }
        }

        int border = 20;
        double xStep = xMax != xMin ? (pictureBox.Width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (pictureBox.Height - 2 * border) / (yMax - yMin) : 1;

        Point[] points = new Point[coordinates.Rows];
        for (int i = 0; i < coordinates.Rows; i++)
          points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                newBitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

        Random rand = new Random();
        using (Graphics graphics = Graphics.FromImage(newBitmap)) {
          graphics.Clear(Color.White);
          graphics.DrawString("Showing locations spaced out according to their distances", Font, Brushes.Black, 5, 2);

          for (int i = 0; i < coordinates.Rows - 1; i++) {
            for (int j = i + 1; j < coordinates.Rows; j++) {
              Point start = points[i], end = points[j];
              string caption = String.Empty;
              double d = Math.Max(distances[i, j], distances[j, i]);
              float width = (float)Math.Ceiling(5.0 * d / maxDistance);
              if (d > 0) {
                graphics.DrawLine(new Pen(Color.IndianRed, width), start, end);
                if (distances[i, j] != distances[j, i])
                  caption = distances[i, j].ToString(CultureInfo.InvariantCulture.NumberFormat)
                    + " / " + distances[j, i].ToString(CultureInfo.InvariantCulture.NumberFormat);
                else
                  caption = distances[i, j].ToString(CultureInfo.InvariantCulture.NumberFormat);
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
        showingMessage = false;
        return newBitmap;
      }
      return null;
    }
    #endregion

    #region Draw weights
    private Bitmap GenerateWeightsImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        Bitmap newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

        double maxWeight = double.MinValue;
        for (int i = 0; i < weights.Rows; i++)
          for (int j = i + 1; j < weights.Rows; j++) {
            if (weights[i, j] > maxWeight)
              maxWeight = weights[i, j] + weights[j, i];
          }

        DoubleMatrix distances = new DoubleMatrix(weights.Rows, weights.Columns);
        for (int i = 0; i < distances.Rows; i++)
          for (int j = 0; j < distances.Columns; j++) {
            distances[i, j] = maxWeight + 1 - weights[i, j];
          }

        DoubleMatrix coordinates;
        double stress = double.NaN;
        try {
          coordinates = MultidimensionalScaling.KruskalShepard(distances);
          stress = MultidimensionalScaling.CalculateNormalizedStress(distances, coordinates);
          stressLabel.Text = stress.ToString("0.00", CultureInfo.CurrentCulture.NumberFormat);
        } catch {
          WriteCenteredTextToBitmap(ref newBitmap, "Weights matrix is not symmetric");
          showingMessage = true;
          stressLabel.Text = "-";
          return newBitmap;
        }
        double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
        for (int i = 0; i < coordinates.Rows; i++) {
          if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
          if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
          if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
          if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
        }

        int border = 20;
        double xStep = xMax != xMin ? (pictureBox.Width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (pictureBox.Height - 2 * border) / (yMax - yMin) : 1;

        Point[] points = new Point[coordinates.Rows];
        for (int i = 0; i < coordinates.Rows; i++)
          points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                newBitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

        Random rand = new Random();
        using (Graphics graphics = Graphics.FromImage(newBitmap)) {
          graphics.Clear(Color.White);
          graphics.DrawString("Showing facilities spaced out according to their weights", Font, Brushes.Black, 5, 2);

          for (int i = 0; i < coordinates.Rows - 1; i++) {
            for (int j = i + 1; j < coordinates.Rows; j++) {
              Point start = points[i], end = points[j];
              string caption = String.Empty;
              double d = Math.Max(distances[i, j], distances[j, i]);
              double w = weights[i, j];
              if (w > 0) {
                float width = (float)Math.Ceiling(3.0 * w / maxWeight);
                graphics.DrawLine(new Pen(Color.MediumBlue, width), start, end);
                caption = w.ToString(CultureInfo.InvariantCulture.NumberFormat);
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
        showingMessage = false;
        return newBitmap;
      }
      return null;
    }
    #endregion

    #region Draw assignment
    private Bitmap GenerateAssignmentImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        Bitmap newBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

        for (int i = 0; i < distances.Rows; i++) {
          for (int j = i + 1; j < distances.Rows; j++) {
            if (distances[i, j] != distances[j, i]) {
              WriteCenteredTextToBitmap(ref newBitmap, "Distance matrix is not symmetric");
              stressLabel.Text = "-";
              showingMessage = true;
              return newBitmap;
            }
            if (weights[i, j] != weights[j, i]) {
              WriteCenteredTextToBitmap(ref newBitmap, "Weights matrix is not symmetric");
              stressLabel.Text = "-";
              showingMessage = true;
              return newBitmap;
            }
          }
        }

        DoubleMatrix coordinates = null;
        double stress = double.NaN;
        try {
          coordinates = MultidimensionalScaling.KruskalShepard(distances);
          stress = MultidimensionalScaling.CalculateNormalizedStress(distances, coordinates);
          stressLabel.Text = stress.ToString("0.00", CultureInfo.CurrentCulture.NumberFormat);
        } catch {
          WriteCenteredTextToBitmap(ref newBitmap, "Unknown error");
          showingMessage = true;
          stressLabel.Text = "-";
          return newBitmap;
        }

        double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
        double maxWeight = double.MinValue;
        for (int i = 0; i < coordinates.Rows; i++) {
          if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
          if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
          if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
          if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];

          for (int j = i + 1; j < coordinates.Rows; j++) {
            if (weights[i, j] > maxWeight) maxWeight = weights[i, j];
          }
        }

        int border = 20;
        double xStep = xMax != xMin ? (pictureBox.Width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (pictureBox.Height - 2 * border) / (yMax - yMin) : 1;

        Point[] points = new Point[coordinates.Rows];
        for (int i = 0; i < coordinates.Rows; i++)
          points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                newBitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

        Random rand = new Random();
        using (Graphics graphics = Graphics.FromImage(newBitmap)) {
          graphics.Clear(Color.White);
          for (int i = 0; i < assignment.Length - 1; i++) {
            for (int j = i + 1; j < assignment.Length; j++) {
              Point start, end;
              string caption = String.Empty;
              double d = distances[i, j];
              start = points[assignment[i]];
              end = points[assignment[j]];
              double w = weights[i, j];
              if (w > 0) {
                float width = (float)Math.Ceiling(4.0 * w / maxWeight);
                graphics.DrawLine(new Pen(Color.MediumBlue, width), start, end);
                caption = w.ToString(CultureInfo.InvariantCulture.NumberFormat);
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
        showingMessage = false;
        return newBitmap;
      }
      return null;
    }
    #endregion

    private void CustomDispose(bool disposing) {
      DeregisterDistancesEvents();
      DeregisterWeightsEvents();
      DeregisterAssignmentEvents();
      if (bitmap != null) bitmap.Dispose();
      bitmap = null;
      if (defaultBitmap != null) {
        defaultBitmap.Dispose();
        defaultBitmap = null;
      }
    }
  }
}
