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
  [Item(Name = "MeanProduct", Description = "Product of mean functions for Gaussian processes.")]
  public sealed class MeanProduct : Item, IMeanFunction {
    [Storable]
    private ItemList<IMeanFunction> factors;

    [Storable]
    private int numberOfVariables;

    public ItemList<IMeanFunction> Factors {
      get { return factors; }
    }

    [StorableConstructor]
    private MeanProduct(bool deserializing)
      : base(deserializing) {
    }

    private MeanProduct(MeanProduct original, Cloner cloner)
      : base(original, cloner) {
      this.factors = cloner.Clone(original.factors);
      this.numberOfVariables = original.numberOfVariables;
    }

    public MeanProduct() {
      this.factors = new ItemList<IMeanFunction>();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanProduct(this, cloner);
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

    public double[] GetMean(double[,] x) {
      var res = factors.First().GetMean(x);
      foreach (var t in factors.Skip(1)) {
        var a = t.GetMean(x);
        for (int i = 0; i < res.Length; i++) res[i] *= a[i];
      }
      return res;
    }

    public double[] GetGradients(int k, double[,] x) {
      double[] res = Enumerable.Repeat(1.0, x.GetLength(0)).ToArray();
      // find index of factor for the given k
      int j = 0;
      while (k >= factors[j].GetNumberOfParameters(numberOfVariables)) {
        k -= factors[j].GetNumberOfParameters(numberOfVariables);
        j++;
      }
      for (int i = 0; i < factors.Count; i++) {
        var f = factors[i];
        if (i == j) {
          // multiply gradient
          var g = f.GetGradients(k, x);
          for (int ii = 0; ii < res.Length; ii++) res[ii] *= g[ii];
        } else {
          // multiply mean
          var m = f.GetMean(x);
          for (int ii = 0; ii < res.Length; ii++) res[ii] *= m[ii];
        }
      }
      return res;
    }
  }
}
