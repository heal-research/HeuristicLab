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
  [Item(Name = "CovarianceSEiso",
    Description = "Isotropic squared exponential covariance function for Gaussian processes.")]
  public class CovarianceSEiso : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;
    [Storable]
    private double sf2;
    [Storable]
    private double l;
    [Storable]
    private bool symmetric;
    private double[,] sd;

    [StorableConstructor]
    protected CovarianceSEiso(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceSEiso(CovarianceSEiso original, Cloner cloner)
      : base(original, cloner) {
      if (original.x != null) {
        this.x = new double[original.x.GetLength(0), original.x.GetLength(1)];
        Array.Copy(original.x, this.x, x.Length);

        this.xt = new double[original.xt.GetLength(0), original.xt.GetLength(1)];
        Array.Copy(original.xt, this.xt, xt.Length);

        this.sd = new double[original.sd.GetLength(0), original.sd.GetLength(1)];
        Array.Copy(original.sd, this.sd, sd.Length);
        this.sf2 = original.sf2;
      }
      this.sf2 = original.sf2;
      this.l = original.l;
      this.symmetric = original.symmetric;
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
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Min(1E6, Math.Exp(2 * hyp[1])); // upper limit for scale
      sd = null;
    }
    public void SetData(double[,] x) {
      SetData(x, x);
      this.symmetric = true;
    }


    public void SetData(double[,] x, double[,] xt) {
      this.symmetric = false;
      this.x = x;
      this.xt = xt;
      sd = null;
    }

    public double GetCovariance(int i, int j) {
      if (sd == null) CalculateSquaredDistances();
      return sf2 * Math.Exp(-sd[i, j] / 2.0);
    }

    public double[] GetGradient(int i, int j) {
      var res = new double[2];
      res[0] = sf2 * Math.Exp(-sd[i, j] / 2.0) * sd[i, j];
      res[1] = 2.0 * sf2 * Math.Exp(-sd[i, j] / 2.0);
      return res;
    }

    private void CalculateSquaredDistances() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      sd = new double[rows, cols];
      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < rows; j++) {
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e / l), Util.GetRow(xt, j).Select(e => e / l));
            sd[j, i] = sd[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e / l), Util.GetRow(xt, j).Select(e => e / l));
          }
        }
      }
    }
  }
}
