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
  public class SimpleMeanAbsolutePercentageErrorEvaluator : SimpleEvaluator {
    public ILookupParameter<PercentValue> AverageRelativeErrorParameter {
      get { return (ILookupParameter<PercentValue>)Parameters["AverageRelativeError"]; }
    }

    [StorableConstructor]
    protected SimpleMeanAbsolutePercentageErrorEvaluator(bool deserializing) : base(deserializing) { }
    protected SimpleMeanAbsolutePercentageErrorEvaluator(SimpleMeanAbsolutePercentageErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SimpleMeanAbsolutePercentageErrorEvaluator() {
      Parameters.Add(new LookupParameter<PercentValue>("AverageRelativeError", "The average relative error of estimated values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      AverageRelativeErrorParameter.ActualValue = new PercentValue(Calculate(original, estimated));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleMeanAbsolutePercentageErrorEvaluator(this, cloner);
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      OnlineMeanAbsolutePercentageErrorEvaluator onlineEvaluator = new OnlineMeanAbsolutePercentageErrorEvaluator();
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        onlineEvaluator.Add(o, e);
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        return onlineEvaluator.MeanAbsolutePercentageError;
      }
    }
  }
}
