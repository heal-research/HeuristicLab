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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("8F1A684A-98BE-429A-BDA2-E1FB7DDF09F0")]
  [Item(Name = "CovarianceSum",
    Description = "Sum covariance function for Gaussian processes.")]
  public sealed class CovarianceSum : Item, ICovarianceFunction {
    [Storable]
    private ItemList<ICovarianceFunction> terms;

    [Storable]
    private int numberOfVariables;
    public ItemList<ICovarianceFunction> Terms {
      get { return terms; }
    }

    [StorableConstructor]
    private CovarianceSum(StorableConstructorFlag _) : base(_) {
    }

    private CovarianceSum(CovarianceSum original, Cloner cloner)
      : base(original, cloner) {
      this.terms = cloner.Clone(original.terms);
      this.numberOfVariables = original.numberOfVariables;
    }

    public CovarianceSum()
      : base() {
      this.terms = new ItemList<ICovarianceFunction>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSum(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return terms.Select(t => t.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] p) {
      int offset = 0;
      foreach (var t in terms) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(p.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      if (terms.Count == 0) throw new ArgumentException("at least one term is necessary for the product covariance function.");
      var functions = new List<ParameterizedCovarianceFunction>();
      foreach (var t in terms) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        functions.Add(t.GetParameterizedCovarianceFunction(p.Take(numberOfParameters).ToArray(), columnIndices));
        p = p.Skip(numberOfParameters).ToArray();
      }

      var sum = new ParameterizedCovarianceFunction();
      sum.Covariance = (x, i, j) => functions.Select(e => e.Covariance(x, i, j)).Sum();
      sum.CrossCovariance = (x, xt, i, j) => functions.Select(e => e.CrossCovariance(x, xt, i, j)).Sum();
      sum.CovarianceGradient = (x, i, j) => {
        var g = new List<double>();
        foreach (var e in functions)
          g.AddRange(e.CovarianceGradient(x, i, j));
        return g;
      };
      return sum;
    }
  }
}
