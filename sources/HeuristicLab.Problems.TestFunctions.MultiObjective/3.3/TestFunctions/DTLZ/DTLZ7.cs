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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("DTLZ7", "Testfunction as defined as DTLZ6 in http://repository.ias.ac.in/81671/ [30.11.15] NOTE: The website http://people.ee.ethz.ch/~sop/download/supplementary/testproblems/dtlz7/index.php [16.12.2015] lables this function as DTLZ7")]
  [StorableClass]
  public class DTLZ7 : DTLZ {
    protected override double GetBestKnownHypervolume(int objectives) {
      if (objectives == 2) return 116.1138716447221;
      return -1;
    }

    [StorableConstructor]
    protected DTLZ7(bool deserializing) : base(deserializing) { }
    protected DTLZ7(DTLZ7 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DTLZ7(this, cloner);
    }
    public DTLZ7() : base() { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < objectives) {
        throw new ArgumentException("The dimensionality of the problem(ProblemSize) must be larger than or equal to the number of objectives");
      }
      double[] res = new double[objectives];

      //calculate g(Xm)
      double g = 0, length = length = r.Length - objectives + 1;
      for (int i = objectives; i < r.Length; i++) {
        g += r[i];
      }
      g = 1.0 + 9.0 / length * g;
      if (length == 0) { g = 1; }

      //calculating f0...fM-2
      for (int i = 0; i < objectives - 1; i++) {
        res[i] = r[i];
      }
      //calculate fM-1
      double h = objectives;
      for (int i = 0; i < objectives - 1; i++) {
        h -= res[i] / (1 + g) * (1 + Math.Sin(3 * Math.PI * res[i]));
      }
      res[objectives - 1] = (1 + g) * h;

      return res;
    }
  }
}
