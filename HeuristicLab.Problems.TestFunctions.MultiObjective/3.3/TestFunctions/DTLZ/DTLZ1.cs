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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("DTLZ1", "Testfunction as defined as DTLZ1 in http://repository.ias.ac.in/81671/ [30.11.15]")]
  [StorableClass]
  public class DTLZ1 : DTLZ {
    protected override double GetBestKnownHypervolume(int objectives) {
      if (objectives == 2) return 120 + 7.0 / 8;
      return -1;
    }

    [StorableConstructor]
    protected DTLZ1(bool deserializing) : base(deserializing) { }
    protected DTLZ1(DTLZ1 original, Cloner cloner) : base(original, cloner) { }
    public DTLZ1() : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DTLZ1(this, cloner);
    }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < objectives) {
        throw new ArgumentException("The dimensionality of the problem(ProblemSize) must be larger than or equal to the number of objectives");
      }
      double[] res = new double[objectives];
      int k = r.Length - objectives + 1;
      double g = 0;

      for (int i = r.Length - k; i < r.Length; i++) {
        g += (r[i] - 0.5) * (r[i] - 0.5) - Math.Cos(20.0 * Math.PI * (r[i] - 0.5));
      };
      g += k;
      g *= 100;

      for (int i = 0; i < objectives; i++) {
        res[i] = 0.5 * (1.0 + g);
        for (int j = 0; j < objectives - i - 1; ++j)
          res[i] *= (r[j]);
        if (i > 0)
          res[i] *= 1 - r[objectives - i - 1];
      }
      return res;
    }
  }
}
