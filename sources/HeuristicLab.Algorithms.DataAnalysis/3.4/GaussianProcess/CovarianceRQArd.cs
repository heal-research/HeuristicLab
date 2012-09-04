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
  [Item(Name = "CovarianceRQArd",
    Description = "Rational quadratic covariance function with automatic relevance determination for Gaussian processes.")]
  public class CovarianceRQArd : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double[] inverseLength;
    public double[] InverseLength {
      get {
        if (inverseLength == null) return null;
        double[] res = new double[inverseLength.Length];
        Array.Copy(inverseLength, res, res.Length);
        return res;
      }
    }
    [Storable]
    private double alpha;
    public double Shape { get { return alpha; } }

    [StorableConstructor]
    protected CovarianceRQArd(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceRQArd(CovarianceRQArd original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.inverseLength = original.InverseLength; // array is cloned in the getter
      this.alpha = original.alpha;
    }

    public CovarianceRQArd()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRQArd(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables + 2;
    }

    public void SetParameter(double[] hyp) {
      this.inverseLength = hyp.Take(hyp.Length - 2).Select(e => 1.0 / Math.Exp(e)).ToArray();
      this.sf2 = Math.Exp(2 * hyp[hyp.Length - 2]);
      this.alpha = Math.Exp(hyp[hyp.Length - 1]);
    }


    public double GetCovariance(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);
      return sf2 * Math.Pow(1 + 0.5 * d / alpha, -alpha);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);
      double b = 1 + 0.5 * d / alpha;
      for (int k = 0; k < inverseLength.Length; k++) {
        yield return sf2 * Math.Pow(b, -alpha - 1) * Util.SqrDist(x[i, k] * inverseLength[k], x[j, k] * inverseLength[k]);
      }
      yield return 2 * sf2 * Math.Pow(b, -alpha);
      yield return sf2 * Math.Pow(b, -alpha) * (0.5 * d / b - alpha * Math.Log(b));
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double d = Util.SqrDist(x, i, xt, j, inverseLength);
      return sf2 * Math.Pow(1 + 0.5 * d / alpha, -alpha);
    }
  }
}
