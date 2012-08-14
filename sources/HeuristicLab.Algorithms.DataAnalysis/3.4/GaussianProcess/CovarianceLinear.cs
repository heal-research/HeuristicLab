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
  [Item(Name = "CovarianceLinear", Description = "Linear covariance function for Gaussian processes.")]
  public class CovarianceLinear : Item, ICovarianceFunction {
    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }
    [StorableConstructor]
    protected CovarianceLinear(bool deserializing) : base(deserializing) { }
    protected CovarianceLinear(CovarianceLinear original, Cloner cloner)
      : base(original, cloner) {
    }
    public CovarianceLinear()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinear(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length > 0) throw new ArgumentException("No hyperparameters are allowed for the linear covariance function.");
    }

    public double GetCovariance(double[,] x, int i, int j) {
      return Util.ScalarProd(Util.GetRow(x, i), Util.GetRow(x, j));
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      yield break;
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      return Util.ScalarProd(Util.GetRow(x, i), Util.GetRow(xt, j));
    }
  }
}
