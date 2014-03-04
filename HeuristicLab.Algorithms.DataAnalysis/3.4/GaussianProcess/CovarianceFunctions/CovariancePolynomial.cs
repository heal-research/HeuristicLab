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
  [Item(Name = "CovariancePolynomial",
    Description = "Polynomial covariance function for Gaussian processes.")]
  public sealed class CovariancePolynomial : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ConstParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Const"]; }
    }

    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<IntValue> DegreeParameter {
      get { return (IValueParameter<IntValue>)Parameters["Degree"]; }
    }
    private bool HasFixedConstParameter {
      get { return ConstParameter.Value != null; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }

    [StorableConstructor]
    private CovariancePolynomial(bool deserializing)
      : base(deserializing) {
    }

    private CovariancePolynomial(CovariancePolynomial original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovariancePolynomial()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Const", "Additive constant in the polymomial."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the polynomial covariance function."));
      Parameters.Add(new ValueParameter<IntValue>("Degree", "The degree of the polynomial (only non-zero positive values allowed).", new IntValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovariancePolynomial(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (HasFixedConstParameter ? 0 : 1) +
        (HasFixedScaleParameter ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double @const, scale;
      GetParameterValues(p, out @const, out scale);
      ConstParameter.Value = new DoubleValue(@const);
      ScaleParameter.Value = new DoubleValue(scale);
    }

    private void GetParameterValues(double[] p, out double @const, out double scale) {
      // gather parameter values
      int n = 0;
      if (HasFixedConstParameter) {
        @const = ConstParameter.Value.Value;
      } else {
        @const = Math.Exp(p[n]);
        n++;
      }

      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[n]);
        n++;
      }
      if (p.Length != n) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancePolynomial", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double @const, scale;
      int degree = DegreeParameter.Value.Value;
      if (degree <= 0) throw new ArgumentException("The degree parameter for CovariancePolynomial must be greater than zero.");
      GetParameterValues(p, out @const, out scale);
      var fixedConst = HasFixedConstParameter;
      var fixedScale = HasFixedScaleParameter;
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => scale * Math.Pow(@const + Util.ScalarProd(x, i, j, 1.0, columnIndices), degree);
      cov.CrossCovariance = (x, xt, i, j) => scale * Math.Pow(@const + Util.ScalarProd(x, i, xt, j, 1.0, columnIndices), degree);
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, @const, scale, degree, columnIndices, fixedConst, fixedScale);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double c, double scale, int degree, IEnumerable<int> columnIndices,
      bool fixedConst, bool fixedScale) {
      double s = Util.ScalarProd(x, i, j, 1.0, columnIndices);
      if (!fixedConst) yield return c * degree * scale * Math.Pow(c + s, degree - 1);
      if (!fixedScale) yield return 2 * scale * Math.Pow(c + s, degree);
    }
  }
}
