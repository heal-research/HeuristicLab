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
  [Item(Name = "CovarianceScale",
    Description = "Scale covariance function for Gaussian processes.")]
  public sealed class CovarianceScale : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }

    public IValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return (IValueParameter<ICovarianceFunction>)Parameters["CovarianceFunction"]; }
    }

    [StorableConstructor]
    private CovarianceScale(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceScale(CovarianceScale original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceScale()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale parameter."));
      Parameters.Add(new ValueParameter<ICovarianceFunction>("CovarianceFunction", "The covariance function that should be scaled.", new CovarianceSquaredExponentialIso()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceScale(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return (ScaleParameter.Value != null ? 0 : 1) + CovarianceFunctionParameter.Value.GetNumberOfParameters(numberOfVariables);
    }

    public void SetParameter(double[] p) {
      double scale;
      GetParameterValues(p, out scale);
      ScaleParameter.Value = new DoubleValue(scale);
      CovarianceFunctionParameter.Value.SetParameter(p.Skip(1).ToArray());
    }

    private void GetParameterValues(double[] p, out double scale) {
      // gather parameter values
      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[0]);
      }
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double scale;
      GetParameterValues(p, out scale);
      var subCov = CovarianceFunctionParameter.Value.GetParameterizedCovarianceFunction(p.Skip(1).ToArray(), columnIndices);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => scale * subCov.Covariance(x, i, j);
      cov.CrossCovariance = (x, xt, i, j) => scale * subCov.CrossCovariance(x, xt, i, j);
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, columnIndices, scale, subCov);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices, double scale, ParameterizedCovarianceFunction cov) {
      yield return 2 * scale * cov.Covariance(x, i, j);
      foreach (var g in cov.CovarianceGradient(x, i, j))
        yield return scale * g;
    }
  }
}
