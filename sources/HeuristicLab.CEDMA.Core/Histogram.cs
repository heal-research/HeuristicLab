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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class Histogram {
    private int buckets;
    public int Buckets {
      get { return buckets; }
      set {
        buckets = value;
        ResetBucketBounds();
      }
    }

    private double[] limit;

    private List<double> values;

    public Histogram(int buckets) {
      this.buckets = buckets;
      values = new List<double>();
    }

    public double LowerValue(int bucketIndex) {
      return limit[bucketIndex];
    }

    public double UpperValue(int bucketIndex) {
      return limit[bucketIndex+1];
    }

    public int Frequency(int bucketIndex) {
      return values.Count(x => x >= LowerValue(bucketIndex) && x < UpperValue(bucketIndex));
    }

    public void AddValues(IEnumerable<double> xs) {
      values.AddRange(xs.Where(x=>
        !double.IsInfinity(x) && !double.IsNaN(x) && double.MaxValue!=x && double.MinValue !=x));
      ResetBucketBounds();
    }

    private void ResetBucketBounds() {
      limit = new double[buckets+1];
      values.Sort();
      double min = values[10];
      double max = values[values.Count-10];

      double step = (max - min) / buckets;
      double cur = min;
      for(int i = 0; i < buckets+1; i++) {
        limit[i] = cur;
        cur += step;
      }
    }
  }
}
