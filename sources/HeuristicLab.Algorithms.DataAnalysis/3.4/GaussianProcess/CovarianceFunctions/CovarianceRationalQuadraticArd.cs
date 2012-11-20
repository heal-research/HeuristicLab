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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceRationalQuadraticArd",
    Description = "Rational quadratic covariance function with automatic relevance determination for Gaussian processes.")]
  public sealed class CovarianceRationalQuadraticArd : ParameterizedNamedItem, ICovarianceFunction {
    [Storable]
    private double sf2;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return scaleParameter; }
    }

    [Storable]
    private double[] inverseLength;
    [Storable]
    private readonly HyperParameter<DoubleArray> inverseLengthParameter;
    public IValueParameter<DoubleArray> InverseLengthParameter {
      get { return inverseLengthParameter; }
    }

    [Storable]
    private double shape;
    [Storable]
    private readonly HyperParameter<DoubleValue> shapeParameter;
    public IValueParameter<DoubleValue> ShapeParameter {
      get { return shapeParameter; }
    }

    [StorableConstructor]
    private CovarianceRationalQuadraticArd(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceRationalQuadraticArd(CovarianceRationalQuadraticArd original, Cloner cloner)
      : base(original, cloner) {
      this.scaleParameter = cloner.Clone(original.scaleParameter);
      this.sf2 = original.sf2;

      this.inverseLengthParameter = cloner.Clone(original.inverseLengthParameter);
      if (original.inverseLength != null) {
        this.inverseLength = new double[original.inverseLength.Length];
        Array.Copy(original.inverseLength, inverseLength, inverseLength.Length);
      }

      this.shapeParameter = cloner.Clone(original.shapeParameter);
      this.shape = original.shape;

      RegisterEvents();
    }

    public CovarianceRationalQuadraticArd()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale parameter of the rational quadratic covariance function with ARD.");
      this.inverseLengthParameter = new HyperParameter<DoubleArray>("InverseLength", "The inverse length parameter for automatic relevance determination.");
      this.shapeParameter = new HyperParameter<DoubleValue>("Shape", "The shape parameter (alpha) of the rational quadratic covariance function with ARD.");

      Parameters.Add(scaleParameter);
      Parameters.Add(inverseLengthParameter);
      Parameters.Add(shapeParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRationalQuadraticArd(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { sf2 = scaleParameter.Value.Value; });
      Util.AttachValueChangeHandler<DoubleValue, double>(shapeParameter, () => { shape = shapeParameter.Value.Value; });
      Util.AttachArrayChangeHandler<DoubleArray, double>(inverseLengthParameter, () => { inverseLength = inverseLengthParameter.Value.ToArray(); });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (scaleParameter.Fixed ? 0 : 1) +
        (shapeParameter.Fixed ? 0 : 1) +
        (inverseLengthParameter.Fixed ? 0 : numberOfVariables);
    }

    public void SetParameter(double[] hyp) {
      int i = 0;
      if (!scaleParameter.Fixed) {
        scaleParameter.SetValue(new DoubleValue(Math.Exp(2 * hyp[i])));
        i++;
      }
      if (!shapeParameter.Fixed) {
        shapeParameter.SetValue(new DoubleValue(Math.Exp(hyp[i])));
        i++;
      }
      if (!inverseLengthParameter.Fixed) {
        inverseLengthParameter.SetValue(new DoubleArray(hyp.Skip(i).Select(e => 1.0 / Math.Exp(e)).ToArray()));
        i += hyp.Skip(i).Count();
      }
      if (hyp.Length != i) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceRationalQuadraticArd", "hyp");
    }


    public double GetCovariance(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);
      return sf2 * Math.Pow(1 + 0.5 * d / shape, -shape);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength, columnIndices);
      double b = 1 + 0.5 * d / shape;
      foreach (var columnIndex in columnIndices) {
        yield return sf2 * Math.Pow(b, -shape - 1) * Util.SqrDist(x[i, columnIndex] * inverseLength[columnIndex], x[j, columnIndex] * inverseLength[columnIndex]);
      }
      yield return 2 * sf2 * Math.Pow(b, -shape);
      yield return sf2 * Math.Pow(b, -shape) * (0.5 * d / b - shape * Math.Log(b));
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      double d = Util.SqrDist(x, i, xt, j, inverseLength, columnIndices);
      return sf2 * Math.Pow(1 + 0.5 * d / shape, -shape);
    }
  }
}
