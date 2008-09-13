#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Drawing;
using System.Linq;
using HeuristicLab.Charting;

namespace HeuristicLab.CEDMA.Charting {
  public class BubbleChart : Chart {
    private const double maxXJitterPercent = 0.05;
    private const double maxYJitterPercent = 0.05;
    private const int minBubbleSize = 5;
    private const int maxBubbleSize = 30;
    private const int maxAlpha = 255;
    private const int minAlpha = 50;
    private static readonly Color defaultColor = Color.Blue;

    private List<string> dimensions;
    private Dictionary<string, List<double>> values;
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private string xDimension;
    private string yDimension;
    private string sizeDimension;
    private bool invertSize;
    private double minX = double.PositiveInfinity;
    private double minY = double.PositiveInfinity;
    private double maxX = double.NegativeInfinity;
    private double maxY = double.NegativeInfinity;

    private Random random = new Random();

    public BubbleChart(PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      dimensions = new List<string>();
      values = new Dictionary<string, List<double>>();
    }
    public BubbleChart(double x1, double y1, double x2, double y2)
      : this(new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void AddDimension(string name) {
      dimensions.Add(name);
      values.Add(name, new List<double>());
    }
    public void RemoveDimension(string name) {
      dimensions.Remove(name);
      values.Remove(name);
    }

    public void AddDataPoint(string dimension, double value) {
      values[dimension].Add(value);
    }

    public void SetBubbleSizeDimension(string dimension, bool inverted) {
      this.sizeDimension = dimension;
      this.invertSize = inverted;
      Repaint();
      EnforceUpdate();
    }

    public void ShowXvsY(string xDimension, string yDimension) {
      if(this.xDimension != xDimension || this.yDimension != yDimension) {
        this.xDimension = xDimension;
        this.yDimension = yDimension;
        ResetViewSize();
        Repaint();
        ZoomToViewSize();
      }
    }

    public override void MouseClick(Point point, System.Windows.Forms.MouseButtons button) {
      // should select the model in the underlying model
    }

    public override void MouseDoubleClick(Point point, System.Windows.Forms.MouseButtons button) {
      // should open the model
    }

    internal void SetJitter(double xJitterFactor, double yJitterFactor) {
      this.xJitterFactor = xJitterFactor * maxXJitterPercent * Size.Width;
      this.yJitterFactor = yJitterFactor * maxYJitterPercent * Size.Height;
      Repaint();
      EnforceUpdate();
    }

    private void Repaint() {

      List<double> xs = values[xDimension];
      List<double> ys = values[yDimension];

      List<double> sizes = null;
      double maxSize = 1;
      double minSize = 1;
      if(sizeDimension != null && values.ContainsKey(sizeDimension)) {
        sizes = values[sizeDimension];
        maxSize = sizes.Max();
        minSize = sizes.Min();
      }
      UpdateEnabled = false;
      Group.Clear();
      Group.Add(new Axis(this, 0, 0, AxisType.Both));
      UpdateViewSize(0, 0, 5);
      Group points = new Group(this);
      int n = Math.Min(xs.Count, ys.Count);
      if(sizes != null) n = Math.Min(n, sizes.Count);
      for(int i = 0; i < n; i++) {
        double x = xs[i] + (random.NextDouble() * 2.0 - 1.0) * xJitterFactor;
        double y = ys[i] + (random.NextDouble() * 2.0 - 1.0) * yJitterFactor;
        int size;
        if(sizes != null) {
          size = CalculateSize(sizes[i], minSize, maxSize);
        } else {
          size = minBubbleSize;
        }

        if(double.IsInfinity(x) || x == double.MaxValue || x == double.MinValue) x = double.NaN;
        if(double.IsInfinity(y) || y == double.MaxValue || y == double.MinValue) y = double.NaN;
        if(!double.IsNaN(x) && !double.IsNaN(y)) {
          UpdateViewSize(x, y, size);
          int alpha = CalculateAlpha(size);
          Pen pen = new Pen(Color.FromArgb(alpha, defaultColor));
          Brush brush = pen.Brush;
          points.Add(new FixedSizeCircle(this, x, y, size, pen, brush));
        }
      }
      Group.Add(points);
      UpdateEnabled = true;
    }

    private int CalculateSize(double size, double minSize, double maxSize) {
      double sizeDifference = ((size - minSize) / (maxSize - minSize) * (maxBubbleSize - minBubbleSize));
      if(invertSize) return maxBubbleSize - (int)sizeDifference;
      else return minBubbleSize + (int)sizeDifference;
    }

    private int CalculateAlpha(int size) {
      return maxAlpha - (int)((double)(size - minBubbleSize) / (double)(maxBubbleSize - minBubbleSize) * (double)(maxAlpha - minAlpha));
    }

    private void ZoomToViewSize() {
      if(minX < maxX && minY < maxY) {
        // enlarge view by 5% on each side
        double width = maxX - minX;
        double height = maxY - minY;
        minX = minX - width * 0.05;
        maxX = maxX + width * 0.05;
        minY = minY - height * 0.05;
        maxY = maxY + height * 0.05;
        ZoomIn(minX, minY, maxX, maxY);
      }
    }

    private void UpdateViewSize(double x, double y, double size) {
      if(x - size < minX) minX = x - size;
      if(x + size > maxX) maxX = x + size;
      if(y - size < minY) minY = y + size;
      if(y + size > maxY) maxY = y + size;
    }

    private void ResetViewSize() {
      minX = double.PositiveInfinity;
      maxX = double.NegativeInfinity;
      minY = double.PositiveInfinity;
      maxY = double.NegativeInfinity;
    }
  }
}
