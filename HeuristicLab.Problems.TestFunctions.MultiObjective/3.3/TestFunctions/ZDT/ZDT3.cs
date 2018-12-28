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
  [Item("ZDT3", "ZDT3 function as defined in http://www.tik.ee.ethz.ch/sop/download/supplementary/testproblems/ [30.11.2015]")]
  [StorableType("89C250F4-DC30-4852-AE6E-E3936569A2BA")]
  public class ZDT3 : ZDT {
    protected override double GetBestKnownHypervolume(int objectives) {
      return 128.77811613069076060;
    }

    [StorableConstructor]
    protected ZDT3(StorableConstructorFlag _) : base(_) { }
    protected ZDT3(ZDT3 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZDT3(this, cloner);
    }

    public ZDT3() : base() { }

    public override double[] Evaluate(RealVector r) {
      double g = 0;
      for (int i = 1; i < r.Length; i++) g += r[i];
      g = 1.0 + 9.0 * g / (r.Length - 1);
      double d = r[0] / g;
      double f0 = r[0];
      double f1 = g * (1.0 - Math.Sqrt(d) - d * Math.Sin(10 * Math.PI * r[0]));
      return new double[] { f0, f1 };
    }
  }
}
