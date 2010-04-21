#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleNMSEEvaluator : SimpleEvaluator {

    public ILookupParameter<DoubleValue> NormalizedMeanSquaredErrorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["NormalizedMeanSquaredError"]; }
    }

    public SimpleNMSEEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("NormalizedMeanSquaredError", "The normalized mean squared error (divided by variance) of estimated values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];

      NormalizedMeanSquaredErrorParameter.ActualValue = new DoubleValue(Calculate(original, estimated));
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      double mse = SimpleMSEEvaluator.Calculate(original, estimated);
      return mse / original.Variance();
    }
  }
}
