#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleMeanAbsolutePercentageOfRangeErrorEvaluator : SimpleEvaluator {

    public ILookupParameter<PercentValue> AveragePercentageOfRangeErrorParameter {
      get { return (ILookupParameter<PercentValue>)Parameters["AveragePercentageOfRangeError"]; }
    }

    [StorableConstructor]
    protected SimpleMeanAbsolutePercentageOfRangeErrorEvaluator(bool deserializing) : base(deserializing) { }
    protected SimpleMeanAbsolutePercentageOfRangeErrorEvaluator(SimpleMeanAbsolutePercentageOfRangeErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SimpleMeanAbsolutePercentageOfRangeErrorEvaluator() {
      Parameters.Add(new LookupParameter<PercentValue>("AveragePercentageOfRangeError", "The average relative (percentage of range) error of estimated values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      AveragePercentageOfRangeErrorParameter.ActualValue = new PercentValue(Calculate(original, estimated));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator(this, cloner);
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      double errorsSum = 0.0;
      int n = 0;
      IList<double> originalList = original as IList<double>;
      if (originalList == null) originalList = original.ToList();

      double range = originalList.Max() - originalList.Min();
      if (double.IsInfinity(range)) return double.MaxValue;
      if (range.IsAlmost(0.0)) return double.MaxValue;


      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;

        if (!double.IsNaN(e) && !double.IsInfinity(e) &&
          !double.IsNaN(o) && !double.IsInfinity(o) && !o.IsAlmost(0.0)) {
          double percent_error = Math.Abs((e - o) / range);
          errorsSum += percent_error;
          n++;
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else if (n == 0) {
        return double.MaxValue;
      } else {
        return errorsSum / n;
      }
    }
  }
}
