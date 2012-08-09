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
      return factors.Select(t => t.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] hyp) {
      int offset = 0;
      foreach (var t in factors) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(hyp.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }
    public void SetData(double[,] x) {
      SetData(x, x);
    }

    public void SetData(double[,] x, double[,] xt) {
      foreach (var t in factors) {
        t.SetData(x, xt);
      }
    }

    public double GetCovariance(int i, int j) {
      return factors.Select(t => t.GetCovariance(i, j)).Aggregate((a, b) => a * b);
    }

    private Dictionary<int, Tuple<int, int>> cachedParameterMap;
    public double GetGradient(int i, int j, int k) {
      if (cachedParameterMap == null) {
        CalculateParameterMap();
      }
      int ti = cachedParameterMap[k].Item1;
      k = cachedParameterMap[k].Item2;
      double res = 1.0;
      for (int ii = 0; ii < factors.Count; ii++) {
        var f = factors[ii];
        if (ii == ti) {
          res *= f.GetGradient(i, j, k);
        } else {
          res *= f.GetCovariance(i, j);
        }
      }
      return res;
    }

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
