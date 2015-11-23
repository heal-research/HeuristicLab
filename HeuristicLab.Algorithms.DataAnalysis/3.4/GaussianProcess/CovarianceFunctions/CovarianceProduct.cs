#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq.Expressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceProduct",
    Description = "Product covariance function for Gaussian processes.")]
  public sealed class CovarianceProduct : Item, ICovarianceFunction {
    [Storable]
    private ItemList<ICovarianceFunction> factors;

    [Storable]
    private int numberOfVariables;
    public ItemList<ICovarianceFunction> Factors {
      get { return factors; }
    }

    [StorableConstructor]
    private CovarianceProduct(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceProduct(CovarianceProduct original, Cloner cloner)
      : base(original, cloner) {
      this.factors = cloner.Clone(original.factors);
      this.numberOfVariables = original.numberOfVariables;
    }

    public CovarianceProduct()
      : base() {
      this.factors = new ItemList<ICovarianceFunction>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceProduct(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return factors.Select(f => f.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] p) {
      int offset = 0;
      foreach (var f in factors) {
        var numberOfParameters = f.GetNumberOfParameters(numberOfVariables);
        f.SetParameter(p.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      if (factors.Count == 0) throw new ArgumentException("at least one factor is necessary for the product covariance function.");
      var functions = new List<ParameterizedCovarianceFunction>();
      foreach (var f in factors) {
        int numberOfParameters = f.GetNumberOfParameters(numberOfVariables);
        functions.Add(f.GetParameterizedCovarianceFunction(p.Take(numberOfParameters).ToArray(), columnIndices));
        p = p.Skip(numberOfParameters).ToArray();
      }


      var product = new ParameterizedCovarianceFunction();
      product.Covariance = (x, i, j) => functions.Select(e => e.Covariance(x, i, j)).Aggregate((a, b) => a * b);
      product.CrossCovariance = (x, xt, i, j) => functions.Select(e => e.CrossCovariance(x, xt, i, j)).Aggregate((a, b) => a * b);
      product.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, functions);
      return product;
    }

    public static IEnumerable<double> GetGradient(double[,] x, int i, int j, List<ParameterizedCovarianceFunction> factorFunctions) {
      var covariances = factorFunctions.Select(f => f.Covariance(x, i, j)).ToArray();
      for (int ii = 0; ii < factorFunctions.Count; ii++) {
        foreach (var g in factorFunctions[ii].CovarianceGradient(x, i, j)) {
          double res = g;
          for (int jj = 0; jj < covariances.Length; jj++)
            if (ii != jj) res *= covariances[jj];
          yield return res;
        }
      }
    }
  }
}
