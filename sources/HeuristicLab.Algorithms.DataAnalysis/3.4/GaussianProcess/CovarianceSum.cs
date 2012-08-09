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
  [Item(Name = "CovarianceSum",
    Description = "Sum covariance function for Gaussian processes.")]
  public class CovarianceSum : Item, ICovarianceFunction {
    [Storable]
    private ItemList<ICovarianceFunction> terms;

    [Storable]
    private int numberOfVariables;
    public ItemList<ICovarianceFunction> Terms {
      get { return terms; }
    }

    [StorableConstructor]
    protected CovarianceSum(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceSum(CovarianceSum original, Cloner cloner)
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
      int offset = 0;
      foreach (var t in terms) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(hyp.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }
    public void SetData(double[,] x) {
      SetData(x, x);
    }

    public void SetData(double[,] x, double[,] xt) {
      foreach (var t in terms) {
        t.SetData(x, xt);
      }
    }

    public double GetCovariance(int i, int j) {
      return terms.Select(t => t.GetCovariance(i, j)).Sum();
    }

    public double GetGradient(int i, int j, int k) {
      int ii = 0;
      while (k > terms[ii].GetNumberOfParameters(numberOfVariables)) {
        k -= terms[ii].GetNumberOfParameters(numberOfVariables);
      }
      return terms[ii].GetGradient(i, j, k);
    }
  }
}
