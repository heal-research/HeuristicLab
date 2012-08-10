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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanLinear", Description = "Linear mean function for Gaussian processes.")]
  public class MeanLinear : Item, IMeanFunction {
    [Storable]
    private double[] alpha;
    public double[] Weights {
      get {
        if (alpha == null) return new double[0];
        var copy = new double[alpha.Length];
        Array.Copy(alpha, copy, copy.Length);
        return copy;
      }
    }
    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables;
    }
    [StorableConstructor]
    protected MeanLinear(bool deserializing) : base(deserializing) { }
    protected MeanLinear(MeanLinear original, Cloner cloner)
      : base(original, cloner) {
      if (original.alpha != null) {
        this.alpha = new double[original.alpha.Length];
        Array.Copy(original.alpha, alpha, original.alpha.Length);
      }
    }
    public MeanLinear()
      : base() {
    }

    public void SetParameter(double[] hyp) {
      this.alpha = new double[hyp.Length];
      Array.Copy(hyp, alpha, hyp.Length);
    }
    public void SetData(double[,] x) {
      // nothing to do
    }

    public double[] GetMean(double[,] x) {
      // sanity check
      if (alpha.Length != x.GetLength(1)) throw new ArgumentException("The number of hyperparameters must match the number of variables for the linear mean function.");
      int cols = x.GetLength(1);
      int n = x.GetLength(0);
      return (from i in Enumerable.Range(0, n)
              let rowVector = from j in Enumerable.Range(0, cols)
                              select x[i, j]
              select Util.ScalarProd(alpha, rowVector))
        .ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      int cols = x.GetLength(1);
      int n = x.GetLength(0);
      if (k > cols) throw new ArgumentException();
      return (from r in Enumerable.Range(0, n)
              select x[r, k]).ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanLinear(this, cloner);
    }
  }
}
