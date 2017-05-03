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

namespace HeuristicLab.Algorithms.DataAnalysis.KernelRidgeRegression {
  [StorableClass]
  [Item("InverseMultiquadraticKernel", "A kernel function that uses the inverse multi-quadratic function  1 / sqrt(1+||x-c||²/beta²). Similar to http://crsouza.com/2010/03/17/kernel-functions-for-machine-learning-applications/ with beta as a scaling factor.")]
  public class InverseMultiquadraticKernel : KernelBase {

    private const double C = 1.0;
    #region HLConstructors & Boilerplate
    [StorableConstructor]
    protected InverseMultiquadraticKernel(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }
    protected InverseMultiquadraticKernel(InverseMultiquadraticKernel original, Cloner cloner) : base(original, cloner) { }
    public InverseMultiquadraticKernel() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new InverseMultiquadraticKernel(this, cloner);
    }
    #endregion

    protected override double Get(double norm) {
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return 1 / Math.Sqrt(C + d * d);
    }

    //n²/(b³(n²/b² + C)^1.5)
    protected override double GetGradient(double norm) {
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return d * d / (beta * Math.Pow(d * d + C, 1.5));
    }
  }
}
