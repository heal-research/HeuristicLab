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
  [Item(Name = "CovarianceConst",
    Description = "Constant covariance function for Gaussian processes.")]
  public sealed class CovarianceConst : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Scale"]; }
    }
    private bool HasFixedScaleParameter {
      get { return ScaleParameter.Value != null; }
    }
    [StorableConstructor]
    private CovarianceConst(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceConst(CovarianceConst original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceConst()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Scale", "The scale of the constant covariance function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceConst(this, cloner);
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
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceConst", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double scale;
      GetParameterValues(p, out scale);
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => scale;
      cov.CrossCovariance = (x, xt, i, j) => scale;
      if (HasFixedScaleParameter) {
        cov.CovarianceGradient = (x, i, j) => Enumerable.Empty<double>();
      } else {
        cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, scale, columnIndices);
      }
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double scale, IEnumerable<int> columnIndices) {
      yield return 2.0 * scale;
    }
  }
}
