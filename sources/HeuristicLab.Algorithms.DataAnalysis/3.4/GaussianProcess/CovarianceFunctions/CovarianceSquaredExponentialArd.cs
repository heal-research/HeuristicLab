#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceSquaredExponentialArd", Description = "Squared exponential covariance function with automatic relevance determination for Gaussian processes.")]
  public sealed class CovarianceSquaredExponentialArd : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleArray> InverseLengthParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["InverseLength"]; }
    }
    private bool HasFixedInverseLengthParameter {
      get { return InverseLengthParameter.Value != null; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceSquaredExponentialArd(bool deserializing) : base(deserializing) { }
    private CovarianceSquaredExponentialArd(CovarianceSquaredExponentialArd original, Cloner cloner)
      : base(original, cloner) {
    }
    public CovarianceSquaredExponentialArd()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the squared exponential covariance function with ARD."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>("InverseLength", "The inverse length parameter for automatic relevance determination."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSquaredExponentialArd(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (HasFixedScaleParameter ? 0 : 1) +
        (HasFixedInverseLengthParameter ? 0 : numberOfVariables);
    }

    public void SetParameter(double[] p) {
      double scale;
      double[] inverseLength;
      GetParameterValues(p, out scale, out inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
      InverseLengthParameter.Value = new DoubleArray(inverseLength);
    }

    private void GetParameterValues(double[] p, out double scale, out double[] inverseLength) {
      int c = 0;
      // gather parameter values
      if (HasFixedInverseLengthParameter) {
        inverseLength = InverseLengthParameter.Value.ToArray();
      } else {
        int length = p.Length;
        if (!HasFixedScaleParameter) length--;
        inverseLength = p.Select(e => 1.0 / Math.Exp(e)).Take(length).ToArray();
        c += inverseLength.Length;
      }
      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceSquaredExponentialArd", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double scale;
      double[] inverseLength;
      GetParameterValues(p, out scale, out inverseLength);
      var fixedInverseLength = HasFixedInverseLengthParameter;
      var fixedScale = HasFixedScaleParameter;
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double d = i == j
                 ? 0.0
                 : Util.SqrDist(x, i, j, inverseLength, columnIndices);
        return scale * Math.Exp(-d / 2.0);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double d = Util.SqrDist(x, i, xt, j, inverseLength, columnIndices);
        return scale * Math.Exp(-d / 2.0);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, inverseLength, fixedInverseLength, fixedScale);
      return cov;
    }

    // order of returned gradients must match the order in GetParameterValues!
    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, int[] columnIndices, double scale, double[] inverseLength,
      bool fixedInverseLength, bool fixedScale) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);

      int k = 0;
      if (!fixedInverseLength) {
        for (int c = 0; c < columnIndices.Length; c++) {
          var columnIndex = columnIndices[c];
          double sqrDist = Util.SqrDist(x[i, columnIndex] * inverseLength[k], x[j, columnIndex] * inverseLength[k]);
          yield return scale * Math.Exp(-d / 2.0) * sqrDist;
          k++;
        }
      }
      if (!fixedScale) yield return 2.0 * scale * Math.Exp(-d / 2.0);
    }
  }
}
