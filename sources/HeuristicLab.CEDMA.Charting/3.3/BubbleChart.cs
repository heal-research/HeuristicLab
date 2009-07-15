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
using HeuristicLab.CEDMA.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Diagnostics;

namespace HeuristicLab.CEDMA.Charting {
  public class BubbleChart : Chart {
    private const string X_JITTER = "__X_JITTER";
    private const string Y_JITTER = "__Y_JITTER";
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
    private List<ResultsEntry> filteredEntries;
    private Results results;
    private Dictionary<IPrimitive, ResultsEntry> primitiveToEntryDictionary;
    private Random random = new Random();
    private Group points;

    public BubbleChart(Results results, PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      primitiveToEntryDictionary = new Dictionary<IPrimitive, ResultsEntry>();
      this.results = results;
      filteredEntries = new List<ResultsEntry>();

      foreach (var resultsEntry in results.GetEntries()) {
        if (resultsEntry.Get(X_JITTER) == null)
          resultsEntry.Set(X_JITTER, random.NextDouble() * 2.0 - 1.0);
        if (resultsEntry.Get(Y_JITTER) == null)
          resultsEntry.Set(Y_JITTER, random.NextDouble() * 2.0 - 1.0);
      }
      results.Changed += new EventHandler(results_Changed);
    }

    void results_Changed(object sender, EventArgs e) {
      Repaint();
      EnforceUpdate();
    }

