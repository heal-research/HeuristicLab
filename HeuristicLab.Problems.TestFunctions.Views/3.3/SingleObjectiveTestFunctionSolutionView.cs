#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.TestFunctions.Views {
  /// <summary>
  /// A view for a SingleObjectiveTestFunctions solution.
  /// </summary>
  [View("Single Objective Test Functions View")]
  [Content(typeof(SingleObjectiveTestFunctionSolution), true)]
  public partial class SingleObjectiveTestFunctionSolutionView : ItemView {
    private Bitmap backgroundImage;

    public new SingleObjectiveTestFunctionSolution Content {
      get { return (SingleObjectiveTestFunctionSolution)base.Content; }
      set { base.Content = value; }
    }

    public SingleObjectiveTestFunctionSolutionView() {
      InitializeComponent();
      pictureBox.SizeChanged += new EventHandler(pictureBox_SizeChanged);
      qualityView.ReadOnly = true;
      realVectorView.ReadOnly = true;
      backgroundImage = null;
    }

    protected override void DeregisterContentEvents() {
      Content.BestKnownRealVectorChanged -= new EventHandler(Content_BestKnownRealVectorChanged);
      Content.BestRealVectorChanged -= new EventHandler(Content_BestRealVectorChanged);
      Content.QualityChanged -= new EventHandler(Content_QualityChanged);
      Content.PopulationChanged -= new EventHandler(Content_PopulationChanged);
      Content.BoundsChanged -= new EventHandler(Content_BoundsChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.BestKnownRealVectorChanged += new EventHandler(Content_BestKnownRealVectorChanged);
      Content.BestRealVectorChanged += new EventHandler(Content_BestRealVectorChanged);
      Content.QualityChanged += new EventHandler(Content_QualityChanged);
      Content.PopulationChanged += new EventHandler(Content_PopulationChanged);
      Content.BoundsChanged += new EventHandler(Content_BoundsChanged);
    }

    private void Content_BestKnownRealVectorChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_BestKnownRealVectorChanged), sender, e);
      else {
        GenerateImage();
      }
    }

    private void Content_BestRealVectorChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_BestRealVectorChanged), sender, e);
      else {
        realVectorView.Content = Content.BestRealVector;
        pictureBox.Visible = Content.BestRealVector.Length == 2;
        GenerateImage();
      }
    }

    private void Content_QualityChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_QualityChanged), sender, e);
      else {
        qualityView.Content = Content.BestQuality;
        pictureBox.Visible = Content.BestRealVector.Length == 2;
        GenerateImage();
      }
    }

    private void Content_PopulationChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_PopulationChanged), sender, e);
      else {
        GenerateImage();
      }
    }

    private void Content_BoundsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_BoundsChanged), sender, e);
      else
        GenerateImage();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityView.Content = null;
        realVectorView.Content = null;
      } else {
        qualityView.Content = Content.BestQuality;
        realVectorView.Content = Content.BestRealVector;

        pictureBox.Visible = Content.BestRealVector.Length == 2;
        GenerateImage();
      }
    }

    protected override void OnPaint(PaintEventArgs e) {
      GenerateImage();
      base.OnPaint(e);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      qualityView.Enabled = Content != null;
      realVectorView.Enabled = Content != null;
    }

    private void GenerateImage() {
      if (pictureBox.Enabled && pictureBox.Width > 0 && pictureBox.Height > 0) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          if (backgroundImage == null) {
            GenerateBackgroundImage();
            pictureBox.Image = backgroundImage;
          }
          pictureBox.Refresh();
          DoubleMatrix bounds = Content.Bounds;
          if (bounds == null) bounds = Content.TestFunction.Bounds;
          double xMin = bounds[0, 0], xMax = bounds[0, 1], yMin = bounds[1 % bounds.Rows, 0], yMax = bounds[1 % bounds.Rows, 1];
          double xStep = backgroundImage.Width / (xMax - xMin), yStep = backgroundImage.Height / (yMax - yMin);
          using (Graphics graphics = pictureBox.CreateGraphics()) {
            if (Content.BestKnownRealVector != null) {
              Pen cross = new Pen(Brushes.Red, 2.0f);
              float a = (float)((Content.BestKnownRealVector[0] - xMin) * xStep);
              float b = (float)((Content.BestKnownRealVector[1] - yMin) * yStep);
              graphics.DrawLine(cross, a - 4, b - 4, a + 4, b + 4);
              graphics.DrawLine(cross, a - 4, b + 4, a + 4, b - 4);
            }
            if (Content.Population != null) {
              foreach (RealVector vector in Content.Population)
                graphics.FillEllipse(Brushes.Blue, (float)((vector[0] - xMin) * xStep - 4), (float)((vector[1] - yMin) * yStep - 4), 8.0f, 8.0f);
            }
            if (Content.BestRealVector != null) {
              graphics.FillEllipse(Brushes.Green, (float)((Content.BestRealVector[0] - xMin) * xStep - 5), (float)((Content.BestRealVector[1] - yMin) * yStep - 5), 10.0f, 10.0f);
            }
          }
        }
      }
    }

    private void GenerateBackgroundImage() {
      if (backgroundImage != null)
        backgroundImage.Dispose();
      backgroundImage = new Bitmap(pictureBox.Width, pictureBox.Height);
      DoubleMatrix bounds = Content.Bounds;
      if (bounds == null) bounds = Content.TestFunction.Bounds;
      double xMin = bounds[0, 0], xMax = bounds[0, 1], yMin = bounds[1 % bounds.Rows, 0], yMax = bounds[1 % bounds.Rows, 1];
      double xStep = (xMax - xMin) / backgroundImage.Width, yStep = (yMax - yMin) / backgroundImage.Height;
      double minPoint = Double.MaxValue, maxPoint = Double.MinValue;
      DoubleMatrix points = new DoubleMatrix(backgroundImage.Height, backgroundImage.Width);
      for (int i = 0; i < backgroundImage.Width; i++)
        for (int j = 0; j < backgroundImage.Height; j++) {
          points[j, i] = Content.TestFunction.Evaluate2D(xMin + i * xStep, yMin + j * yStep);
          if (points[j, i] < minPoint) minPoint = points[j, i];
          if (points[j, i] > maxPoint) maxPoint = points[j, i];
        }
      double grayStep;
      if (maxPoint == minPoint) grayStep = -1;
      else grayStep = 100 / (maxPoint - minPoint);

      for (int i = 0; i < backgroundImage.Width; i++)
        for (int j = 0; j < backgroundImage.Height; j++) {
          int luminosity = (grayStep > 0) ? (int)Math.Round((points[j, i] - minPoint) * grayStep) : (128);
          backgroundImage.SetPixel(i, j, Color.FromArgb(255 - luminosity, 255 - luminosity, 255 - luminosity));
        }
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      if (backgroundImage != null) backgroundImage.Dispose();
      backgroundImage = null;
      pictureBox.Image = null;
      GenerateImage();
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      pictureBox.Enabled = false;
      if (backgroundImage != null) backgroundImage.Dispose();
      backgroundImage = null;
      pictureBox.Image = null;
      base.OnClosing(e);
    }
  }
}
