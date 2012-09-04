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
  [Item(Name = "CovarianceLinearArd",
    Description = "Linear covariance function with automatic relevance determination for Gaussian processes.")]
  public class CovarianceLinearArd : Item, ICovarianceFunction {
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

    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables;
    }

    [StorableConstructor]
    protected CovarianceLinearArd(bool deserializing) : base(deserializing) { }
    protected CovarianceLinearArd(CovarianceLinearArd original, Cloner cloner)
      : base(original, cloner) {
      this.inverseLength = original.InverseLength;  // array is copied in the getter
    }
    public CovarianceLinearArd()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinearArd(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      inverseLength = hyp.Select(e => 1.0 / Math.Exp(e)).ToArray();
    }

    public double GetCovariance(double[,] x, int i, int j) {
      return Util.ScalarProd(x, i, j, inverseLength);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      for (int k = 0; k < inverseLength.Length; k++) {
        yield return -2.0 * x[i, k] * x[j, k] * inverseLength[k] * inverseLength[k];
      }
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      return Util.ScalarProd(x, i, xt, j, inverseLength);
    }
  }
}
