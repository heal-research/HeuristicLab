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
  [Item(Name = "CovarianceRQiso",
    Description = "Isotropic rational quadratic covariance function for Gaussian processes.")]
  public class CovarianceRQiso : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double l;
    public double Length { get { return l; } }
    [Storable]
    private double alpha;
    public double Shape { get { return alpha; } }
    [Storable]
    private bool symmetric;
    private double[,] d2;

    [StorableConstructor]
    protected CovarianceRQiso(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceRQiso(CovarianceRQiso original, Cloner cloner)
      : base(original, cloner) {
      if (original.x != null) {
        this.x = new double[original.x.GetLength(0), original.x.GetLength(1)];
        Array.Copy(original.x, this.x, x.Length);

        this.xt = new double[original.xt.GetLength(0), original.xt.GetLength(1)];
        Array.Copy(original.xt, this.xt, xt.Length);

        this.d2 = new double[original.d2.GetLength(0), original.d2.GetLength(1)];
        Array.Copy(original.d2, this.d2, d2.Length);
        this.sf2 = original.sf2;
      }
      this.sf2 = original.sf2;
      this.l = original.l;
      this.alpha = original.alpha;
      this.symmetric = original.symmetric;
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
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
      this.alpha = Math.Exp(hyp[2]);
      d2 = null;
    }
    public void SetData(double[,] x) {
      SetData(x, x);
      this.symmetric = true;
    }


    public void SetData(double[,] x, double[,] xt) {
      this.symmetric = false;
      this.x = x;
      this.xt = xt;
      d2 = null;
    }

    public double GetCovariance(int i, int j) {
      if (d2 == null) CalculateSquaredDistances();
      return sf2 * Math.Pow(1 + 0.5 * d2[i, j] / alpha, -alpha);
    }

    public double GetGradient(int i, int j, int k) {
      switch (k) {
        case 0: return sf2 * Math.Pow(1 + 0.5 * d2[i, j] / alpha, -alpha - 1) * d2[i, j];
        case 1: return 2 * sf2 * Math.Pow((1 + 0.5 * d2[i, j] / alpha), (-alpha));
        case 2: {
            double g = (1 + 0.5 * d2[i, j] / alpha);
            g = sf2 * Math.Pow(g, -alpha) * (0.5 * d2[i, j] / g - alpha * Math.Log(g));
            return g;
          }
        default: throw new ArgumentException("CovarianceRQiso has three hyperparameters", "k");
      }
    }

    private void CalculateSquaredDistances() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      d2 = new double[rows, cols];
      double lInv = 1.0 / l;
      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < rows; j++) {
            d2[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(xt, j).Select(e => e * lInv));
            d2[j, i] = d2[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            d2[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e * lInv), Util.GetRow(xt, j).Select(e => e * lInv));
          }
        }
      }
    }
  }
}
