#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item(Name = "CovariancePiecewisePolynomial",
    Description = "Piecewise polynomial covariance function with compact support for Gaussian processes.")]
  public sealed class CovariancePiecewisePolynomial : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> LengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Length"]; }
    }

    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IConstrainedValueParameter<IntValue> VParameter {
      get { return (IConstrainedValueParameter<IntValue>)Parameters["V"]; }
    }

    [StorableConstructor]
    private CovariancePiecewisePolynomial(bool deserializing)
      : base(deserializing) {
    }

    private CovariancePiecewisePolynomial(CovariancePiecewisePolynomial original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovariancePiecewisePolynomial()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Length", "The length parameter of the isometric piecewise polynomial covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the piecewise polynomial covariance function."));

      var validValues = new ItemSet<IntValue>(new IntValue[] { 
        (IntValue)(new IntValue().AsReadOnly()), 
        (IntValue)(new IntValue(1).AsReadOnly()), 
        (IntValue)(new IntValue(2).AsReadOnly()), 
        (IntValue)(new IntValue(3).AsReadOnly()) });
      Parameters.Add(new ConstrainedValueParameter<IntValue>("V", "The v parameter of the piecewise polynomial function (allowed values 0, 1, 2, 3).", validValues, validValues.First()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovariancePiecewisePolynomial(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (LengthParameter.Value != null ? 0 : 1) +
        (ScaleParameter.Value != null ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double @const, scale;
      GetParameterValues(p, out @const, out scale);
      LengthParameter.Value = new DoubleValue(@const);
      ScaleParameter.Value = new DoubleValue(scale);
    }

    private void GetParameterValues(double[] p, out double length, out double scale) {
      // gather parameter values
      int n = 0;
      if (LengthParameter.Value != null) {
        length = LengthParameter.Value.Value;
      } else {
        length = Math.Exp(p[n]);
        n++;
      }

      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[n]);
        n++;
      }
      if (p.Length != n) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancePiecewisePolynomial", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double length, scale;
      int v = VParameter.Value.Value;
      GetParameterValues(p, out length, out scale);
      int exp = (int)Math.Floor(columnIndices.Count() / 2.0) + v + 1;

      Func<double, double> f;
      Func<double, double> df;
      switch (v) {
        case 0:
          f = (r) => 1.0;
          df = (r) => 0.0;
          break;
        case 1:
          f = (r) => 1 + (exp + 1) * r;
          df = (r) => exp + 1;
          break;
        case 2:
          f = (r) => 1 + (exp + 2) * r + (exp * exp + 4.0 * exp + 3) / 3.0 * r * r;
          df = (r) => (exp + 2) + 2 * (exp * exp + 4.0 * exp + 3) / 3.0 * r;
          break;
        case 3:
          f = (r) => 1 + (exp + 3) * r + (6.0 * exp * exp + 36 * exp + 45) / 15.0 * r * r +
                     (exp * exp * exp + 9 * exp * exp + 23 * exp + 45) / 15.0 * r * r * r;
          df = (r) => (exp + 3) + 2 * (6.0 * exp * exp + 36 * exp + 45) / 15.0 * r +
                      (exp * exp * exp + 9 * exp * exp + 23 * exp + 45) / 5.0 * r * r;
          break;
        default: throw new ArgumentException();
      }

      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double k = Math.Sqrt(Util.SqrDist(x, i, x, j, 1.0 / length, columnIndices));
        return scale * Math.Pow(Math.Max(1 - k, 0), exp + v) * f(k);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double k = Math.Sqrt(Util.SqrDist(x, i, xt, j, 1.0 / length, columnIndices));
        return scale * Math.Pow(Math.Max(1 - k, 0), exp + v) * f(k);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, length, scale, v, exp, f, df, columnIndices);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double length, double scale, int v, double exp, Func<double, double> f, Func<double, double> df, IEnumerable<int> columnIndices) {
      double k = Math.Sqrt(Util.SqrDist(x, i, x, j, 1.0 / length, columnIndices));
      yield return scale * Math.Pow(Math.Max(1.0 - k, 0), exp + v - 1) * k * ((exp + v) * f(k) - Math.Max(1 - k, 0) * df(k));
      yield return 2.0 * scale * Math.Pow(Math.Max(1 - k, 0), exp + v) * f(k);
    }
  }
}
