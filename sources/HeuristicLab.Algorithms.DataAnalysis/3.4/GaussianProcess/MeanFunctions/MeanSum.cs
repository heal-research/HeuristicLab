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
  [Item(Name = "MeanSum", Description = "Sum of mean functions for Gaussian processes.")]
  public sealed class MeanSum : Item, IMeanFunction {
    [Storable]
    private ItemList<IMeanFunction> terms;

    [Storable]
    private int numberOfVariables;
    public ItemList<IMeanFunction> Terms {
      get { return terms; }
    }

    [StorableConstructor]
    private MeanSum(bool deserializing) : base(deserializing) { }
    private MeanSum(MeanSum original, Cloner cloner)
      : base(original, cloner) {
      this.terms = cloner.Clone(original.terms);
      this.numberOfVariables = original.numberOfVariables;
    }
    public MeanSum() {
      this.terms = new ItemList<IMeanFunction>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanSum(this, cloner);
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

    public double[] GetMean(double[,] x) {
      var res = terms.First().GetMean(x);
      foreach (var t in terms.Skip(1)) {
        var a = t.GetMean(x);
        for (int i = 0; i < res.Length; i++) res[i] += a[i];
      }
      return res;
    }

    public double[] GetGradients(int k, double[,] x) {
      int i = 0;
      while (k >= terms[i].GetNumberOfParameters(numberOfVariables)) {
        k -= terms[i].GetNumberOfParameters(numberOfVariables);
        i++;
      }
      return terms[i].GetGradients(k, x);
    }
  }
}
