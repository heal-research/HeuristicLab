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
  public class SimpleMSEEvaluator : SimpleEvaluator {

    public ILookupParameter<DoubleValue> MeanSquaredErrorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MeanSquaredError"]; }
    }

    [StorableConstructor]
    protected SimpleMSEEvaluator(bool deserializing) : base(deserializing) { }
    protected SimpleMSEEvaluator(SimpleMSEEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleMSEEvaluator(this, cloner);
    }
    public SimpleMSEEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("MeanSquaredError", "The mean squared error of estimated values."));
    }

    protected override void Apply(DoubleMatrix values) {
      MeanSquaredErrorParameter.ActualValue = new DoubleValue(Calculate(values));
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      var onlineMseEvaluator = new OnlineMeanSquaredErrorEvaluator();
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        onlineMseEvaluator.Add(o, e);
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        return onlineMseEvaluator.MeanSquaredError;
      }
    }

    public static double Calculate(DoubleMatrix values) {
      var original = from row in Enumerable.Range(0, values.Rows)
                     select values[row, ORIGINAL_INDEX];
      var estimated = from row in Enumerable.Range(0, values.Rows)
                      select values[row, ESTIMATION_INDEX];
      return Calculate(original, estimated);
    }
  }
}
