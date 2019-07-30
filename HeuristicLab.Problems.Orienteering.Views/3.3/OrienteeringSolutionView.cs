#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.Orienteering.Views {
  [View("OrienteeringSolution View")]
  [Content(typeof(OrienteeringSolution), true)]
  public partial class OrienteeringSolutionView : ItemView {
    public new OrienteeringSolution Content {
      get { return (OrienteeringSolution)base.Content; }
      set { base.Content = value; }
    }
    public OrienteeringSolutionView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.QualityChanged -= Content_QualityChanged;
      Content.PenaltyChanged -= Content_PenaltyChanged;
      Content.DistanceChanged -= Content_DistanceChanged;
      Content.CoordinatesChanged -= Content_CoordinatesChanged;
      Content.StartingPointChanged -= Content_StartingPointChanged;
      Content.TerminalPointChanged -= Content_TerminalPointChanged;
      Content.ScoresChanged -= Content_ScoresChanged;
      Content.IntegerVectorChanged -= Content_IntegerVectorChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.QualityChanged += Content_QualityChanged;
      Content.PenaltyChanged += Content_PenaltyChanged;
      Content.DistanceChanged += Content_DistanceChanged;
      Content.CoordinatesChanged += Content_CoordinatesChanged;
      Content.StartingPointChanged += Content_StartingPointChanged;
      Content.TerminalPointChanged += Content_TerminalPointChanged;
      Content.ScoresChanged += Content_ScoresChanged;
      Content.IntegerVectorChanged += Content_IntegerVectorChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityValueView.Content = null;
        penaltyValueView.Content = null;
        distanceValueView.Content = null;
        splitContainer.Panel1Collapsed = true;
        pictureBox.Image = null;
        tourViewHost.Content = null;
      } else {
        qualityValueView.Content = Content.Quality;
        penaltyValueView.Content = Content.Penalty;
        distanceValueView.Content = Content.Distance;
        SetPanelCollapsing();
        GenerateImage();
        tourViewHost.Content = Content.IntegerVector;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      qualityValueView.Enabled = Content != null;
      penaltyValueView.Enabled = Content != null;
      distanceValueView.Enabled = Content != null;
      pictureBox.Enabled = Content != null;
      tourGroupBox.Enabled = Content != null;
    }

    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          var coordinates = Content.Coordinates;
          var scores = Content.Scores;
          var integerVector = Content.IntegerVector;
          var bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

          if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2)
            && (scores != null) && (coordinates.Rows == scores.Length)) {
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
              if (integerVector != null && integerVector.Length > 1) {
                Point[] tour = new Point[integerVector.Length];
                for (int i = 0; i < integerVector.Length; i++) {
                  tour[i] = points[integerVector[i]];
                }
                bool visualizePenalty = Content.Penalty != null && Content.Penalty.Value > 0;
                graphics.DrawLines(visualizePenalty ? Pens.Red : Pens.Black, tour);
              }

              double scoreMin = scores.Min();
              double scoreMax = scores.Max();
              double scoreRange = scoreMax - scoreMin;
              for (int i = 0; i < points.Length; i++) {
                double score = scores[i];
                int size = scoreRange.IsAlmost(0.0)
                  ? 6
                  : (int)Math.Round(((score - scoreMin) / scoreRange) * 8 + 2);
                graphics.FillRectangle(Brushes.Red, points[i].X - size / 2, points[i].Y - size / 2, size, size);
              }
              int startingPoint = Content.StartingPoint.Value;
              int terminalPoint = Content.TerminalPoint.Value;
              Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
              var beginSize = graphics.MeasureString("Begin", font);
              if (startingPoint >= 0 && startingPoint < points.Length)
                graphics.DrawString("Begin", font, Brushes.Black, points[startingPoint].X - beginSize.Width, points[startingPoint].Y - beginSize.Height);
              if (terminalPoint >= 0 && terminalPoint < points.Length)
                graphics.DrawString("End", font, Brushes.Black, points[terminalPoint].X, points[terminalPoint].Y);
            }
          } else {
            using (Graphics graphics = Graphics.FromImage(bitmap)) {
              graphics.Clear(Color.White);
              Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
              string text = "No coordinates defined or in wrong format.";
              SizeF strSize = graphics.MeasureString(text, font);
              graphics.DrawString(text, font, Brushes.Black, (float)(pictureBox.Width - strSize.Width) / 2.0f, (float)(pictureBox.Height - strSize.Height) / 2.0f);
            }
          }
          pictureBox.Image = bitmap;
        }
      }
    }

    private void SetPanelCollapsing() {
      splitContainer.Panel1Collapsed = qualityValueView.Content == null && penaltyValueView.Content == null && distanceValueView.Content == null;
    }

    private void Content_QualityChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_QualityChanged, sender, e);
      else {
        qualityValueView.Content = Content.Quality;
        SetPanelCollapsing();
      }
    }
    private void Content_PenaltyChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_PenaltyChanged, sender, e);
      else {
        penaltyValueView.Content = Content.Penalty;
        GenerateImage();
        SetPanelCollapsing();
      }
    }

    private void Content_DistanceChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_DistanceChanged, sender, e);
      else {
        distanceValueView.Content = Content.Distance;
        SetPanelCollapsing();
      }
    }

    private void Content_CoordinatesChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_CoordinatesChanged, sender, e);
      else GenerateImage();
    }

    private void Content_StartingPointChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_StartingPointChanged, sender, e);
      else GenerateImage();
    }

    private void Content_TerminalPointChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_TerminalPointChanged, sender, e);
      else GenerateImage();
    }

    private void Content_ScoresChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ScoresChanged, sender, e);
      else GenerateImage();
    }

    private void Content_IntegerVectorChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_IntegerVectorChanged, sender, e);
      else {
        GenerateImage();
        tourViewHost.Content = Content.IntegerVector;
      }
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
