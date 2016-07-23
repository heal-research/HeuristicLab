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
  [Item("ZDT4", "ZDT4 function as defined in http://www.tik.ee.ethz.ch/sop/download/supplementary/testproblems/ [30.11.2015]")]
  [StorableClass]
  public class ZDT4 : ZDT {
    protected override double[,] GetBounds(int objectives) {
      double[,] bounds = new double[objectives, 2];
      bounds[0, 0] = 0; bounds[0, 1] = 1;
      for (int i = 1; i < objectives; i++) {
        bounds[i, 0] = -5;
        bounds[i, 1] = 5;
      }
      return bounds;
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return 120 + 2.0 / 3;
    }

    [StorableConstructor]
    protected ZDT4(bool deserializing) : base(deserializing) { }
    protected ZDT4(ZDT4 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZDT4(this, cloner);
    }
    public ZDT4() : base() { }

    public override double[] Evaluate(RealVector r) {
      double g = 0;
      for (int i = 1; i < r.Length; i++) {
        double v = r[i];
        g += v * v - 10 * Math.Cos(4 * Math.PI * v);
      }
      g = 1.0 + 10.0 * (r.Length - 1) + g;
      double d = r[0] / g;
      double f0 = r[0];
      double f1 = 1 - Math.Sqrt(d);
      return new double[] { f0, f1 };
    }
  }
}
