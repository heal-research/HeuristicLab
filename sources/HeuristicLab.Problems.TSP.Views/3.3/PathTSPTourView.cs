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
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.TSP.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("PathTSPTour View")]
  [Content(typeof(PathTSPTour), true)]
  public sealed partial class PathTSPTourView : ItemView {
    public new PathTSPTour Content {
      get { return (PathTSPTour)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public PathTSPTourView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public PathTSPTourView(PathTSPTour content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesChanged -= new EventHandler(Content_CoordinatesChanged);
      Content.PermutationChanged -= new EventHandler(Content_PermutationChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesChanged += new EventHandler(Content_CoordinatesChanged);
      Content.PermutationChanged += new EventHandler(Content_PermutationChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pictureBox.Image = null;
        pictureBox.Enabled = false;
      } else {
        pictureBox.Enabled = true;
        GenerateImage();
      }
    }

    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          DoubleMatrix coordinates = Content.Coordinates;
          Permutation permutation = Content.Permutation;
          Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

          if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2)) {
            double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
            for (int i = 0; i < coordinates.Rows; i++) {
              if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
              if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
              if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
              if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
            }

            int border = 20;
            double xStep = (pictureBox.Width - 2 * border) / (xMax - xMin);
            double yStep = (pictureBox.Height - 2 * border) / (yMax - yMin);

            Point[] points = new Point[coordinates.Rows];
            for (int i = 0; i < coordinates.Rows; i++)
              points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                    border + ((int)((coordinates[i, 1] - yMin) * yStep)));

            Graphics graphics = Graphics.FromImage(bitmap);
            if ((permutation != null) && (permutation.Length == coordinates.Rows) && (permutation.Validate())) {
              Point[] tour = new Point[permutation.Length];
              for (int i = 0; i < permutation.Length; i++) {
                tour[i] = points[permutation[i]];
              }
              graphics.DrawPolygon(Pens.Black, tour);
            }
            for (int i = 0; i < points.Length; i++)
              graphics.FillRectangle(Brushes.Red, points[i].X - 2, points[i].Y - 2, 6, 6);
            graphics.Dispose();
          }
          pictureBox.Image = bitmap;
        }
      }
    }

    private void Content_CoordinatesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_CoordinatesChanged), sender, e);
      else
        GenerateImage();
    }
    private void Content_PermutationChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_PermutationChanged), sender, e);
      else
        GenerateImage();
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
