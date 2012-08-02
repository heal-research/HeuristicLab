#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanConst", Description = "Constant mean function for Gaussian processes.")]
  public class MeanConst : Item, IMeanFunction {
    [Storable]
    private double c;
    [Storable]
    private int n;
    public int GetNumberOfParameters(int numberOfVariables) {
      return 1;
    }
    [StorableConstructor]
    protected MeanConst(bool deserializing) : base(deserializing) { }
    protected MeanConst(MeanConst original, Cloner cloner)
      : base(original, cloner) {
      this.c = original.c;
      this.n = original.n;
    }
    public MeanConst()
      : base() {
    }

    public void SetParameter(double[] hyp, double[,] x) {
      if (hyp.Length != 1) throw new ArgumentException("Only one hyper-parameter allowed for constant mean function.", "hyp");
      this.c = hyp[0];
      this.n = x.GetLength(0);
    }

    public double[] GetMean(double[,] x) {
      return Enumerable.Repeat(c, n).ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      if (k > 0) throw new ArgumentException();
      return Enumerable.Repeat(1.0, n).ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanConst(this, cloner);
    }
  }
}
