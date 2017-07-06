#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("GaussianKernel", "A kernel function that uses Gaussian function exp(-n²/beta²). As described in http://crsouza.com/2010/03/17/kernel-functions-for-machine-learning-applications/")]
  public class GaussianKernel : KernelBase {
    [StorableConstructor]
    protected GaussianKernel(bool deserializing) : base(deserializing) { }

    protected GaussianKernel(GaussianKernel original, Cloner cloner) : base(original, cloner) { }

    public GaussianKernel() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianKernel(this, cloner);
    }

    protected override double Get(double norm) {
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return Math.Exp(-d * d);
    }

    //2 * n²/b²* 1/b * exp(-n²/b²)
    protected override double GetGradient(double norm) {
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return 2 * d * d / beta * Math.Exp(-d * d);
    }
  }
}
