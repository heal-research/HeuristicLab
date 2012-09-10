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
  [Item(Name = "CovarianceSEard", Description = "Squared exponential covariance function with automatic relevance determination for Gaussian processes.")]
  public sealed class CovarianceSEard : ParameterizedNamedItem, ICovarianceFunction {
    [Storable]
    private double sf2;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter { get { return scaleParameter; } }

    [Storable]
    private double[] inverseLength;
    [Storable]
    private readonly HyperParameter<DoubleArray> inverseLengthParameter;
    public IValueParameter<DoubleArray> InverseLengthParameter { get { return inverseLengthParameter; } }

    [StorableConstructor]
    private CovarianceSEard(bool deserializing) : base(deserializing) { }
    private CovarianceSEard(CovarianceSEard original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.scaleParameter = cloner.Clone(original.scaleParameter);

      if (original.inverseLength != null) {
        this.inverseLength = new double[original.inverseLength.Length];
        Array.Copy(original.inverseLength, this.inverseLength, this.inverseLength.Length);
      }
      this.inverseLengthParameter = cloner.Clone(original.inverseLengthParameter);

      RegisterEvents();
    }
    public CovarianceSEard()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale parameter of the squared exponential covariance function with ARD.");
      this.inverseLengthParameter = new HyperParameter<DoubleArray>("InverseLength", "The inverse length parameter for automatic relevance determination.");

      Parameters.Add(scaleParameter);
      Parameters.Add(inverseLengthParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSEard(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { sf2 = scaleParameter.Value.Value; });
      Util.AttachArrayChangeHandler<DoubleArray, double>(inverseLengthParameter, () => {
        inverseLength =
          inverseLengthParameter.Value.ToArray();
      });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (scaleParameter.Fixed ? 0 : 1) +
        (inverseLengthParameter.Fixed ? 0 : numberOfVariables);
    }


    public void SetParameter(double[] hyp) {
      int i = 0;
      if (!scaleParameter.Fixed) {
        scaleParameter.SetValue(new DoubleValue(Math.Exp(2 * hyp[i])));
        i++;
      }
      if (!inverseLengthParameter.Fixed) {
        inverseLengthParameter.SetValue(new DoubleArray(hyp.Skip(i).Select(e => 1.0 / Math.Exp(e)).ToArray()));
        i += hyp.Skip(i).Count();
      }
      if (hyp.Length != i) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancSEard", "hyp");
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
