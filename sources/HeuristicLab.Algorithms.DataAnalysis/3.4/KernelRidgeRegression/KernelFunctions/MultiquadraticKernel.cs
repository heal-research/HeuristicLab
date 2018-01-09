#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  // conditionally positive definite. (need to add polynomials) see http://num.math.uni-goettingen.de/schaback/teaching/sc.pdf 
  [Item("MultiquadraticKernel", "A kernel function that uses the multi-quadratic function sqrt(1+||x-c||²/beta²). Similar to http://crsouza.com/2010/03/17/kernel-functions-for-machine-learning-applications/ with beta as a scaling factor.")]
  public class MultiquadraticKernel : KernelBase {

    private const double C = 1.0;

    [StorableConstructor]
    protected MultiquadraticKernel(bool deserializing) : base(deserializing) { }

    protected MultiquadraticKernel(MultiquadraticKernel original, Cloner cloner) : base(original, cloner) { }

    public MultiquadraticKernel() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiquadraticKernel(this, cloner);
    }

    protected override double Get(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return Math.Sqrt(C + d * d);
    }

    //-n²/(d³*sqrt(C+n²/d²))
    protected override double GetGradient(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance gradient while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return -d * d / (beta * Math.Sqrt(C + d * d));
    }
  }
}
