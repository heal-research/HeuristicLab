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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("IHR4", "Testfunction as defined as IHR4 in \"Igel, C., Hansen, N., & Roth, S. (2007). Covariance matrix adaptation for multi-objective optimization. Evolutionary computation, 15(1), 1-28.\" [24.06.16]")]
  [StorableType("E83A9E6E-F7B3-4B27-B5CA-A323D89844F5")]
  public class IHR4 : IHR {
    protected override IList<double[]> GetOptimalParetoFront(int objectives) {
      List<double[]> res = new List<double[]>();
      for (int i = 0; i <= 500; i++) {
        RealVector r = new RealVector(objectives);
        r[0] = 1 / 500.0 * i;
        res.Add(this.Evaluate(r, objectives));
      }
      return res;
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return HypervolumeCalculator.CalculateHypervolume(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -5, 5 } };
    }

    [StorableConstructor]
    protected IHR4(StorableConstructorFlag _) : base(_) { }
    protected IHR4(IHR4 original, Cloner cloner) : base(original, cloner) { }
    public IHR4() : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IHR4(this, cloner);
    }

    protected override double F1(RealVector y) {
      return Math.Abs(y[0]);
    }

    protected override double F2(RealVector y) {
      var g = G(y);
      var d = H(y[0], y) / g;
      return g * HF(1 - Math.Sqrt(d), y);
    }

    protected override double G(RealVector y) {
      double sum = 0.0;
      for (int i = 1; i < y.Length; i++) {
        sum += y[i] * y[i] - 10 * Math.Cos(4 * Math.PI * y[i]);
      }
      return 1 + 10 * (y.Length - 1) + sum;
    }
  }
}
