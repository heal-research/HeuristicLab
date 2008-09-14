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

namespace HeuristicLab.CEDMA.Charting {
  public class Histogram : Chart {
    private static readonly Color defaultColor = Color.Blue;
    private static readonly Color selectionColor = Color.Red;

    private double minX;
    private double maxX;
    private double maxFrequency;
    private const int N_BUCKETS = 50;
    private const int MAX_BUCKETS = 100;
    private List<Record> records;
    private ResultList results;
    private Dictionary<IPrimitive, List<Record>> primitiveToRecordsDictionary;
    private Dictionary<Record, IPrimitive> recordToPrimitiveDictionary;
    private Group bars;
    private double[] limits;
    private int[] buckets;
    private string dimension;

    public Histogram(ResultList results, double x1, double y1, double x2, double y2)
      : this(results, new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public Histogram(ResultList results, PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      records = new List<Record>();
      primitiveToRecordsDictionary = new Dictionary<IPrimitive, List<Record>>();
      recordToPrimitiveDictionary = new Dictionary<Record, IPrimitive>();
      this.results = results;
      foreach(Record r in results.Records) {
        records.Add(r);
      }
      results.OnRecordAdded += new EventHandler<RecordAddedEventArgs>(results_OnRecordAdded);
      results.Changed += new EventHandler(results_Changed);
      limits = new double[N_BUCKETS - 1];
      buckets = new int[N_BUCKETS];
    }

    void results_Changed(object sender, EventArgs e) {
      Repaint();
      EnforceUpdate();
    }

    void results_OnRecordAdded(object sender, RecordAddedEventArgs e) {
      lock(records) {
        records.Add(e.Record);
      }
    }

    public void ShowFrequency(string dimension) {
      if(this.dimension != dimension) {
        this.dimension = dimension;
        ResetViewSize();
        Repaint();
        ZoomToViewSize();
      }
    }

    private void Repaint() {
      lock(records) {
        if(dimension == null) return;
        UpdateEnabled = false;
        Group.Clear();
        primitiveToRecordsDictionary.Clear();
        recordToPrimitiveDictionary.Clear();
        bars = new Group(this);
        Group.Add(new Axis(this, 0, 0, AxisType.Both));
        UpdateViewSize(0, 0);
        Pen defaultPen = new Pen(defaultColor);
        Brush defaultBrush = defaultPen.Brush;
        PaintHistogram(records, defaultPen, defaultBrush);
        Pen selectionPen = new Pen(selectionColor);
        Brush selectionBrush = selectionPen.Brush;
        PaintHistogram(records.Where(r => r.Selected), selectionPen, selectionBrush);
        Group.Add(bars);
        UpdateEnabled = true;
      }
    }

    private void PaintHistogram(IEnumerable<Record> records, Pen pen, Brush brush) {
      var values = records.Select(r => new { Record = r, Value = r.Get(dimension) }).Where(
        x => !double.IsNaN(x.Value) && !double.IsInfinity(x.Value) && x.Value != double.MinValue && x.Value != double.MaxValue).OrderBy(x => x.Value);
      if(values.Count() == 0) return;
      double bucketSize = 1.0;
      var frequencies = values.GroupBy(x => x.Value);
      if(frequencies.Count() > MAX_BUCKETS) {
        double min = values.ElementAt((int)(values.Count() * 0.05)).Value;
        double max = values.ElementAt((int)(values.Count() * 0.95)).Value;
        bucketSize = (max - min) / N_BUCKETS;
        frequencies = values.GroupBy(x => Math.Min(Math.Max(min, Math.Floor((x.Value - min) / bucketSize) * bucketSize + min), max));
      }
      foreach(var g in frequencies) {
        double freq = g.Count();
        double lower = g.Key;
        double upper = g.Key + bucketSize;
        HeuristicLab.Charting.Rectangle bar = new HeuristicLab.Charting.Rectangle(this, lower, 0, upper, freq, pen, brush);
        primitiveToRecordsDictionary[bar] = g.Select(r => r.Record).ToList();
        primitiveToRecordsDictionary[bar].ForEach(x => recordToPrimitiveDictionary[x] = bar);
        if(lower == frequencies.First().Key) bar.ToolTipText = " x < " + upper + " : " + freq;
        else if(lower == frequencies.Last().Key) bar.ToolTipText = "x >= " + lower + " : " + freq;
        else bar.ToolTipText = "x in [" + lower + " .. " + upper + "[ : " + freq;
        bars.Add(bar);
        UpdateViewSize(lower, freq);
        UpdateViewSize(upper, freq);
      }
    }

    private void ZoomToViewSize() {
      if(minX < maxX) {
        // enlarge view by 5% on each side
        double width = maxX - minX;
        minX = minX - width * 0.05;
        maxX = maxX + width * 0.05;
        double minY = 0 - maxFrequency * 0.05;
        double maxY = maxFrequency + maxFrequency * 0.05;
        ZoomIn(minX, minY, maxX, maxY);
      }
    }

    private void UpdateViewSize(double x, double freq) {
      if(x < minX) minX = x;
      if(x > maxX) maxX = x;
      if(freq > maxFrequency) maxFrequency = freq;
    }

    private void ResetViewSize() {
      minX = double.PositiveInfinity;
      maxX = double.NegativeInfinity;
      maxFrequency = double.NegativeInfinity;
    }

    internal List<Record> GetRecords(Point point) {
      List<Record> records = null;
      IPrimitive p = bars.GetPrimitive(TransformPixelToWorld(point));
      if(p != null) {
        primitiveToRecordsDictionary.TryGetValue(p, out records);
      }
      return records;
    }

    public override void MouseClick(Point point, MouseButtons button) {
      if(button == MouseButtons.Left) {
        lock(records) {
          List<Record> rs = GetRecords(point);
          UpdateEnabled = false;
          if(rs != null) rs.ForEach(r => r.ToggleSelected());
          UpdateEnabled = true;
        }
        results.FireChanged();
      } else {
        base.MouseClick(point, button);
      }
    }
  }
}
