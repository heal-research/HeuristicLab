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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("C6AEEC11-1F8D-40D1-8D8A-DCCCE886E46C")]
  [Item(Name = "CovarianceNoise",
    Description = "Noise covariance function for Gaussian processes.")]
  public sealed class CovarianceNoise : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceNoise(StorableConstructorFlag _) : base(_) {
    }

    private CovarianceNoise(CovarianceNoise original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceNoise()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale of noise."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceNoise(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return HasFixedScaleParameter ? 0 : 1;
    }

    public void SetParameter(double[] p) {
      double scale;
      GetParameterValues(p, out scale);
      ScaleParameter.Value = new DoubleValue(scale);
    }

    private void GetParameterValues(double[] p, out double scale) {
      int c = 0;
      // gather parameter values
      if (HasFixedScaleParameter) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceNoise", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double scale;
      GetParameterValues(p, out scale);
      var fixedScale = HasFixedScaleParameter;
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => i == j ? scale : 0.0;
      cov.CrossCovariance = (x, xt, i, j) => Util.SqrDist(x, i, xt, j, columnIndices, 1.0) < 1e-9 ? scale : 0.0;
      if (fixedScale)
        cov.CovarianceGradient = (x, i, j) => new double[0];
      else
        cov.CovarianceGradient = (x, i, j) => new double[1] { i == j ? 2.0 * scale : 0.0 };
      return cov;
    }
  }
}
