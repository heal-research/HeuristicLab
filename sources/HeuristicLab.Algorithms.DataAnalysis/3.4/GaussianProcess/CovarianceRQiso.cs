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
  [Item(Name = "CovarianceRQiso",
    Description = "Isotropic rational quadratic covariance function for Gaussian processes.")]
  public class CovarianceRQiso : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double l;
    public double Length { get { return l; } }
    [Storable]
    private double alpha;
    public double Shape { get { return alpha; } }

    [StorableConstructor]
    protected CovarianceRQiso(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceRQiso(CovarianceRQiso original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.l = original.l;
      this.alpha = original.alpha;
    }

    public CovarianceRQiso()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRQiso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 3;
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length != 3) throw new ArgumentException("CovarianceRQiso has three hyperparameters", "k");
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
      this.alpha = Math.Exp(hyp[2]);
    }


    public double GetCovariance(double[,] x, int i, int j) {
      double lInv = 1.0 / l;
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(x, j).Select(e => e * lInv));
      return sf2 * Math.Pow(1 + 0.5 * d / alpha, -alpha);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double lInv = 1.0 / l;
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(x, j).Select(e => e * lInv));

      double b = 1 + 0.5 * d / alpha;
      yield return sf2 * Math.Pow(b, -alpha - 1) * d;
      yield return 2 * sf2 * Math.Pow(b, -alpha);
      yield return sf2 * Math.Pow(b, -alpha) * (0.5 * d / b - alpha * Math.Log(b));
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double lInv = 1.0 / l;
      double d = Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(xt, j).Select(e => e * lInv));
      return sf2 * Math.Pow(1 + 0.5 * d / alpha, -alpha);
    }
  }
}
