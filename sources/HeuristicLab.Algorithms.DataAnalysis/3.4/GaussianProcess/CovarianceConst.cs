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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceConst",
    Description = "Constant covariance function for Gaussian processes.")]
  public class CovarianceConst : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }

    [StorableConstructor]
    protected CovarianceConst(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceConst(CovarianceConst original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
    }

    public CovarianceConst()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceConst(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 1;
    }

    public void SetParameter(double[] hyp) {
      this.sf2 = Math.Exp(2 * hyp[0]);
    }
    public void SetData(double[,] x) {
      // nothing to do
    }


    public void SetData(double[,] x, double[,] xt) {
      // nothing to do
    }

    public double GetCovariance(int i, int j) {
      return sf2;
    }

    public double GetGradient(int i, int j, int k) {
      if (k != 0) throw new ArgumentException("CovarianceConst has only one hyperparameters", "k");
      return 2 * sf2;
    }
  }
}
