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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceLinearArd",
    Description = "Linear covariance function with automatic relevance determination for Gaussian processes.")]
  public class CovarianceLinearArd : CovarianceFunction {
    public IValueParameter<DoubleArray> InverseLengthParameter {
      get { return inverseLengthParameter; }
    }

    [Storable]
    private HyperParameter<DoubleArray> inverseLengthParameter;

    [Storable]
    private double[] inverseLength;

    [StorableConstructor]
    protected CovarianceLinearArd(bool deserializing) : base(deserializing) { }
    protected CovarianceLinearArd(CovarianceLinearArd original, Cloner cloner)
      : base(original, cloner) {
      inverseLengthParameter = cloner.Clone(original.inverseLengthParameter);
      if (original.inverseLength != null) {
        this.inverseLength = new double[original.inverseLength.Length];
        Array.Copy(original.inverseLength, inverseLength, inverseLength.Length);
      }

      RegisterEvents();
    }
    public CovarianceLinearArd()
      : base() {
      inverseLengthParameter = new HyperParameter<DoubleArray>("InverseLength",
                                                               "The inverse length parameter for ARD.");
      Parameters.Add(inverseLengthParameter);
      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinearArd(this, cloner);
    }

    // caching
    private void RegisterEvents() {
      AttachArrayChangeHandler<DoubleArray, double>(inverseLengthParameter, () => { inverseLength = inverseLengthParameter.Value.ToArray(); });
    }


    public override int GetNumberOfParameters(int numberOfVariables) {
      if (!inverseLengthParameter.Fixed)
        return numberOfVariables;
      else
        return 0;
    }

    public override void SetParameter(double[] hyp) {
      if (!inverseLengthParameter.Fixed && hyp.Length > 0) {
        inverseLengthParameter.SetValue(new DoubleArray(hyp.Select(e => 1.0 / Math.Exp(e)).ToArray()));
      } else throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceLinearArd", "hyp");
    }

    public override double GetCovariance(double[,] x, int i, int j) {
      return Util.ScalarProd(x, i, j, inverseLength);
    }

    public override IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      for (int k = 0; k < inverseLength.Length; k++) {
        yield return -2.0 * x[i, k] * x[j, k] * inverseLength[k] * inverseLength[k];
      }
    }

    public override double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      return Util.ScalarProd(x, i, xt, j, inverseLength);
    }
  }
}
