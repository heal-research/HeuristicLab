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
  [Item("Fonseca", "Fonseca and Flemming function from // https://en.wikipedia.org/wiki/Test_functions_for_optimization [30.11.2015]")]
  [StorableClass]
  public class Fonseca : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -4, 4 } };
    }

    protected override bool[] GetMaximization(int objecitves) {
      return new bool[2];
    }

    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      return ParetoFrontStore.GetParetoFront("Misc.ParetoFronts." + this.ItemName);
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return Hypervolume.Calculate(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    protected override double[] GetReferencePoint(int objectives) {
      return new double[] { 11, 11 };
    }

    [StorableConstructor]
    protected Fonseca(bool deserializing) : base(deserializing) { }
    protected Fonseca(Fonseca original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Fonseca(this, cloner);
    }
    public Fonseca() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 1, maximumSolutionLength: int.MaxValue) { }


    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("The Fonseca problem must always have 2 objectives");
      double f0 = 0.0, aux = 1.0 / Math.Sqrt(r.Length);

      //objective1
      for (int i = 0; i < r.Length; i++) {
        double d = r[i] - aux;
        f0 += d * d;
      }
      f0 = 1 - Math.Exp(-f0);

      //objective2
      double f1 = 0.0;
      for (int i = 0; i < r.Length; i++) {
        double d = r[i] + aux;
        f1 += d * d;
      }
      f1 = 1 - Math.Exp(-f1);

      double[] res = { f0, f1 };
      return res;
    }

  }
}
