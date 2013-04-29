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
using System.Linq.Expressions;
using AutoDiff;
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
        (ScaleParameter.Value != null ? 0 : 1) +
        (LengthParameter.Value != null ? 0 : 1);
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
      if (LengthParameter.Value != null) {
        length = LengthParameter.Value.Value;
      } else {
        length = Math.Exp(2 * p[c]);
        c++;
      }

      if (ScaleParameter.Value != null) {
        scale = ScaleParameter.Value.Value;
      } else {
        scale = Math.Exp(2 * p[c]);
        c++;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceNeuralNetwork", "p");
    }


    private static Func<Term, UnaryFunc> asin = UnaryFunc.Factory(
        x => Math.Asin(x),      // evaluate
        x => 1 / Math.Sqrt(1 - x * x));  // derivative of atan
    private static Func<Term, UnaryFunc> sqrt = UnaryFunc.Factory(
      x => Math.Sqrt(x),
      x => 1 / (2 * Math.Sqrt(x)));

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, IEnumerable<int> columnIndices) {
      double length, scale;
      GetParameterValues(p, out scale, out length);
      // create functions
      AutoDiff.Variable p0 = new AutoDiff.Variable();
      AutoDiff.Variable p1 = new AutoDiff.Variable();
      var l = TermBuilder.Exp(2.0 * p0);
      var s = TermBuilder.Exp(2.0 * p1);
      AutoDiff.Variable[] x1 = new AutoDiff.Variable[columnIndices.Count()];
      AutoDiff.Variable[] x2 = new AutoDiff.Variable[columnIndices.Count()];
      AutoDiff.Term sx = 1;
      AutoDiff.Term s1 = 1;
      AutoDiff.Term s2 = 1;
      foreach (var k in columnIndices) {
        x1[k] = new AutoDiff.Variable();
        x2[k] = new AutoDiff.Variable();
        sx += x1[k] * x2[k];
        s1 += x1[k] * x1[k];
        s2 += x2[k] * x2[k];
      }

      var parameter = x1.Concat(x2).Concat(new AutoDiff.Variable[] { p0, p1 }).ToArray();
      var values = new double[x1.Length + x2.Length + 2];
      var c = (s * asin(sx / (sqrt((l + s1) * (l + s2))))).Compile(parameter);

      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        int k = 0;
        foreach (var col in columnIndices) {
          values[k] = x[i, col];
          k++;
        }
        foreach (var col in columnIndices) {
          values[k] = x[j, col];
          k++;
        }
        values[k] = Math.Log(Math.Sqrt(length));
        values[k + 1] = Math.Log(Math.Sqrt(scale));
        return c.Evaluate(values);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        int k = 0;
        foreach (var col in columnIndices) {
          values[k] = x[i, col];
          k++;
        }
        foreach (var col in columnIndices) {
          values[k] = xt[j, col];
          k++;
        }
        values[k] = Math.Log(Math.Sqrt(length));
        values[k + 1] = Math.Log(Math.Sqrt(scale));
        return c.Evaluate(values);
      };
      cov.CovarianceGradient = (x, i, j) => {
        int k = 0;
        foreach (var col in columnIndices) {
          values[k] = x[i, col];
          k++;
        }
        foreach (var col in columnIndices) {
          values[k] = x[j, col];
          k++;
        }
        values[k] = Math.Log(Math.Sqrt(length));
        values[k + 1] = Math.Log(Math.Sqrt(scale));
        return c.Differentiate(values).Item1.Skip(columnIndices.Count() * 2);
      };
      return cov;
    }

  }
}
