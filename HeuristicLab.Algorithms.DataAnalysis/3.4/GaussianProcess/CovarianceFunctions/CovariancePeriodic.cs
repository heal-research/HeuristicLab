#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("198B5472-6CAA-4C39-BB1F-6EC16CB7801B")]
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

    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }
    private bool HasFixedInverseLengthParameter {
      get { return InverseLengthParameter.Value != null; }
    }
    private bool HasFixedPeriodParameter {
      get { return PeriodParameter.Value != null; }
    }


    [StorableConstructor]
    private CovariancePeriodic(StorableConstructorFlag _) : base(_) { }
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
      return (HasFixedScaleParameter ? 0 : 1) +
       (HasFixedPeriodParameter ? 0 : 1) +
       (HasFixedInverseLengthParameter ? 0 : 1);
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
      if (HasFixedInverseLengthParameter) {
        inverseLength = InverseLengthParameter.Value.Value;
      } else {
        inverseLength = 1.0 / Math.Exp(p[c]);
        c++;
      }
      if (HasFixedPeriodParameter) {
        period = PeriodParameter.Value.Value;
      } else {
        period = Math.Exp(p[c]);
        c++;
      }
      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancePeriodic", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double inverseLength, period, scale;
      GetParameterValues(p, out scale, out period, out inverseLength);
      var fixedInverseLength = HasFixedInverseLengthParameter;
      var fixedPeriod = HasFixedPeriodParameter;
      var fixedScale = HasFixedScaleParameter;
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
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, period, inverseLength, fixedInverseLength, fixedPeriod, fixedScale);
      return cov;
    }

    private static IList<double> GetGradient(double[,] x, int i, int j, int[] columnIndices, double scale, double period, double inverseLength,
      bool fixedInverseLength, bool fixedPeriod, bool fixedScale) {
      double k = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices) / period;
      double gradient = Math.Sin(k) * inverseLength;
      gradient *= gradient;
      var g = new List<double>(3);
      if (!fixedInverseLength) 
        g.Add(4.0 * scale * Math.Exp(-2.0 * gradient) * gradient);
      if (!fixedPeriod) {
        double r = Math.Sin(k) * inverseLength;
        g.Add(2.0 * k * scale * Math.Exp(-2 * r * r) * Math.Sin(2 * k) * inverseLength * inverseLength);
      }
      if (!fixedScale)
        g.Add(2.0 * scale * Math.Exp(-2 * gradient));
      return g;
    }

    private static double GetDistance(double[,] x, double[,] xt, int i, int j, int[] columnIndices) {
      return Math.Sqrt(Util.SqrDist(x, i, xt, j, columnIndices, 1));
    }
  }
}
