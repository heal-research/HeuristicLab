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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceRationalQuadraticArd",
    Description = "Rational quadratic covariance function with automatic relevance determination for Gaussian processes.")]
  public sealed class CovarianceRationalQuadraticArd : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleArray> InverseLengthParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["InverseLength"]; }
    }

    public IValueParameter<DoubleValue> ShapeParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Shape"]; }
    }

    [StorableConstructor]
    private CovarianceRationalQuadraticArd(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceRationalQuadraticArd(CovarianceRationalQuadraticArd original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceRationalQuadraticArd()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the rational quadratic covariance function with ARD."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>("InverseLength", "The inverse length parameter for automatic relevance determination."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Shape", "The shape parameter (alpha) of the rational quadratic covariance function with ARD."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRationalQuadraticArd(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (ScaleParameter.Value != null ? 0 : 1) +
        (ShapeParameter.Value != null ? 0 : 1) +
        (InverseLengthParameter.Value != null ? 0 : numberOfVariables);
    }

    public void SetParameter(double[] p) {
      double scale, shape;
      double[] inverseLength;
      GetParameterValues(p, out scale, out shape, out inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
      ShapeParameter.Value = new DoubleValue(shape);
      InverseLengthParameter.Value = new DoubleArray(inverseLength);
    }

    private void GetParameterValues(double[] p, out double scale, out double shape, out double[] inverseLength) {
      int c = 0;
      // gather parameter values
      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (ShapeParameter.Value != null) {
        shape = ShapeParameter.Value.Value;
      } else {
        shape = Math.Exp(p[c]);
        c++;
      }
      if (InverseLengthParameter.Value != null) {
        inverseLength = InverseLengthParameter.Value.ToArray();
      } else {
        inverseLength = p.Skip(c).Select(e => 1.0 / Math.Exp(e)).ToArray();
        c += inverseLength.Length;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceRationalQuadraticArd", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double scale, shape;
      double[] inverseLength;
      GetParameterValues(p, out scale, out shape, out inverseLength);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double d = i == j
                    ? 0.0
                    : Util.SqrDist(x, i, j, inverseLength, columnIndices);
        return scale * Math.Pow(1 + 0.5 * d / shape, -shape);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double d = Util.SqrDist(x, i, xt, j, inverseLength, columnIndices);
        return scale * Math.Pow(1 + 0.5 * d / shape, -shape);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, shape, inverseLength);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices, double scale, double shape, double[] inverseLength) {
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);
      double b = 1 + 0.5 * d / shape;
      int k = 0;
      foreach (var columnIndex in columnIndices) {
        yield return scale * Math.Pow(b, -shape - 1) * Util.SqrDist(x[i, columnIndex] * inverseLength[k], x[j, columnIndex] * inverseLength[k]);
        k++;
      }
      yield return 2 * scale * Math.Pow(b, -shape);
      yield return scale * Math.Pow(b, -shape) * (0.5 * d / b - shape * Math.Log(b));
    }
  }
}