    public BubbleChart(Results results, double x1, double y1, double x2, double y2)
      : this(results, new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void SetBubbleSizeDimension(string dimension, bool inverted) {
      this.sizeDimension = dimension;
      this.invertSize = inverted;
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

    internal void SetJitter(double xJitterFactor, double yJitterFactor) {
      this.xJitterFactor = xJitterFactor * maxXJitterPercent * Size.Width;
      this.yJitterFactor = yJitterFactor * maxYJitterPercent * Size.Height;
      Repaint();
      EnforceUpdate();
    }

    public override void Render(Graphics graphics, int width, int height) {
      graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      base.Render(graphics, width, height);
    }

    private void Repaint() {
      if (xDimension == null || yDimension == null) return;
      double maxSize = 1;
      double minSize = 1;
      if (sizeDimension != null && results.OrdinalVariables.Contains(sizeDimension)) {
        var sizes = results.GetEntries()
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
      primitiveToEntryDictionary.Clear();
      points = new Group(this);
      Group.Add(new Axis(this, 0, 0, AxisType.Both));
      UpdateViewSize(0, 0, TransformPixelToWorld(new Size(5, 0)).Width);
      foreach (ResultsEntry r in results.GetEntries().Where(x => x.Visible)) {
        List<double> xs = new List<double>();
        List<double> ys = new List<double>();
        List<object> actualXValues = new List<object>();
        List<object> actualYValues = new List<object>();
        int size;
        if (results.OrdinalVariables.Contains(xDimension) && r.Get(xDimension) != null) {
          xs.Add(Convert.ToDouble(r.Get(xDimension)) + (double)r.Get(X_JITTER) * xJitterFactor);
          actualXValues.Add(r.Get(xDimension));
        } else if (results.CategoricalVariables.Contains(xDimension) && r.Get(xDimension) != null) {
          xs.Add(results.IndexOfCategoricalValue(xDimension, r.Get(xDimension)) + (double)r.Get(X_JITTER) * xJitterFactor);
          actualXValues.Add(r.Get(xDimension));
        } else if (results.MultiDimensionalCategoricalVariables.Contains(xDimension)) {
          var path = xDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<ResultsEntry> subEntries = (IEnumerable<ResultsEntry>)r.Get(path.ElementAt(0));
          foreach (ResultsEntry subEntry in subEntries) {
            if (subEntry.Get(path.ElementAt(1)) != null) {
              xs.Add(results.IndexOfCategoricalValue(xDimension, subEntry.Get(path.ElementAt(1))) + (double)r.Get(X_JITTER) * xJitterFactor);
              actualXValues.Add(subEntry.Get(path.ElementAt(1)));
            }
          }
        } else if (results.MultiDimensionalOrdinalVariables.Contains(xDimension)) {
          var path = xDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<ResultsEntry> subEntries = (IEnumerable<ResultsEntry>)r.Get(path.ElementAt(0));
          foreach (ResultsEntry subEntry in subEntries) {
            if (subEntry.Get(path.ElementAt(1)) != null) {
              xs.Add(Convert.ToDouble(subEntry.Get(path.ElementAt(1))) + (double)r.Get(X_JITTER) * xJitterFactor);
              actualXValues.Add(subEntry.Get(path.ElementAt(1)));
            }
          }
        }
        if (results.OrdinalVariables.Contains(yDimension) && r.Get(yDimension) != null) {
          ys.Add(Convert.ToDouble(r.Get(yDimension)) + (double)r.Get(Y_JITTER) * yJitterFactor);
          actualYValues.Add(r.Get(yDimension));
        } else if (results.CategoricalVariables.Contains(yDimension) && r.Get(yDimension) != null) {
          ys.Add(results.IndexOfCategoricalValue(yDimension, r.Get(yDimension)) + (double)r.Get(Y_JITTER) * yJitterFactor);
          actualYValues.Add(r.Get(yDimension));
        } else if (results.MultiDimensionalCategoricalVariables.Contains(yDimension)) {
          var path = yDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<ResultsEntry> subEntries = (IEnumerable<ResultsEntry>)r.Get(path.ElementAt(0));
          foreach (ResultsEntry subEntry in subEntries) {
            if (subEntry.Get(path.ElementAt(1)) != null) {
              ys.Add(results.IndexOfCategoricalValue(yDimension, subEntry.Get(path.ElementAt(1))) + (double)r.Get(Y_JITTER) * yJitterFactor);
              actualYValues.Add(subEntry.Get(path.ElementAt(1)));
            }
          }
        } else if (results.MultiDimensionalOrdinalVariables.Contains(yDimension)) {
          var path = yDimension.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
          IEnumerable<ResultsEntry> subEntries = (IEnumerable<ResultsEntry>)r.Get(path.ElementAt(0));
          foreach (ResultsEntry subEntry in subEntries) {
            if (subEntry.Get(path.ElementAt(1)) != null) {
              ys.Add(Convert.ToDouble(subEntry.Get(path.ElementAt(1))) + (double)r.Get(Y_JITTER) * yJitterFactor);
              actualYValues.Add(subEntry.Get(path.ElementAt(1)));
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
          if (!double.IsNaN(x) && !double.IsNaN(y) && IsReasonablePoint(new PointD(x,y))) {
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
            primitiveToEntryDictionary[c] = r;
          }
        }
      }
      Group.Add(points);
      UpdateEnabled = true;
    }

    private bool IsReasonablePoint(PointD pointD) {
      return pointD.X > LowerLeft.X && pointD.X < UpperRight.X && pointD.Y > LowerLeft.Y && pointD.Y < UpperRight.Y; 
    }

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
      if (minX < maxX && minY < maxY) {
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

    internal ResultsEntry GetResultsEntry(Point point) {
      ResultsEntry r = null;
      IPrimitive p = points.GetPrimitive(TransformPixelToWorld(point));
      if (p != null) {
        primitiveToEntryDictionary.TryGetValue(p, out r);
      }
      return r;
    }

    public override void MouseDrag(Point start, Point end, MouseButtons button) {
      if (button == MouseButtons.Left && Mode == ChartMode.Select) {
        PointD a = TransformPixelToWorld(start);
        PointD b = TransformPixelToWorld(end);
        double minX = Math.Min(a.X, b.X);
        double minY = Math.Min(a.Y, b.Y);
        double maxX = Math.Max(a.X, b.X);
        double maxY = Math.Max(a.Y, b.Y);
        HeuristicLab.Charting.Rectangle rect = new HeuristicLab.Charting.Rectangle(this, minX, minY, maxX, maxY);

        List<IPrimitive> primitives = new List<IPrimitive>();
        primitives.AddRange(points.Primitives);

        foreach (FixedSizeCircle p in primitives) {
          if (rect.ContainsPoint(p.Point)) {
            ResultsEntry r;
            primitiveToEntryDictionary.TryGetValue(p, out r);
            if (r != null) r.ToggleSelected();
          }
        }
        if (primitives.Count() > 0) results.FireChanged();
      } else {
        base.MouseDrag(start, end, button);
      }
    }

    public override void MouseClick(Point point, MouseButtons button) {
      if (button == MouseButtons.Left) {
        ResultsEntry r = GetResultsEntry(point);
        if (r != null) {
          r.ToggleSelected();
          results.FireChanged();
        }
      } else {
        base.MouseClick(point, button);
      }
    }

    public override void MouseDoubleClick(Point point, MouseButtons button) {
      if (button == MouseButtons.Left) {
        ResultsEntry entry = GetResultsEntry(point);
        if (entry != null) {
          string serializedData = (string)entry.Get(Ontology.SerializedData.Uri.Replace(Ontology.CedmaNameSpace, ""));
          var model = (IItem)PersistenceManager.RestoreFromGZip(Convert.FromBase64String(serializedData));
          PluginManager.ControlManager.ShowControl(model.CreateView());
        }
      } else {
        base.MouseDoubleClick(point, button);
      }
    }

    internal void ToggleSelected() {
      foreach (ResultsEntry entry in results.GetEntries()) {
        entry.ToggleSelected();
      }
      results.FireChanged();
    }

    internal void ClearSelection() {
      foreach (ResultsEntry entry in results.GetEntries().Where(x=>x.Selected)) {
        entry.ToggleSelected();
      }
      results.FireChanged();
    }

    internal void ApplyFilter(Func<ResultsEntry, bool> filterPred) {
      foreach (ResultsEntry r in results.GetEntries()) {
        if (filterPred(r)) {
          r.Visible = false;
          r.Selected = false;
          filteredEntries.Add(r);
        } 
      }
      results.FireChanged();
    }

    internal void ClearFilter() {
      foreach (ResultsEntry r in filteredEntries) {
        r.Visible = true;
      }
      filteredEntries.Clear();
      results.FireChanged();
    }
  }
}
