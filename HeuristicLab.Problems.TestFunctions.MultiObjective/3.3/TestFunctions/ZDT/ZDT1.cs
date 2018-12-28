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
  [Item("ZDT1", "ZDT1 function as defined in http://www.tik.ee.ethz.ch/sop/download/supplementary/testproblems/ [30.11.2015]")]
  [StorableType("5F62D1CB-4EC5-4C89-B168-25DF1F549CCA")]
  public class ZDT1 : ZDT {
    protected override double GetBestKnownHypervolume(int objectives) {
      return 120 + 2.0 / 3;
    }

    [StorableConstructor]
    protected ZDT1(StorableConstructorFlag _) : base(_) { }
    protected ZDT1(ZDT1 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZDT1(this, cloner);
    }

    public ZDT1() : base() { }

    public override double[] Evaluate(RealVector r) {
      double g = 0;
      for (int i = 1; i < r.Length; i++) g += r[i];
      g = 1.0 + 9.0 * g / (r.Length - 1);
      double f0 = r[0];
      double f1 = g * (1.0 - Math.Sqrt(r[0] / g));
      return new double[] { f0, f1 };
    }
  }
}
