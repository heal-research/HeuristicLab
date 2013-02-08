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
  [Item(Name = "CovariancePeriodic", Description = "Periodic covariance function for Gaussian processes.")]
  public sealed class CovariancePeriodic : ParameterizedNamedItem, ICovarianceFunction {

    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<DoubleValue> InverseLengthParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["InverseLength"]; }
    }

    public IValueParameter<DoubleValue> PeriodParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Period"]; }
    }


    [StorableConstructor]
    private CovariancePeriodic(bool deserializing) : base(deserializing) { }
    private CovariancePeriodic(CovariancePeriodic original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovariancePeriodic()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale of the periodic covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("InverseLength", "The inverse length parameter for the periodic covariance function."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Period", "The period parameter for the periodic covariance function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovariancePeriodic(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return (ScaleParameter.Value != null ? 0 : 1) +
       (PeriodParameter.Value != null ? 0 : 1) +
       (InverseLengthParameter.Value != null ? 0 : 1);
    }

    public void SetParameter(double[] p) {
      double scale, inverseLength, period;
      GetParameterValues(p, out scale, out period, out inverseLength);
      ScaleParameter.Value = new DoubleValue(scale);
      PeriodParameter.Value = new DoubleValue(period);
      InverseLengthParameter.Value = new DoubleValue(inverseLength);
    }


    private void GetParameterValues(double[] 
      p, out double scale, out double period, out double inverseLength) {
      // gather parameter values
      int c = 0;
      if (InverseLengthParameter.Value != null) {
        inverseLength = InverseLengthParameter.Value.Value;
      } else {
        inverseLength = 1.0 / Math.Exp(p[c]);
        c++;
      }
      if (PeriodParameter.Value != null) {
        period = PeriodParameter.Value.Value;
      } else {
        period = Math.Exp(p[c]);
        c++;
      }
      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancePeriodic", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double inverseLength, period, scale;
      GetParameterValues(p, out scale, out period, out inverseLength);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        double k = i == j ? 0.0 : GetDistance(x, x, i, j, columnIndices);
        k = Math.PI * k / period;
        k = Math.Sin(k) * inverseLength;
        k = k * k;

        return scale * Math.Exp(-2.0 * k);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        double k = GetDistance(x, xt, i, j, columnIndices);
        k = Math.PI * k / period;
        k = Math.Sin(k) * inverseLength;
        k = k * k;

        return scale * Math.Exp(-2.0 * k);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, period, inverseLength);
      return cov;
    }


    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices, double scale, double period, double inverseLength) {
      double k = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices) / period;
      double gradient = Math.Sin(k) * inverseLength;
      gradient *= gradient;
      yield return 4.0 * scale * Math.Exp(-2.0 * gradient) * gradient;
      double r = Math.Sin(k) * inverseLength;
      yield return 2.0 * k * scale * Math.Exp(-2 * r * r) *Math.Sin(2*k) * inverseLength * inverseLength;
      yield return 2.0 * scale * Math.Exp(-2 * gradient); 
      /*
       *  double s = Math.Log(scale) / 2.0;
      double l = Math.Log(1.0 / inverseLength);
      double p = Math.Log(period);

      double v;
      {
        v = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices) * Math.Exp(-p);
        double sin = Math.Sin(v);
        double sin2 = sin * sin;
        yield return 4.0 * sin2 * Math.Exp(-2.0 * Math.Exp(-2 * l) * sin2 - 2 * l + 2 * s);
      }
      {
        double pi_d = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices);
        double sin = Math.Sin(Math.Exp(-p)*pi_d);
        double sin2 = Math.Sin(2*Math.Exp(-p)*pi_d);
        yield return 2*pi_d*sin2*Math.Exp(-2*Math.Exp(-2*l)*sin*sin - 2*l - p + 2*s);
      }
      {
        double pi_d = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices);
        double sin = Math.Sin(Math.Exp(-p) * pi_d);
        yield return 2 * Math.Exp(2 * scale - 2 * Math.Exp(-2 * l) * sin * sin);
      }*/
    }

    private static double GetDistance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      return Math.Sqrt(Util.SqrDist(x, i, xt, j, 1, columnIndices));
    }
  }
}
