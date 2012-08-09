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
  [Item(Name = "CovariancePeriodic", Description = "Periodic covariance function for Gaussian processes.")]
  public class CovariancePeriodic : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;
    [Storable]
    private double sf2;
    [Storable]
    private double l;
    [Storable]
    private double p;

    private bool symmetric;

    private double[,] sd;
    public int GetNumberOfParameters(int numberOfVariables) {
      return 3;
    }
    [StorableConstructor]
    protected CovariancePeriodic(bool deserializing) : base(deserializing) { }
    protected CovariancePeriodic(CovariancePeriodic original, Cloner cloner)
      : base(original, cloner) {
      if (original.x != null) {
        x = new double[original.x.GetLength(0), original.x.GetLength(1)];
        Array.Copy(original.x, x, x.Length);
        xt = new double[original.xt.GetLength(0), original.xt.GetLength(1)];
        Array.Copy(original.xt, xt, xt.Length);
      }
      sf2 = original.sf2;
      l = original.l;
      p = original.p;
      symmetric = original.symmetric;
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

      sf2 = Math.Min(10E6, sf2); // upper limit for the scale

      sd = null;
    }
    public void SetData(double[,] x) {
      SetData(x, x);
      this.symmetric = true;
    }

    public void SetData(double[,] x, double[,] xt) {
      this.x = x;
      this.xt = xt;
      this.symmetric = false;

      sd = null;
    }

    public double GetCovariance(int i, int j) {
      if (sd == null) CalculateSquaredDistances();
      double k = sd[i, j];
      k = Math.PI * k / p;
      k = Math.Sin(k) / l;
      k = k * k;

      return sf2 * Math.Exp(-2.0 * k);
    }

    public double GetGradient(int i, int j, int k) {
      double v = Math.PI * sd[i, j] / p;
      switch (k) {
        case 0: {
            double newK = Math.Sin(v) / l;
            newK = newK * newK;
            return 4 * sf2 * Math.Exp(-2 * newK) * newK;
          }
        case 1: {
            double r = Math.Sin(v) / l;
            return 4 * sf2 / l * Math.Exp(-2 * r * r) * r * Math.Cos(v) * v;
          }
        case 2: {
            double newK = Math.Sin(v) / l;
            newK = newK * newK;
            return 2 * sf2 * Math.Exp(-2 * newK);

          }
        default: {
            throw new ArgumentException("CovariancePeriodic only has three hyperparameters.", "k");
          }
      }
    }

    private void CalculateSquaredDistances() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      sd = new double[rows, cols];

      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < cols; j++) {
            sd[i, j] = Math.Sqrt(Util.SqrDist(Util.GetRow(x, i), Util.GetRow(x, j)));
            sd[j, i] = sd[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            sd[i, j] = Math.Sqrt(Util.SqrDist(Util.GetRow(x, i), Util.GetRow(xt, j)));
          }
        }
      }
    }
  }
}
