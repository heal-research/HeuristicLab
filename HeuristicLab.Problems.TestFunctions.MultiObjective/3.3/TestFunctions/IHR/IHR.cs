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
using HeuristicLab.Common;
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("3DBC1750-FDEA-454C-AF65-EA4337CEE823")]
  public abstract class IHR : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -1, 1 } };
    }

    protected override bool[] GetMaximization(int objectives) {
      return new bool[objectives];
    }

    protected override double[] GetReferencePoint(int objectives) {
      double[] rp = new double[objectives];
      for (int i = 0; i < objectives; i++) {
        rp[i] = 11;
      }
      return rp;
    }

    [StorableConstructor]
    protected IHR(StorableConstructorFlag _) : base(_) { }
    protected IHR(IHR original, Cloner cloner) : base(original, cloner) { }
    public IHR() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 2, maximumSolutionLength: int.MaxValue) { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < objectives) {
        throw new ArgumentException("The dimensionality of the problem(ProblemSize) must be larger than or equal to the number of objectives");
      }
      return new double[] { F1(r), F2(r) };
    }
    protected abstract double F1(RealVector y);
    protected abstract double F2(RealVector y);
    protected abstract double G(RealVector y);


    protected double H(double x, RealVector r) {
      return 1.0 / (1 + Math.Exp(-x / Math.Sqrt(r.Length)));
    }
    protected double HF(double x, RealVector r) {
      double ymax = 1;
      return Math.Abs(r[0]) <= ymax ? x : 1 + Math.Abs(r[0]);
    }
    protected double HG(double x) {
      return (x * x) / (Math.Abs(x) + 0.1);
    }
  }
}
