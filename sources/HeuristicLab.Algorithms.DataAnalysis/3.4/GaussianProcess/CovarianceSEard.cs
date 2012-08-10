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
  [Item(Name = "CovarianceSEard", Description = "Squared exponential covariance function with automatic relevance determination for Gaussian processes.")]
  public class CovarianceSEard : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }

    [Storable]
    private double[] l;
    public double[] Length {
      get {
        if (l == null) return new double[0];
        var copy = new double[l.Length];
        Array.Copy(l, copy, copy.Length);
        return copy;
      }
    }

    private double[,] sd;
    private bool symmetric;

    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables + 1;
    }
    [StorableConstructor]
    protected CovarianceSEard(bool deserializing) : base(deserializing) { }
    protected CovarianceSEard(CovarianceSEard original, Cloner cloner)
      : base(original, cloner) {
      if (original.x != null) {
        this.x = new double[original.x.GetLength(0), original.x.GetLength(1)];
        Array.Copy(original.x, this.x, x.Length);

        this.xt = new double[original.xt.GetLength(0), original.xt.GetLength(1)];
        Array.Copy(original.xt, this.xt, xt.Length);

        this.sd = new double[original.sd.GetLength(0), original.sd.GetLength(1)];
        Array.Copy(original.sd, this.sd, sd.Length);

        this.l = new double[original.l.Length];
        Array.Copy(original.l, this.l, l.Length);
      }
      this.sf2 = original.sf2;
      this.symmetric = original.symmetric;
    }
    public CovarianceSEard()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSEard(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      this.l = hyp.Take(hyp.Length - 1).Select(Math.Exp).ToArray();
      this.sf2 = Math.Exp(2 * hyp[hyp.Length - 1]);
      // sf2 = Math.Min(10E6, sf2); // upper limit for the scale

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
      return sf2 * Math.Exp(-sd[i, j] / 2.0);
    }

    public double GetGradient(int i, int j, int k) {
      if (k < l.Length) {
        double sqrDist = Util.SqrDist(x[i, k] / l[k], xt[j, k] / l[k]);
        return sf2 * Math.Exp(-sd[i, j] / 2.0) * sqrDist;
      } else if (k == l.Length) {
        return 2.0 * sf2 * Math.Exp(-sd[i, j] / 2.0);
      } else {
        throw new ArgumentException("CovarianceSEard has dimension+1 hyperparameters.", "k");
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
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select((e, k) => e / l[k]),
                                    Util.GetRow(xt, j).Select((e, k) => e / l[k]));
            sd[j, i] = sd[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select((e, k) => e / l[k]),
                                    Util.GetRow(xt, j).Select((e, k) => e / l[k]));
          }
        }
      }
    }
  }
}
