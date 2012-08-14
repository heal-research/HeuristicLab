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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceSEiso",
    Description = "Isotropic squared exponential covariance function for Gaussian processes.")]
  public class CovarianceSEiso : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double l;
    public double Length { get { return l; } }

    [StorableConstructor]
    protected CovarianceSEiso(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceSEiso(CovarianceSEiso original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.l = original.l;
    }

    public CovarianceSEiso()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSEiso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 2;
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length != 2) throw new ArgumentException("CovarianceSEiso has two hyperparameters", "k");
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
    }


    public double GetCovariance(double[,] x, int i, int j) {
      double lInv = 1.0 / l;
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(x, j).Select(e => e * lInv));
      return sf2 * Math.Exp(-d / 2.0);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double lInv = 1.0 / l;
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(x, j).Select(e => e * lInv));
      double g = Math.Exp(-d / 2.0);
      yield return sf2 * g * d;
      yield return 2.0 * sf2 * g;
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double lInv = 1.0 / l;
      double d = Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(xt, j).Select(e => e * lInv));
      return sf2 * Math.Exp(-d / 2.0);
    }
  }
}
