#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.NK {
  [Item("ExponentialDistributionWeightsInitializer", "Assigns exponentially decreasing weights using the rate parameter lambda.")]
  [StorableType("F67982B7-A94B-4876-977A-34DB44B40739")]
  public sealed class ExponentialDistributionWeightsInitializer : ParameterizedNamedItem, IWeightsInitializer {
    public IValueParameter<DoubleValue> LambdaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Lambda"]; }
    }

    [StorableConstructor]
    private ExponentialDistributionWeightsInitializer(StorableConstructorFlag _) : base(_) { }
    private ExponentialDistributionWeightsInitializer(ExponentialDistributionWeightsInitializer original, Cloner cloner)
      : base(original, cloner) {
    }
    public ExponentialDistributionWeightsInitializer() {
      Parameters.Add(new ValueParameter<DoubleValue>("Lambda", "The rate parameter of the exponential distribution.", new DoubleValue(1.0)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExponentialDistributionWeightsInitializer(this, cloner);
    }

    private static double f(double x, double lambda) {
      if (x < 0.0)
        return 0.0;
      return lambda * Math.Exp(-lambda * x);
    }

    public IEnumerable<double> GetWeights(int nComponents) {
      double lambda = LambdaParameter.Value.Value;
      for (int i = 0; i < nComponents; i++)
        yield return f(i, lambda);
    }
  }
}
