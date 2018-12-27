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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("CIGTAB", "to be aded")]
  [StorableClass]
  public class CIGTAB : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -10, 10 } };
    }

    protected override bool[] GetMaximization(int objecitves) {
      return new bool[2];
    }

    protected override double[] GetReferencePoint(int objecitves) {
      return new double[] { 11, 11 };
    }

    protected override IEnumerable<double[]> GetOptimalParetoFront(int objecitves) {
      List<double[]> res = new List<double[]>();
      for (int i = 0; i <= 500; i++) {
        RealVector r = new RealVector(2);
        r[0] = 2 / 500.0 * i;
        r[1] = 2 / 500.0 * i;
        res.Add(this.Evaluate(r, 2));
      }
      return res;
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return Hypervolume.Calculate(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    [StorableConstructor]
    protected CIGTAB(bool deserializing) : base(deserializing) { }
    protected CIGTAB(CIGTAB original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CIGTAB(this, cloner);
    }

    public CIGTAB() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 1, maximumSolutionLength: int.MaxValue) { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("The CIGTAB problem must always have 2 objectives");
      double x = r[0];
      double a = 1000;
      double sum = x * x;
      for (int i = 1; i < r.Length - 1; i++) {
        sum += a * r[i] * r[i];
      }
      sum += a * a * r[r.Length - 1] * r[r.Length - 1];

      //objective1
      double f0 = 1 / (a * a * r.Length) * sum;

      x = x - 2;
      sum = x * x;
      for (int i = 1; i < r.Length - 1; i++) {
        sum += a * (r[i] - 2) * (r[i] - 2);
      }

      sum += a * a * (r[r.Length - 1] - 2) * (r[r.Length - 1] - 2);
      //objective0
      double f1 = 1 / (a * a * r.Length) * sum;

      return new double[] { f0, f1 };
    }
  }
}
