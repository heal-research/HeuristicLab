#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using System.Text;

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

      ToolTip tt = new ToolTip();
      tt.SetToolTip(pictureBox, "Legend: Blue = Depot, Black = City (OK), Orange = City (too early), Red = City (too late)");
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesChanged -= new EventHandler(Content_CoordinatesChanged);
      Content.SolutionChanged -= new EventHandler(Content_SolutionChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesChanged += new EventHandler(Content_CoordinatesChanged);
      Content.SolutionChanged += new EventHandler(Content_SolutionChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pictureBox.Image = null;
      } else {
        GenerateImage();
        UpdateTourView();
      }
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      pictureBox.Enabled = Content != null;
      tourGroupBox.Enabled = Content != null;
    }

    private void UpdateTourView() {
      StringBuilder sb = new StringBuilder();

      if (Content != null && Content.Solution != null) {
        foreach (Tour tour in Content.Solution.GetTours(new ValueLookupParameter<DoubleMatrix>("DistanceMatrix", Content.DistanceMatrix))) {
          foreach (int city in tour.Cities) {
            sb.Append(city);
            sb.Append(" ");
          }
          sb.AppendLine();
        }
      }

      valueTextBox.Text = sb.ToString();
    }

    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          DoubleMatrix coordinates = Content.Coordinates;
          DoubleMatrix distanceMatrix = Content.DistanceMatrix;
          BoolValue useDistanceMatrix = Content.UseDistanceMatrix;
          DoubleArray dueTime = Content.DueTime;
          DoubleArray serviceTime = Content.ServiceTime;
          DoubleArray readyTime = Content.ReadyTime;

          Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

          Pen[] pens = {new Pen(Color.FromArgb(92,20,237)), new Pen(Color.FromArgb(237,183,20)), new Pen(Color.FromArgb(237,20,219)), new Pen(Color.FromArgb(20,237,76)),
                    new Pen(Color.FromArgb(237,61,20)), new Pen(Color.FromArgb(115,78,26)), new Pen(Color.FromArgb(20,237,229)), new Pen(Color.FromArgb(39,101,19)),
                    new Pen(Color.FromArgb(230,170,229)), new Pen(Color.FromArgb(142,136,89)), new Pen(Color.FromArgb(157,217,166)), new Pen(Color.FromArgb(31,19,101)),
                    new Pen(Color.FromArgb(173,237,20)), new Pen(Color.FromArgb(230,231,161)), new Pen(Color.FromArgb(142,89,89)), new Pen(Color.FromArgb(93,89,142)),
                    new Pen(Color.FromArgb(146,203,217)), new Pen(Color.FromArgb(101,19,75)), new Pen(Color.FromArgb(198,20,237)), new Pen(Color.FromArgb(185,185,185)),
                    new Pen(Color.FromArgb(179,32,32)), new Pen(Color.FromArgb(18,119,115)), new Pen(Color.FromArgb(104,158,239)), new Pen(Color.Black)};

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

            using (Graphics graphics = Graphics.FromImage(bitmap)) {
              if (Content.Solution != null) {
                int currentTour = 0;
                foreach (Tour tour in Content.Solution.GetTours(new ValueLookupParameter<DoubleMatrix>("DistanceMatrix", distanceMatrix))) {
                  double t = 0.0;
                  Point[] tourPoints = new Point[tour.Cities.Count + 2];
                  Brush[] customerBrushes = new Brush[tour.Cities.Count];
                  int lastCustomer = 0;

                  for (int i = -1; i <= tour.Cities.Count; i++) {
                    int location = 0;

                    if (i == -1 || i == tour.Cities.Count)
                      location = 0; //depot
                    else
                      location = tour.Cities[i];

                    Point locationPoint = new Point(border + ((int)((coordinates[location, 0] - xMin) * xStep)),
                                    bitmap.Height - (border + ((int)((coordinates[location, 1] - yMin) * yStep))));
                    tourPoints[i + 1] = locationPoint;

                    if (i != -1 && i != tour.Cities.Count) {
                      Brush customerBrush = Brushes.Black;

                      t += VRPUtilities.GetDistance(
                        lastCustomer, location, coordinates, distanceMatrix, useDistanceMatrix);

                      if (t < readyTime[location]) {
                        t = readyTime[location];
                        customerBrush = Brushes.Orange;
                      } else if (t > dueTime[location]) {
                        customerBrush = Brushes.Red;
                      }

                      t += serviceTime[location];
                      customerBrushes[i] = customerBrush;
                    }
                    lastCustomer = location;
                  }

                  graphics.DrawPolygon(pens[((currentTour >= pens.Length) ? (pens.Length - 1) : (currentTour))], tourPoints);

                  for (int i = 0; i < tour.Cities.Count; i++) {
                    graphics.FillRectangle(customerBrushes[i], tourPoints[i + 1].X - 3, tourPoints[i + 1].Y - 3, 6, 6);
                  }

                  graphics.FillEllipse(Brushes.Blue, tourPoints[0].X - 5, tourPoints[0].Y - 5, 10, 10);

                  currentTour++;
                }
              } else {
                Point locationPoint;
                //just draw customers
                for (int i = 1; i < coordinates.Rows; i++) {
                  locationPoint = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                  bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

                  graphics.FillRectangle(Brushes.Black, locationPoint.X - 3, locationPoint.Y - 3, 6, 6);
                }

                locationPoint = new Point(border + ((int)((coordinates[0, 0] - xMin) * xStep)),
                                  bitmap.Height - (border + ((int)((coordinates[0, 1] - yMin) * yStep))));
                graphics.FillEllipse(Brushes.Blue, locationPoint.X - 5, locationPoint.Y - 5, 10, 10);
              }
            }
          }

          for (int i = 0; i < pens.Length; i++)
            pens[i].Dispose();

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
    private void Content_SolutionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_SolutionChanged), sender, e);
      else {
        GenerateImage();
        UpdateTourView();
      }
    }
    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
