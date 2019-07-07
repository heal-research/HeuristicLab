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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  public partial class DensityChart : UserControl {
    public DensityChart() {
      InitializeComponent();
    }

    public void UpdateChart(IList<string> data, double minimumHeight = 0.1) {
      if (data == null || !data.Any())
        return;
      UpdateChartWithBuckets(CalculateBuckets(data));
    }


    public void UpdateChart(IList<double> data, double min, double max, int numBuckets, double minimumHeight = 0.1) {
      if (data == null || numBuckets < 0 || min > max || max < min)
        return;

      UpdateChartWithBuckets(CalculateBuckets(data, numBuckets, min, max));
    }


    private void UpdateChartWithBuckets(double[] buckets) {
      // set minimum height of all non-zero buckets on 10% of maximum
      double minHeight = buckets.Max() * 0.1;
      for (int i = 0; i < buckets.Length; i++) {
        if (buckets[i] >= 1 && buckets[i] < minHeight)
          buckets[i] = minHeight;
      }

      var points = chart.Series[0].Points;
      if (points.Count == buckets.Length) {
        for (int i = 0; i < buckets.Length; i++)
          points[i].SetValueY(buckets[i]);
        chart.ChartAreas[0].RecalculateAxesScale();
        chart.Refresh();
      } else {
        chart.Series[0].Points.DataBindY(buckets);
      }
    }

    private double[] CalculateBuckets(IList<double> data, int numBuckets, double min, double max) {
      var buckets = new double[numBuckets];
      var bucketSize = (max - min) / numBuckets;
      if (bucketSize > 0.0) {
        for (int i = 0; i < data.Count; i++) {
          int bucketIndex = (int)((data[i] - min) / bucketSize);
          if (bucketIndex == numBuckets) {
            bucketIndex--;
          }
          if (bucketIndex >= 0 && bucketIndex < buckets.Length)
            buckets[bucketIndex] += 1.0;
        }
      }
      return buckets;
    }
    private double[] CalculateBuckets(IList<string> data) {
      return data.GroupBy(val => val).OrderBy(g => g.Key).Select(g => (double)g.Count()).Concat(new double[] { 0.0 }).ToArray();
    }
  }
}
