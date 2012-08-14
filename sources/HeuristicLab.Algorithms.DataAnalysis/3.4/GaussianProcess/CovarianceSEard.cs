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
  [Item(Name = "CovarianceSEard", Description = "Squared exponential covariance function with automatic relevance determination for Gaussian processes.")]
  public class CovarianceSEard : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }

    [Storable]
    private double[] inverseLength;
    public double[] Length {
      get {
        if (inverseLength == null) return new double[0];
        var copy = new double[inverseLength.Length];
        Array.Copy(inverseLength, copy, copy.Length);
        return copy;
      }
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables + 1;
    }
    [StorableConstructor]
    protected CovarianceSEard(bool deserializing) : base(deserializing) { }
    protected CovarianceSEard(CovarianceSEard original, Cloner cloner)
      : base(original, cloner) {
      if (original.inverseLength != null) {
        this.inverseLength = new double[original.inverseLength.Length];
        Array.Copy(original.inverseLength, this.inverseLength, inverseLength.Length);
      }
      this.sf2 = original.sf2;
    }
    public CovarianceSEard()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSEard(this, cloner);
    }

    public void SetParameter(double[] hyp) {
      this.inverseLength = hyp.Take(hyp.Length - 1).Select(p => 1.0 / Math.Exp(p)).ToArray();
      this.sf2 = Math.Exp(2 * hyp[hyp.Length - 1]);
    }

    public double GetCovariance(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);
      return sf2 * Math.Exp(-d / 2.0);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);

      for (int ii = 0; ii < inverseLength.Length; ii++) {
        double sqrDist = Util.SqrDist(x[i, ii] * inverseLength[ii], x[j, ii] * inverseLength[ii]);
        yield return sf2 * Math.Exp(-d / 2.0) * sqrDist;
      }
      yield return 2.0 * sf2 * Math.Exp(-d / 2.0);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double d = Util.SqrDist(x, i, xt, j, inverseLength);
      return sf2 * Math.Exp(-d / 2.0);
    }
  }
}
