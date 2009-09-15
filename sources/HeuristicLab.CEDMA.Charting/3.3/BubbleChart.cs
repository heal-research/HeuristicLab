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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.SparseMatrix;

namespace HeuristicLab.CEDMA.Charting {
  public abstract class BubbleChart : Chart {
    private const double maxXJitterPercent = 0.05;
    private const double maxYJitterPercent = 0.05;
    private const int minBubbleSize = 5;
    private const int maxBubbleSize = 30;
    private const int maxAlpha = 255;
    private const int minAlpha = 50;
    private static readonly Color defaultColor = Color.Blue;
    private static readonly Color selectionColor = Color.Red;

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

    protected Group points;
    protected VisualMatrix matrix;
    protected Dictionary<IPrimitive, VisualMatrixRow> primitiveToMatrixRowDictionary;

    public BubbleChart(VisualMatrix matrix, PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      primitiveToMatrixRowDictionary = new Dictionary<IPrimitive, VisualMatrixRow>();
      this.matrix = matrix;
    }

    public BubbleChart(VisualMatrix matrix, double x1, double y1, double x2, double y2)
      : this(matrix, new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void SetBubbleSizeDimension(string dimension, bool inverted) {
      this.sizeDimension = dimension;
      this.invertSize = inverted;
      Refresh();
    }

    public void Refresh() {
      Repaint();
      EnforceUpdate();
    }

    public void ShowXvsY(string xDimension, string yDimension) {
      if (this.xDimension != xDimension || this.yDimension != yDimension) {
        this.xDimension = xDimension;
        this.yDimension = yDimension;

        ResetViewSize();
        Repaint();
        ZoomToViewSize();
      }
    }

    public void SetJitter(double xJitterFactor, double yJitterFactor) {
      this.xJitterFactor = xJitterFactor * maxXJitterPercent * Size.Width;
      this.yJitterFactor = yJitterFactor * maxYJitterPercent * Size.Height;
      Repaint();
      EnforceUpdate();
    }

    public override void Render(Graphics graphics, int width, int height) {
      graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      base.Render(graphics, width, height);
    }

    protected void Repaint() {
      if (xDimension == null || yDimension == null) return;
      double maxSize = 1;
      double minSize = 1;
      if (sizeDimension != null && matrix.OrdinalVariables.Contains(sizeDimension)) {
        var sizes = matrix.Rows
          .Select(x => Convert.ToDouble(x.Get(sizeDimension)))
          .Where(size => !double.IsInfinity(size) && size != double.MaxValue && size != double.MinValue)
          .OrderBy(r => r);
        minSize = sizes.ElementAt((int)(sizes.Count() * 0.1));
        maxSize = sizes.ElementAt((int)(sizes.Count() * 0.9));
      } else {
        minSize = 1;
        maxSize = 1;
      }
      UpdateEnabled = false;
      Group.Clear();
      primitiveToMatrixRowDictionary.Clear();
      points = new Group(this);
      Group.Add(new Axis(this, 0, 0, AxisType.Both));
      UpdateViewSize(0, 0, TransformPixelToWorld(new Size(5, 0)).Width);
      foreach (VisualMatrixRow r in matrix.Rows.Where(x => x.Visible)) {
        List<double> xs = new List<double>();
        List<double> ys = new List<double>();
        List<object> actualXValues = new List<object>();
        List<object> actualYValues = new List<object>();
        int size;
        if (matrix.OrdinalVariables.Contains(xDimension) && r.Get(xDimension) != null) {
          xs.Add(Convert.ToDouble(r.Get(xDimension)) + r.XJitter * xJitterFactor);
          actualXValues.Add(r.Get(xDimension));
        } else if (matrix.CategoricalVariables.Contains(xDimension) && r.Get(xDimension) != null) {
          xs.Add(matrix.IndexOfCategoricalValue(xDimension, r.Get(xDimension)) + r.XJitter * xJitterFactor);
          actualXValues.Add(r.Get(xDimension));
        } else if (matrix.MultiDimensionalCategoricalVariables.Contains(xDimension)) {
          var path = xDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<MatrixRow<string, object>> subRows = (IEnumerable<MatrixRow<string, object>>)r.Get(path.ElementAt(0));
          foreach (MatrixRow<string, object> subRow in subRows) {
            if (subRow.Get(path.ElementAt(1)) != null) {
              xs.Add(matrix.IndexOfCategoricalValue(xDimension, subRow.Get(path.ElementAt(1))) + r.YJitter * xJitterFactor);
              actualXValues.Add(subRow.Get(path.ElementAt(1)));
            }
          }
        } else if (matrix.MultiDimensionalOrdinalVariables.Contains(xDimension)) {
          var path = xDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<MatrixRow<string, object>> subRows = (IEnumerable<MatrixRow<string, object>>)r.Get(path.ElementAt(0));
          foreach (MatrixRow<string, object> subRow in subRows) {
            if (subRow.Get(path.ElementAt(1)) != null) {
              xs.Add(Convert.ToDouble(subRow.Get(path.ElementAt(1))) + r.XJitter * xJitterFactor);
              actualXValues.Add(subRow.Get(path.ElementAt(1)));
            }
          }
        }
        if (matrix.OrdinalVariables.Contains(yDimension) && r.Get(yDimension) != null) {
          ys.Add(Convert.ToDouble(r.Get(yDimension)) + r.YJitter * yJitterFactor);
          actualYValues.Add(r.Get(yDimension));
        } else if (matrix.CategoricalVariables.Contains(yDimension) && r.Get(yDimension) != null) {
          ys.Add(matrix.IndexOfCategoricalValue(yDimension, r.Get(yDimension)) + r.YJitter * yJitterFactor);
          actualYValues.Add(r.Get(yDimension));
        } else if (matrix.MultiDimensionalCategoricalVariables.Contains(yDimension)) {
          var path = yDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<MatrixRow<string, object>> subRows = (IEnumerable<MatrixRow<string, object>>)r.Get(path.ElementAt(0));
          foreach (MatrixRow<string, object> subRow in subRows) {
            if (subRow.Get(path.ElementAt(1)) != null) {
              ys.Add(matrix.IndexOfCategoricalValue(yDimension, subRow.Get(path.ElementAt(1))) + r.YJitter * yJitterFactor);
              actualYValues.Add(subRow.Get(path.ElementAt(1)));
            }
          }
        } else if (matrix.MultiDimensionalOrdinalVariables.Contains(yDimension)) {
          var path = yDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<MatrixRow<string, object>> subRows = (IEnumerable<MatrixRow<string, object>>)r.Get(path.ElementAt(0));
          foreach (MatrixRow<string, object> subRow in subRows) {
            if (subRow.Get(path.ElementAt(1)) != null) {
              ys.Add(Convert.ToDouble(subRow.Get(path.ElementAt(1))) + r.YJitter * yJitterFactor);
              actualYValues.Add(subRow.Get(path.ElementAt(1)));
            }
          }
        }
        if (xs.Count() == 0) {
          xs.Add(double.NaN);
          actualXValues.Add("NaN");
        }
        if (ys.Count() == 0) {
          ys.Add(double.NaN);
          actualYValues.Add("NaN");
        }
        size = CalculateSize(Convert.ToDouble(r.Get(sizeDimension)), minSize, maxSize);
        Debug.Assert(xs.Count() == ys.Count() || xs.Count() == 1 || ys.Count() == 1);
        int n = Math.Max(xs.Count(), ys.Count());
        for (int i = 0; i < n; i++) {
          double x = xs[Math.Min(i, xs.Count() - 1)];
          double y = ys[Math.Min(i, ys.Count() - 1)];
          if (double.IsInfinity(x) || x == double.MaxValue || x == double.MinValue) x = double.NaN;
          if (double.IsInfinity(y) || y == double.MaxValue || y == double.MinValue) y = double.NaN;
          //if (!double.IsNaN(x) && !double.IsNaN(y) && IsReasonablePoint(new PointD(x, y))) {
            if (!double.IsNaN(x) && !double.IsNaN(y)) {
            string actualXValue = actualXValues[Math.Min(i, actualXValues.Count() - 1)].ToString();
            string actualYValue = actualYValues[Math.Min(i, actualYValues.Count() - 1)].ToString();
            UpdateViewSize(x, y, TransformPixelToWorld(new Size(size, 0)).Width);
            int alpha = CalculateAlpha(size);
            Pen pen = new Pen(Color.FromArgb(alpha, r.Selected ? selectionColor : defaultColor));
            Brush brush = pen.Brush;
            FixedSizeCircle c = new FixedSizeCircle(this, x, y, size, pen, brush);
            c.ToolTipText = xDimension + " = " + actualXValue + Environment.NewLine +
              yDimension + " = " + actualYValue + Environment.NewLine +
              r.GetToolTipText();
            points.Add(c);
            if (!r.Selected) c.IntoBackground();
            primitiveToMatrixRowDictionary[c] = r;
          }
        }
      }
      Group.Add(points);
      UpdateEnabled = true;
    }

    //Mk 15.09.09 15:00 commented because it is not necessary any more and causes troubles with repainting
    //private bool IsReasonablePoint(PointD pointD) {
    //  return pointD.X > LowerLeft.X && pointD.X < UpperRight.X && pointD.Y > LowerLeft.Y && pointD.Y < UpperRight.Y;
    //}

    private int CalculateSize(double size, double minSize, double maxSize) {
      if (double.IsNaN(size) || double.IsInfinity(size) || size == double.MaxValue || size == double.MinValue) return minBubbleSize;
      if (size > maxSize) size = maxSize;
      if (size < minSize) size = minSize;
      if (Math.Abs(maxSize - minSize) < 1.0E-10) return minBubbleSize;
      double sizeDifference = ((size - minSize) / (maxSize - minSize) * (maxBubbleSize - minBubbleSize));
      if (invertSize) return maxBubbleSize - (int)sizeDifference;
      else return minBubbleSize + (int)sizeDifference;
    }

    private int CalculateAlpha(int size) {
      return (minAlpha + maxAlpha) / 2;
    }

    private void ZoomToViewSize() {
      // enlarge view by 5% on each side
      //if (minX < maxX && minY < maxY) {
      //  double width = maxX - minX;
      //  minX = minX - width * 0.05;
      //  maxX = maxX + width * 0.05;
      //}

      //if(minY < maxY)
      //{
      //  double height = maxY - minY;
      //  minY = minY - height * 0.05;
      //  maxY = maxY + height * 0.05;
      //}
      if (double.IsInfinity(minX) || double.IsNaN(minX) ||
        double.IsInfinity(maxX) || double.IsNaN(maxX) ||
        double.IsInfinity(minY) || double.IsNaN(minY) ||
        double.IsInfinity(maxY) || double.IsNaN(maxY))
        return;

      double height = maxY - minY;
      double width = maxX - minX;
      if (height < 10)
        height = 10;
      if (width < 10)
        width = 10;

      minY = minY - height * 0.05;
      maxY = maxY + height * 0.05;
      minX = minX - width * 0.05;
      maxX = maxX + width * 0.05;
      ZoomIn(minX, minY, maxX, maxY);
    }

    private void UpdateViewSize(double x, double y, double size) {
      if (x - size < minX) minX = x - size;
      if (x + size > maxX) maxX = x + size;
      if (y - size < minY) minY = y + size;
      if (y + size > maxY) maxY = y + size;
    }

    private void ResetViewSize() {
      minX = double.PositiveInfinity;
      maxX = double.NegativeInfinity;
      minY = double.PositiveInfinity;
      maxY = double.NegativeInfinity;
    }

    protected VisualMatrixRow GetMatrixRow(Point point) {
      VisualMatrixRow r = null;
      IPrimitive p = points.GetPrimitive(TransformPixelToWorld(point));
      if (p != null) {
        primitiveToMatrixRowDictionary.TryGetValue(p, out r);
      }
      return r;
    }

    public virtual void ToggleSelected() {
      foreach (VisualMatrixRow row in matrix.Rows) {
        row.ToggleSelected();
      }
      matrix.FireChanged();
    }

    public virtual void ClearSelection() {
      foreach (VisualMatrixRow entry in matrix.Rows.Where(x => x.Selected)) {
        entry.ToggleSelected();
      }
      matrix.FireChanged();
    }

    public virtual void ApplyFilter(Func<VisualMatrixRow, bool> filterPred) {
      foreach (VisualMatrixRow r in matrix.Rows) {
        if (filterPred(r)) {
          r.Visible = false;
          r.Selected = false;
        }
      }
      matrix.FireChanged();
    }

    public virtual void ClearFilter() {
      foreach (VisualMatrixRow r in matrix.Rows.Where(x => !x.Visible)) {
        r.Visible = true;
      }
      matrix.FireChanged();
    }
  }
}
