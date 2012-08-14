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
  [Item(Name = "CovarianceProd",
    Description = "Product covariance function for Gaussian processes.")]
  public class CovarianceProd : Item, ICovarianceFunction {
    [Storable]
    private ItemList<ICovarianceFunction> factors;

    [Storable]
    private int numberOfVariables;
    public ItemList<ICovarianceFunction> Factors {
      get { return factors; }
    }

    [StorableConstructor]
    protected CovarianceProd(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceProd(CovarianceProd original, Cloner cloner)
      : base(original, cloner) {
      this.factors = cloner.Clone(original.factors);
      this.numberOfVariables = original.numberOfVariables;
      AttachEventHandlers();
    }

    public CovarianceProd()
      : base() {
      this.factors = new ItemList<ICovarianceFunction>();
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      this.factors.CollectionReset += (sender, args) => ClearCache();
      this.factors.ItemsAdded += (sender, args) => ClearCache();
      this.factors.ItemsRemoved += (sender, args) => ClearCache();
      this.factors.ItemsReplaced += (sender, args) => ClearCache();
      this.factors.ItemsMoved += (sender, args) => ClearCache();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceProd(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return factors.Select(f => f.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] hyp) {
      if (factors.Count == 0) throw new ArgumentException("at least one factor is necessary for the product covariance function.");
      int offset = 0;
      foreach (var t in factors) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(hyp.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }

    public double GetCovariance(double[,] x, int i, int j) {
      return factors.Select(f => f.GetCovariance(x, i, j)).Aggregate((a, b) => a * b);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      //if (cachedParameterMap == null) {
      //  CalculateParameterMap();
      //}
      //int ti = cachedParameterMap[k].Item1;
      //k = cachedParameterMap[k].Item2;
      //double gradient = 1.0;
      //for (int ii = 0; ii < factors.Count; ii++) {
      //  var f = factors[ii];
      //  if (ii == ti) {
      //    gradient *= f.GetGradient(x, i, j, k);
      //  } else {
      //    gradient *= f.GetCovariance(x, i, j);
      //  }
      //}
      //return gradient;
      var covariances = factors.Select(f => f.GetCovariance(x, i, j)).ToArray();
      for (int ii = 0; ii < factors.Count; ii++) {
        foreach (var g in factors[ii].GetGradient(x, i, j)) {
          double res = g;
          for (int jj = 0; jj < covariances.Length; jj++)
            if (ii != jj) res *= covariances[jj];
          yield return res;
        }
      }
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      return factors.Select(f => f.GetCrossCovariance(x, xt, i, j)).Aggregate((a, b) => a * b);
    }

    private Dictionary<int, Tuple<int, int>> cachedParameterMap;
    private void ClearCache() {
      cachedParameterMap = null;
    }

    private void CalculateParameterMap() {
      cachedParameterMap = new Dictionary<int, Tuple<int, int>>();
      int k = 0;
      for (int ti = 0; ti < factors.Count; ti++) {
        for (int ti_k = 0; ti_k < factors[ti].GetNumberOfParameters(numberOfVariables); ti_k++) {
          cachedParameterMap[k++] = Tuple.Create(ti, ti_k);
        }
      }
    }
  }
}
