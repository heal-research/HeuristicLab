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
    }

    public CovarianceProd()
      : base() {
      this.factors = new ItemList<ICovarianceFunction>();
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

    public double GetGradient(int i, int j, int k) {
      // map from parameter index to factor
      var vi = factors.Select((f, idx) => Enumerable.Repeat(idx, f.GetNumberOfParameters(numberOfVariables))).SelectMany(x => x).ToArray();
      double res = 1.0;
      int jj = Enumerable.Range(0, k).Count(e => vi[e] == vi[k]);
      for (int ii = 0; ii < factors.Count; ii++) {
        var f = factors[ii];
        if (ii == vi[k]) {
          res *= f.GetGradient(i, j, jj);
        } else {
          res *= f.GetCovariance(i, j);
        }
      }
      return res;
    }
  }
}
