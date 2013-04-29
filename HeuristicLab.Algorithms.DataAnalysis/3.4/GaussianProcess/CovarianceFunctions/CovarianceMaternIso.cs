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
  [Item(Name = "CovarianceMaternIso",
    Description = "Matern covariance function for Gaussian processes.")]
  public sealed class CovarianceMaternIso : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> InverseLengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["InverseLength"]; }
    }

    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IConstrainedValueParameter<IntValue> DParameter {
      get { return (IConstrainedValueParameter<IntValue>)Parameters["D"]; }
    }


    [StorableConstructor]
    private CovarianceMaternIso(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceMaternIso(CovarianceMaternIso original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceMaternIso()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("InverseLength", "The inverse length parameter of the isometric Matern covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter of the isometric Matern covariance function."));
      var validDValues = new ItemSet<IntValue>();
      validDValues.Add((IntValue)new IntValue(1).AsReadOnly());
      validDValues.Add((IntValue)new IntValue(3).AsReadOnly());
      validDValues.Add((IntValue)new IntValue(5).AsReadOnly());
      Parameters.Add(new ConstrainedValueParameter<IntValue>("D", "The d parameter (allowed values: 1, 3, or 5) of the isometric Matern covariance function.", validDValues, validDValues.First()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceMaternIso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (InverseLengthParameter.Value != null ? 0 : 1) +
        (ScaleParameter.Value != null ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double inverseLength, scale;
      GetParameterValues(p, out scale, out inverseLength);
      InverseLengthParameter.Value = new DoubleValue(inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
    }

    private void GetParameterValues(double[] p, out double scale, out double inverseLength) {
      // gather parameter values
      int c = 0;
      if (InverseLengthParameter.Value != null) {
        inverseLength = InverseLengthParameter.Value.Value;
      } else {
        inverseLength = 1.0 / Math.Exp(p[c]);
        c++;
      }

      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceMaternIso", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double inverseLength, scale;
      int d = DParameter.Value.Value;
      GetParameterValues(p, out scale, out inverseLength);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double dist = i == j
                       ? 0.0
                       : Math.Sqrt(Util.SqrDist(x, i, j, Math.Sqrt(d) * inverseLength, columnIndices));
        return scale * m(d, dist);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double dist = Math.Sqrt(Util.SqrDist(x, i, xt, j, Math.Sqrt(d) * inverseLength, columnIndices));
        return scale * m(d, dist);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, d, scale, inverseLength, columnIndices);
      return cov;
    }

    private static double m(int d, double t) {
      double f;
      switch (d) {
        case 1: { f = 1; break; }
        case 3: { f = 1 + t; break; }
        case 5: { f = 1 + t * (1 + t / 3.0); break; }
        default: throw new InvalidOperationException();
      }
      return f * Math.Exp(-t);
    }

    private static double dm(int d, double t) {
      double df;
      switch (d) {
        case 1: { df = 1; break; }
        case 3: { df = t; break; }
        case 5: { df = t * (1 + t) / 3.0; break; }
        default: throw new InvalidOperationException();
      }
      return df * t * Math.Exp(-t);
    }


    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, int d, double scale, double inverseLength, IEnumerable<int> columnIndices) {
      double dist = i == j
                   ? 0.0
                   : Math.Sqrt(Util.SqrDist(x, i, j, Math.Sqrt(d) * inverseLength, columnIndices));

      yield return scale * dm(d, dist);
      yield return 2 * scale * m(d, dist);
    }
  }
}
