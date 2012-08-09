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
  [Item(Name = "CovarianceLinear", Description = "Linear covariance function for Gaussian processes.")]
  public class CovarianceLinear : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;

    private double[,] k;
    private bool symmetric;

    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }
    [StorableConstructor]
    protected CovarianceLinear(bool deserializing) : base(deserializing) { }
    protected CovarianceLinear(CovarianceLinear original, Cloner cloner)
      : base(original, cloner) {
      if (original.x != null) {
        this.x = new double[original.x.GetLength(0), original.x.GetLength(1)];
        Array.Copy(original.x, this.x, x.Length);

        this.xt = new double[original.xt.GetLength(0), original.xt.GetLength(1)];
        Array.Copy(original.xt, this.xt, xt.Length);

        this.k = new double[original.k.GetLength(0), original.k.GetLength(1)];
        Array.Copy(original.k, this.k, k.Length);
      }
      this.symmetric = original.symmetric;
    }
    public CovarianceLinear()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinear(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length > 0) throw new ArgumentException("No hyperparameters are allowed for the linear covariance function.");
      k = null;
    }

    public void SetData(double[,] x) {
      SetData(x, x);
      this.symmetric = true;
    }

    public void SetData(double[,] x, double[,] xt) {
      this.x = x;
      this.xt = xt;
      this.symmetric = false;

      k = null;
    }

    public double GetCovariance(int i, int j) {
      if (k == null) CalculateInnerProduct();
      return k[i, j];
    }

    public double GetGradient(int i, int j, int k) {
      throw new NotSupportedException("CovarianceLinear does not have hyperparameters.");
    }


    private void CalculateInnerProduct() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      k = new double[rows, cols];
      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < cols; j++) {
            k[i, j] = Util.ScalarProd(Util.GetRow(x, i),
                                      Util.GetRow(x, j));
            k[j, i] = k[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            k[i, j] = Util.ScalarProd(Util.GetRow(x, i),
                                      Util.GetRow(xt, j));
          }
        }
      }
    }
  }
}
