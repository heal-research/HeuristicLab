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
using HeuristicLab.Problems.VehicleRouting.Encodings;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("VRPSolution View")]
  [Content(typeof(VRPSolution), true)]
  public sealed partial class VRPSolutionView : ItemView {
    public new VRPSolution Content {
      get { return (VRPSolution)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public VRPSolutionView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.QualityChanged -= new EventHandler(Content_QualityChanged);
      Content.DistanceChanged -= new EventHandler(Content_DistanceChanged);
      Content.OverloadChanged -= new EventHandler(Content_OverloadChanged);
      Content.TardinessChanged -= new EventHandler(Content_TardinessChanged);
      Content.TravelTimeChanged -= new EventHandler(Content_TravelTimeChanged);
      Content.VehicleUtilizationChanged -= new EventHandler(Content_VehicleUtilizationChanged);
      Content.CoordinatesChanged -= new EventHandler(Content_CoordinatesChanged);
      Content.SolutionChanged -= new EventHandler(Content_SolutionChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.QualityChanged += new EventHandler(Content_QualityChanged);
      Content.DistanceChanged += new EventHandler(Content_DistanceChanged);
      Content.OverloadChanged += new EventHandler(Content_OverloadChanged);
      Content.TardinessChanged += new EventHandler(Content_TardinessChanged);
      Content.TravelTimeChanged += new EventHandler(Content_TravelTimeChanged);
      Content.VehicleUtilizationChanged += new EventHandler(Content_VehicleUtilizationChanged);
      Content.CoordinatesChanged += new EventHandler(Content_CoordinatesChanged);
      Content.SolutionChanged += new EventHandler(Content_SolutionChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityViewHost.Content = null;
        pictureBox.Image = null;
        tourViewHost.Content = null;
      } else {
        qualityViewHost.Content = Content.Quality;
        distanceViewHost.Content = Content.Distance;
        overloadViewHost.Content = Content.Overload;
        tardinessViewHost.Content = Content.Tardiness;
        travelTimeViewHost.Content = Content.TravelTime;
        vehicleUtilizationViewHost.Content = Content.VehicleUtilization;

        GenerateImage();
        tourViewHost.Content = Content.Solution;
      }
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      tabControl1.Enabled = Content != null;
      pictureBox.Enabled = Content != null;
      tourGroupBox.Enabled = Content != null;
    }

    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          DoubleMatrix coordinates = Content.Coordinates;
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
            double xStep = xMax != xMin ? (pictureBox.Width - 2 * border) / (xMax - xMin) : 1;
            double yStep = yMax != yMin ? (pictureBox.Height - 2 * border) / (yMax - yMin) : 1;

            Point[] points = new Point[coordinates.Rows];
            for (int i = 0; i < coordinates.Rows; i++)
              points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                    bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

            using (Graphics graphics = Graphics.FromImage(bitmap)) {
              if (Content.Solution != null) {
                foreach (Tour tour in Content.Solution.Tours) {
                  Point[] tourPoints = new Point[tour.Count];
                  for (int i = 0; i < tour.Count; i++) {
                    tourPoints[i] = points[tour[i].Value];
                  }
                  graphics.DrawPolygon(Pens.Black, tourPoints);
                }
              }

              for (int i = 0; i < points.Length; i++)
                graphics.FillRectangle(Brushes.Red, points[i].X - 2, points[i].Y - 2, 6, 6);
            }
          }
          pictureBox.Image = bitmap;
        }
      }
    }

    private void Content_QualityChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_QualityChanged), sender, e);
      else
        qualityViewHost.Content = Content.Quality;
    }
    void Content_DistanceChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DistanceChanged), sender, e);
      else
        distanceViewHost.Content = Content.Distance;
    }
    private void Content_CoordinatesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_CoordinatesChanged), sender, e);
      else
        GenerateImage();
    }
    void Content_OverloadChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_OverloadChanged), sender, e);
      else
        overloadViewHost.Content = Content.Overload;
    }
    void Content_TardinessChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_TardinessChanged), sender, e);
      else
        tardinessViewHost.Content = Content.Tardiness;
    }
    void Content_TravelTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_TravelTimeChanged), sender, e);
      else
        travelTimeViewHost.Content = Content.TravelTime;
    }
    void Content_VehicleUtilizationChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_VehicleUtilizationChanged), sender, e);
      else
        vehicleUtilizationViewHost.Content = Content.VehicleUtilization;
    }
    private void Content_SolutionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_SolutionChanged), sender, e);
      else {
        GenerateImage();
        tourViewHost.Content = Content.Solution;
      }
    }
    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
