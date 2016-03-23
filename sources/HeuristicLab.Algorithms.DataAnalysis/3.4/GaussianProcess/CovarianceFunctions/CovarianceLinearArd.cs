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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceLinearArd",
    Description = "Linear covariance function with automatic relevance determination for Gaussian processes.")]
  public sealed class CovarianceLinearArd : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<DoubleArray> InverseLengthParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["InverseLength"]; }
    }
    private bool HasFixedInverseLengthParameter {
      get { return InverseLengthParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceLinearArd(bool deserializing) : base(deserializing) { }
    private CovarianceLinearArd(CovarianceLinearArd original, Cloner cloner)
      : base(original, cloner) {
    }
    public CovarianceLinearArd()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleArray>("InverseLength",
                                                             "The inverse length parameter for ARD."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinearArd(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      if (HasFixedInverseLengthParameter)
        return 0;
      else
        return numberOfVariables;
    }

    public void SetParameter(double[] p) {
      double[] inverseLength;
      GetParameterValues(p, out inverseLength);
      InverseLengthParameter.Value = new DoubleArray(inverseLength);
    }

    private void GetParameterValues(double[] p, out double[] inverseLength) {
      // gather parameter values
      if (HasFixedInverseLengthParameter) {
        inverseLength = InverseLengthParameter.Value.ToArray();
      } else {
        inverseLength = p.Select(e => 1.0 / Math.Exp(e)).ToArray();
      }
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double[] inverseLength;
      GetParameterValues(p, out inverseLength);
      var fixedInverseLength = HasFixedInverseLengthParameter;
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => Util.ScalarProd(x, i, j, inverseLength, columnIndices);
      cov.CrossCovariance = (x, xt, i, j) => Util.ScalarProd(x, i, xt, j, inverseLength, columnIndices);
      if (fixedInverseLength)
        cov.CovarianceGradient = (x, i, j) => Enumerable.Empty<double>();
      else
        cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, inverseLength, columnIndices);
      return cov;
    }

    private static IEnumerable<double> GetGradient(double[,] x, int i, int j, double[] inverseLength, int[] columnIndices) {
      int k = 0;
      for (int c = 0; c < columnIndices.Length; c++) {
        var columnIndex = columnIndices[c];
        yield return -2.0 * x[i, columnIndex] * x[j, columnIndex] * inverseLength[k] * inverseLength[k];
        k++;
      }
    }
  }
}
