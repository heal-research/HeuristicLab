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
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("DTLZ5", "Testfunction as defined as DTLZ5 in http://repository.ias.ac.in/81671/ [30.11.15]")]
  [StorableType("CEFD068E-521B-459F-80F1-B2AE4D114D41")]
  public class DTLZ5 : DTLZ {

    [StorableConstructor]
    protected DTLZ5(StorableConstructorFlag _) : base(_) { }
    protected DTLZ5(DTLZ5 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DTLZ5(this, cloner);
    }
    public DTLZ5() : base() { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < objectives) {
        throw new ArgumentException("The dimensionality of the problem(ProblemSize) must be larger than or equal to the number of objectives");
      }
      double[] res = new double[objectives];

      //calculate g(Xm)
      double g = 0;
      for (int i = objectives; i < r.Length; i++) {
        double d = r[i] - 0.5;
        g += d * d;
      }

      //phi definition
      Func<double, double> phi;
      phi = (double x) => { return Math.PI / (4 * (1 + g)) * (1 + 2 * g * x); };

      //calculating f0...fM-1
      for (int i = 0; i < objectives; i++) {
        double f = i == 0 ? 1 : (Math.Sin(phi(r[objectives - i - 1]) * Math.PI / 2)) * (1 + g);
        for (int j = 0; j < objectives - i - 1; j++) {
          f *= Math.Cos(phi(r[j]) * Math.PI / 2);
        }
        res[i] = f;
      }
      return res;
    }
  }
}
