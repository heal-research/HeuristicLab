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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovariancePeriodic", Description = "Periodic covariance function for Gaussian processes.")]
  public class CovariancePeriodic : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double l;
    public double Length { get { return l; } }
    [Storable]
    private double p;
    public double Period { get { return p; } }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 3;
    }
    [StorableConstructor]
    protected CovariancePeriodic(bool deserializing) : base(deserializing) { }
    protected CovariancePeriodic(CovariancePeriodic original, Cloner cloner)
      : base(original, cloner) {
      sf2 = original.sf2;
      l = original.l;
      p = original.p;
    }
    public CovariancePeriodic()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovariancePeriodic(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length != 3) throw new ArgumentException();
      this.l = Math.Exp(hyp[0]);
      this.p = Math.Exp(hyp[1]);
      this.sf2 = Math.Exp(2 * hyp[2]);
    }

    public double GetCovariance(double[,] x, int i, int j) {
      double k = i == j ? 0.0 : GetDistance(x, x, i, j);
      k = Math.PI * k / p;
      k = Math.Sin(k) / l;
      k = k * k;

      return sf2 * Math.Exp(-2.0 * k);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double v = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j) / p;
      double gradient = Math.Sin(v) / l;
      gradient *= gradient;
      yield return 4.0 * sf2 * Math.Exp(-2.0 * gradient) * gradient;
      double r = Math.Sin(v) / l;
      yield return 4.0 * sf2 / l * Math.Exp(-2 * r * r) * r * Math.Cos(v) * v;
      yield return 2.0 * sf2 * Math.Exp(-2 * gradient);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double k = GetDistance(x, xt, i, j);
      k = Math.PI * k / p;
      k = Math.Sin(k) / l;
      k = k * k;

      return sf2 * Math.Exp(-2.0 * k);
    }

    private double GetDistance(double[,] x, double[,] xt, int i, int j) {
      return Math.Sqrt(Util.SqrDist(Util.GetRow(x, i), Util.GetRow(xt, j)));
    }
  }
}
