#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("5C4AB3C8-48D5-4B6E-830A-89AE2A363C64")]
  [Item(Name = "CovarianceLinear", Description = "Linear covariance function for Gaussian processes.")]
  public sealed class CovarianceLinear : Item, ICovarianceFunction {
    [StorableConstructor]
    private CovarianceLinear(StorableConstructorFlag _) : base(_) { }
    private CovarianceLinear(CovarianceLinear original, Cloner cloner)
      : base(original, cloner) {
    }
    public CovarianceLinear()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinear(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }

    public void SetParameter(double[] p) {
      if (p.Length > 0) throw new ArgumentException("No parameters are allowed for the linear covariance function.");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      if (p.Length > 0) throw new ArgumentException("No parameters are allowed for the linear covariance function.");
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => Util.ScalarProd(x, i, j, columnIndices, 1.0);
      cov.CrossCovariance = (x, xt, i, j) => Util.ScalarProd(x, i, xt, j, columnIndices, 1.0);
      cov.CovarianceGradient = (x, i, j) => new double[0];
      return cov;
    }
  }
}
