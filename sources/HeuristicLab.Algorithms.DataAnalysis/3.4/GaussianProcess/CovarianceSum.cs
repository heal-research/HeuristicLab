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
    private CovarianceSum(bool deserializing)
      : base(deserializing) {
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

    public void SetParameter(double[] hyp) {
      if (terms.Count == 0) throw new ArgumentException("At least one term is needed for sum covariance function.");
      int offset = 0;
      foreach (var t in terms) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(hyp.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }

    public double GetCovariance(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      return terms.Select(t => t.GetCovariance(x, i, j, columnIndices)).Sum();
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      return terms.Select(t => t.GetGradient(x, i, j, columnIndices)).Aggregate(Enumerable.Concat);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      return terms.Select(t => t.GetCrossCovariance(x, xt, i, j, columnIndices)).Sum();
    }
  }
}
