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
using System.Linq.Expressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceSquaredExponentialIso",
    Description = "Isotropic squared exponential covariance function for Gaussian processes.")]
  public sealed class CovarianceSquaredExponentialIso : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleValue> InverseLengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["InverseLength"]; }
    }

    private bool HasFixedInverseLengthParameter {
      get { return InverseLengthParameter.Value != null; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceSquaredExponentialIso(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceSquaredExponentialIso(CovarianceSquaredExponentialIso original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceSquaredExponentialIso()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the isometric squared exponential covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("InverseLength", "The inverse length parameter of the isometric squared exponential covariance function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSquaredExponentialIso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (HasFixedScaleParameter ? 0 : 1) +
        (HasFixedInverseLengthParameter ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double scale, inverseLength;
      GetParameterValues(p, out scale, out inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
      InverseLengthParameter.Value = new DoubleValue(inverseLength);
    }


    private void GetParameterValues(double[] p, out double scale, out double inverseLength) {
      // gather parameter values
      int c = 0;
      if (HasFixedInverseLengthParameter) {
        inverseLength = InverseLengthParameter.Value.Value;
      } else {
        inverseLength = 1.0 / Math.Exp(p[c]);
        c++;
      }

      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceSquaredExponentialIso", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double inverseLength, scale;
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
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, scale, inverseLength, columnIndices,
        fixedInverseLength, fixedScale);
      return cov;
    }

    // order of returned gradients must match the order in GetParameterValues!
    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double sf2, double inverseLength, IEnumerable<int> columnIndices, 
      bool fixedInverseLength, bool fixedScale) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);
      double g = Math.Exp(-d / 2.0);
      if (!fixedInverseLength) yield return sf2 * g * d;
      if (!fixedScale) yield return 2.0 * sf2 * g;
    }
  }
}
