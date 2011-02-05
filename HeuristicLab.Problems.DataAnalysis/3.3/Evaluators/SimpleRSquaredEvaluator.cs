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
  public class SimpleRSquaredEvaluator : SimpleEvaluator {
    public ILookupParameter<DoubleValue> RSquaredParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["RSquared"]; }
    }
    [StorableConstructor]
    protected SimpleRSquaredEvaluator(bool deserializing) : base(deserializing) { }
    protected SimpleRSquaredEvaluator(SimpleRSquaredEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleRSquaredEvaluator(this, cloner);
    }
    public SimpleRSquaredEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("RSquared", "The squared Pearson's Product Moment Correlation (R²) of estimated values and original values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      RSquaredParameter.ActualValue = new DoubleValue(Calculate(original, estimated));
    }


    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      var onlinePearsonRSquaredEvaluator = new OnlinePearsonsRSquaredEvaluator();
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        onlinePearsonRSquaredEvaluator.Add(o, e);
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        return onlinePearsonRSquaredEvaluator.RSquared;
      }
    }
  }
}
