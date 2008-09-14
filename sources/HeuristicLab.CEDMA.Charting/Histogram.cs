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
        r.OnSelectionChanged += new EventHandler(Record_OnSelectionChanged);
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
        e.Record.OnSelectionChanged += new EventHandler(Record_OnSelectionChanged);
        records.Add(e.Record);
      }
    }

    void Record_OnSelectionChanged(object sender, EventArgs e) {
      Record r = (Record)sender;
      IPrimitive primitive;
      recordToPrimitiveDictionary.TryGetValue(r, out primitive);
      if(primitive != null) {
        ((FixedSizeCircle)primitive).UpdateEnabled = false;
        bars.UpdateEnabled = false;
        if(r.Selected) {
          int alpha = primitive.Pen.Color.A;
          primitive.Pen.Color = Color.FromArgb(alpha, selectionColor);
          primitive.Brush = primitive.Pen.Brush;
          primitive.IntoForeground();
        } else {
          int alpha = primitive.Pen.Color.A;
          primitive.Pen.Color = Color.FromArgb(alpha, defaultColor);
          primitive.Brush = primitive.Pen.Brush;
          primitive.IntoBackground();
        }
        ((FixedSizeCircle)primitive).UpdateEnabled = true;
        bars.UpdateEnabled = true;
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
      if(dimension == null) return;
      UpdateEnabled = false;
      Group.Clear();
      primitiveToRecordsDictionary.Clear();
      recordToPrimitiveDictionary.Clear();
      bars = new Group(this);
      Group.Add(new Axis(this, 0, 0, AxisType.Both));
      UpdateViewSize(0, 0);
      var values = records.Select(r => r.Get(dimension)).Where(
        x => !double.IsNaN(x) && !double.IsInfinity(x) && x != double.MinValue && x != double.MaxValue).OrderBy(x => x);
      IEnumerable<IGrouping<double,double>> frequencies;
      double bucketSize;
      if(dimension == Record.TARGET_VARIABLE || dimension == Record.TREE_HEIGHT || dimension == Record.TREE_SIZE) {
        frequencies = values.GroupBy(x => x);
        bucketSize = 1.0;
      } else {
        double min = values.ElementAt((int)(values.Count() * 0.05));
        double max = values.ElementAt((int)(values.Count() * 0.95));
        bucketSize = (max - min) / N_BUCKETS;
        frequencies = values.GroupBy(x => Math.Min(Math.Max(min, Math.Floor((x - min) / bucketSize) * bucketSize + min), max));
      }
      Pen defaultPen = new Pen(defaultColor);
      Brush defaultBrush = defaultPen.Brush;
      foreach(IGrouping<double, double> g in frequencies) {
        double freq = g.Count();
        double lower = g.Key;
        double upper = g.Key+bucketSize;
        HeuristicLab.Charting.Rectangle bar = new HeuristicLab.Charting.Rectangle(this, lower, 0, upper, freq, defaultPen, defaultBrush);
        primitiveToRecordsDictionary[bar] = records;
        if(lower == frequencies.First().Key) bar.ToolTipText = " x < "+upper+" : "+freq;
        else if(lower ==frequencies.Last().Key) bar.ToolTipText = "x >= "+lower+" : "+freq;
        else bar.ToolTipText = "x in ["+lower+" .. "+upper+"[ : "+freq;
        bars.Add(bar);
        UpdateViewSize(lower, freq);
        UpdateViewSize(upper, freq);
      }
      Group.Add(bars);
      UpdateEnabled = true;
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
      if(freq  > maxFrequency) maxFrequency = freq;
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
        List<Record> rs = GetRecords(point);
        if(rs != null) rs.ForEach(r => r.ToggleSelected());
        results.FireChanged();
      } else {
        base.MouseClick(point, button);
      }
    }
  }
}
