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
  /// <summary>
  /// The Variance Accounted For (VAF) function calculates is computed as
  /// VAF(y,y') =  1 - var(y-y')/var(y)
  /// where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x.
  /// </summary>
  public class SimpleVarianceAccountedForEvaluator : SimpleEvaluator {

    public ILookupParameter<DoubleValue> VarianceAccountedForParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["VarianceAccountedFor"]; }
    }

    [StorableConstructor]
    protected SimpleVarianceAccountedForEvaluator(bool deserializing) : base(deserializing) { }
    protected SimpleVarianceAccountedForEvaluator(SimpleVarianceAccountedForEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleVarianceAccountedForEvaluator(this, cloner);
    }
    public SimpleVarianceAccountedForEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("VarianceAccountedFor", "The variance of the original values accounted for by the estimated values (VAF(y,y') = 1 - var(y-y') / var(y) )."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      VarianceAccountedForParameter.ActualValue = new DoubleValue(Calculate(original, estimated));
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      var errors = new List<double>();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        if (!double.IsNaN(e) && !double.IsInfinity(e) &&
          !double.IsNaN(o) && !double.IsInfinity(o)) {
          errors.Add(o - e);
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      }

      double errorsVariance = errors.Variance();
      double originalsVariance = original.Variance();
      if (originalsVariance.IsAlmost(0.0))
        if (errorsVariance.IsAlmost(0.0)) {
          return 1.0;
        } else {
          return double.MaxValue;
        } else {
        return 1.0 - errorsVariance / originalsVariance;
      }
    }
  }
}
