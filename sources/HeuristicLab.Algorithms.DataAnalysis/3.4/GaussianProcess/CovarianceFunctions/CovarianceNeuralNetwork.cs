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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceNeuralNetwork",
    Description = "Neural network covariance function for Gaussian processes.")]
  public sealed class CovarianceNeuralNetwork : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleValue> LengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Length"]; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }
    private bool HasFixedLengthParameter {
      get { return LengthParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceNeuralNetwork(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceNeuralNetwork(CovarianceNeuralNetwork original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceNeuralNetwork()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Length", "The length parameter."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceNeuralNetwork(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (HasFixedScaleParameter ? 0 : 1) +
        (HasFixedLengthParameter ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double scale, length;
      GetParameterValues(p, out scale, out length);
      ScaleParameter.Value = new DoubleValue(scale);
      LengthParameter.Value = new DoubleValue(length);
    }


    private void GetParameterValues(double[] p, out double scale, out double length) {
      // gather parameter values
      int c = 0;
      if (HasFixedLengthParameter) {
        length = LengthParameter.Value.Value;
      } else {
        length = Math.Exp(2 * p[c]);
        c++;
      }

      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceNeuralNetwork", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double length, scale;
      GetParameterValues(p, out scale, out length);
      var fixedLength = HasFixedLengthParameter;
      var fixedScale = HasFixedScaleParameter;

      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double sx = 1.0;
        double s1 = 1.0;
        double s2 = 1.0;
        for (int c = 0; c < columnIndices.Length; c++) {
          var col = columnIndices[c];
          sx += x[i, col] * x[j, col];
          s1 += x[i, col] * x[i, col];
          s2 += x[j, col] * x[j, col];
        }

        return (scale * Math.Asin(sx / (Math.Sqrt((length + s1) * (length + s2)))));
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double sx = 1.0;
        double s1 = 1.0;
        double s2 = 1.0;
        for (int c = 0; c < columnIndices.Length; c++) {
          var col = columnIndices[c];
          sx += x[i, col] * xt[j, col];
          s1 += x[i, col] * x[i, col];
          s2 += xt[j, col] * xt[j, col];
        }

        return (scale * Math.Asin(sx / (Math.Sqrt((length + s1) * (length + s2)))));
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, length, scale, columnIndices, fixedLength, fixedScale);
      return cov;
    }

    // order of returned gradients must match the order in GetParameterValues!
    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double length, double scale, int[] columnIndices,
      bool fixedLength, bool fixedScale) {
      {
        double sx = 1.0;
        double s1 = 1.0;
        double s2 = 1.0;
        for (int c = 0; c < columnIndices.Length; c++) {
          var col = columnIndices[c];
          sx += x[i, col] * x[j, col];
          s1 += x[i, col] * x[i, col];
          s2 += x[j, col] * x[j, col];
        }
        var h = (length + s1) * (length + s2);
        var f = sx / Math.Sqrt(h);
        if (!fixedLength) {
          yield return -scale / Math.Sqrt(1.0 - f * f) * ((length * sx * (2.0 * length + s1 + s2)) / Math.Pow(h, 3.0 / 2.0));
        }
        if (!fixedScale) {
          yield return 2.0 * scale * Math.Asin(f);
        }
      }
    }
  }
}
