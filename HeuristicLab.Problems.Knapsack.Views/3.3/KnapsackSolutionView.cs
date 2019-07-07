#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.Knapsack.Views {
  /// <summary>
  /// A view for a Knapsack solution.
  /// </summary>
  [View("Knapsack View")]
  [Content(typeof(KnapsackSolution), true)]
  public partial class KnapsackSolutionView : HeuristicLab.Core.Views.ItemView {
    public new KnapsackSolution Content {
      get { return (KnapsackSolution)base.Content; }
      set { base.Content = value; }
    }

    public KnapsackSolutionView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.BinaryVectorChanged -= new EventHandler(Content_BinaryVectorChanged);
      Content.CapacityChanged -= new EventHandler(Content_CapacityChanged);
      Content.WeightsChanged -= new EventHandler(Content_WeightsChanged);
      Content.ValuesChanged -= new EventHandler(Content_ValuesChanged);
      Content.QualityChanged -= new EventHandler(Content_QualityChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.BinaryVectorChanged += new EventHandler(Content_BinaryVectorChanged);
      Content.CapacityChanged += new EventHandler(Content_CapacityChanged);
      Content.WeightsChanged += new EventHandler(Content_WeightsChanged);
      Content.ValuesChanged += new EventHandler(Content_ValuesChanged);
      Content.QualityChanged += new EventHandler(Content_QualityChanged);
    }

    private void Content_BinaryVectorChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_BinaryVectorChanged), sender, e);
      else
        GenerateImage();
    }

    private void Content_QualityChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_QualityChanged), sender, e);
      else
        GenerateImage();
    }

    private void Content_CapacityChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_CapacityChanged), sender, e);
      else
        GenerateImage();
    }

    private void Content_WeightsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_WeightsChanged), sender, e);
      else
        GenerateImage();
    }

    private void Content_ValuesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValuesChanged), sender, e);
      else
        GenerateImage();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        pictureBox.Image = null;
      else
        GenerateImage();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      pictureBox.Enabled = Content != null;
    }

    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
          using (Graphics graphics = Graphics.FromImage(bitmap)) {
            int border = 10;

            int borderX = border;
            if (pictureBox.Width - border * 2 < 0)
              borderX = 0;
            int width = pictureBox.Width - borderX * 2;

            int borderY = border;
            if (pictureBox.Height - border * 2 < 0)
              borderY = 0;
            int height = pictureBox.Height - borderY * 2;

            //preprocess
            double capacity = Content.Capacity.Value;

            if (capacity != 0) {
              double packedCapacity = 0;
              double maxValue = 0;
              for (int i = 0; i < Content.BinaryVector.Length; i++) {
                if (Content.BinaryVector[i]) {
                  packedCapacity += Content.Weights[i];
                }

                if (Content.Values[i] > maxValue)
                  maxValue = Content.Values[i];
              }

              int knapsackHeight = height;
              if (packedCapacity > capacity) {
                knapsackHeight = (int)Math.Round(capacity / packedCapacity * (double)height);
              }

              //draw knapsack
              using (Pen pen = new Pen(Color.Black, 2)) {
                graphics.DrawRectangle(pen,
                  borderX - 2, pictureBox.Height - borderY - knapsackHeight - 2, width + 4, knapsackHeight + 4);
              }

              //draw items sorted by value
              List<int> sortedIndices = new List<int>();
              for (int i = 0; i < Content.BinaryVector.Length; i++) {
                if (Content.BinaryVector[i]) {
                  sortedIndices.Add(i);
                }
              }

              sortedIndices.Sort(
                delegate(int i, int j) {
                  if (Content.Values[i] < Content.Values[j])
                    return -1;
                  else if (Content.Values[i] > Content.Values[j])
                    return 1;
                  else
                    return 0;
                });

              int currentPosition = pictureBox.Height - borderY;
              foreach (int i in sortedIndices) {
                if (Content.BinaryVector[i]) {

                  double weight = Content.Weights[i];
                  double factor = weight / capacity;
                  int elementHeight = (int)Math.Floor(knapsackHeight * factor);

                  double value = Content.Values[i];
                  //color according to value
                  int colorValue = 0;
                  if (value != 0)
                    colorValue = (int)Math.Round(255.0 * value / maxValue);
                  Color color = Color.FromArgb(
                    0, 0, colorValue);

                  using (Brush brush = new SolidBrush(color)) {
                    graphics.FillRectangle(brush,
                      borderX, currentPosition - elementHeight, width, elementHeight);
                  }
                  graphics.DrawRectangle(Pens.White,
                    borderX, currentPosition - elementHeight, width, elementHeight);

                  currentPosition -= elementHeight;
                }
              }
            }
          }
          pictureBox.Image = bitmap;
        }
      }
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
