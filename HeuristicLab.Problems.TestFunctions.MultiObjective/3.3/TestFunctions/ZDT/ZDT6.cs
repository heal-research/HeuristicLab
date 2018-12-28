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
  [Item("ZDT6", "ZDT6 function as defined in http://www.tik.ee.ethz.ch/sop/download/supplementary/testproblems/ [30.11.2015]")]
  [StorableType("7F442B29-9A07-487E-BAAB-CBE2D683E421")]
  public class ZDT6 : ZDT {
    protected override double GetBestKnownHypervolume(int objectives) {
      return 117.51857519692037009;            //presumed typo on the ETH-homepage (119.518... is listed there but this doesnot match values for any given Pareto front
    }

    [StorableConstructor]
    protected ZDT6(StorableConstructorFlag _) : base(_) { }
    protected ZDT6(ZDT6 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZDT6(this, cloner);
    }
    public ZDT6() : base() { }

    public override double[] Evaluate(RealVector r) {
      double g = 0;
      for (int i = 1; i < r.Length; i++) g += r[i];
      g = 1.0 + 9.0 * Math.Pow(g / (r.Length - 1), 0.25);
      double f1 = 1 - Math.Exp(-4 * r[0]) * Math.Pow(Math.Sin(6 * Math.PI * r[0]), 6);
      double d = f1 / g;
      double f2 = g * (1.0 - d * d);
      return new double[] { f1, f2 };
    }
  }
}
