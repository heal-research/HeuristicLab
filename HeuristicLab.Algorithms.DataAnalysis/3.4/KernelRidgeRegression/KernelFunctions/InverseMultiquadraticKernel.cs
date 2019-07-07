#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("891AC2D7-520C-4D34-9CBF-174DFA290C12")]
  [Item("InverseMultiquadraticKernel", "A kernel function that uses the inverse multi-quadratic function  1 / sqrt(1+||x-c||²/beta²). Similar to http://crsouza.com/2010/03/17/kernel-functions-for-machine-learning-applications/ with beta as a scaling factor.")]
  public class InverseMultiquadraticKernel : KernelBase {
    private const double C = 1.0;

    [StorableConstructor]
    protected InverseMultiquadraticKernel(StorableConstructorFlag _) : base(_) { }

    protected InverseMultiquadraticKernel(InverseMultiquadraticKernel original, Cloner cloner) : base(original, cloner) { }

    public InverseMultiquadraticKernel() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InverseMultiquadraticKernel(this, cloner);
    }

    protected override double Get(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return 1 / Math.Sqrt(C + d * d);
    }

    //n²/(b³(n²/b² + C)^1.5)
    protected override double GetGradient(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance gradient while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return d * d / (beta * Math.Pow(d * d + C, 1.5));
    }
  }
}
