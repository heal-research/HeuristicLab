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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceRationalQuadraticIso",
    Description = "Isotropic rational quadratic covariance function for Gaussian processes.")]
  public sealed class CovarianceRationalQuadraticIso : ParameterizedNamedItem, ICovarianceFunction {
    [Storable]
    private double sf2;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter { get { return scaleParameter; } }

    [Storable]
    private double inverseLength;
    [Storable]
    private readonly HyperParameter<DoubleValue> inverseLengthParameter;
    public IValueParameter<DoubleValue> InverseLengthParameter { get { return inverseLengthParameter; } }

    [Storable]
    private double shape;
    [Storable]
    private readonly HyperParameter<DoubleValue> shapeParameter;
    public IValueParameter<DoubleValue> ShapeParameter { get { return shapeParameter; } }

    [StorableConstructor]
    private CovarianceRationalQuadraticIso(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceRationalQuadraticIso(CovarianceRationalQuadraticIso original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.scaleParameter = cloner.Clone(original.scaleParameter);

      this.inverseLength = original.inverseLength;
      this.inverseLengthParameter = cloner.Clone(original.inverseLengthParameter);

      this.shape = original.shape;
      this.shapeParameter = cloner.Clone(original.shapeParameter);

      RegisterEvents();
    }

    public CovarianceRationalQuadraticIso()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale parameter of the isometric rational quadratic covariance function.");
      this.inverseLengthParameter = new HyperParameter<DoubleValue>("InverseLength", "The inverse length parameter of the isometric rational quadratic covariance function.");
      this.shapeParameter = new HyperParameter<DoubleValue>("Shape", "The shape parameter (alpha) of the isometric rational quadratic covariance function.");

      Parameters.Add(scaleParameter);
      Parameters.Add(inverseLengthParameter);
      Parameters.Add(shapeParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceRationalQuadraticIso(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { sf2 = scaleParameter.Value.Value; });
      Util.AttachValueChangeHandler<DoubleValue, double>(inverseLengthParameter, () => { inverseLength = inverseLengthParameter.Value.Value; });
      Util.AttachValueChangeHandler<DoubleValue, double>(shapeParameter, () => { shape = shapeParameter.Value.Value; });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (scaleParameter.Fixed ? 0 : 1) +
        (inverseLengthParameter.Fixed ? 0 : 1) +
        (shapeParameter.Fixed ? 0 : 1);
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
        inverseLengthParameter.SetValue(new DoubleValue(1.0 / Math.Exp(hyp[i])));
        i++;
      }
      if (hyp.Length != i) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceRationalQuadraticIso", "hyp");
    }


    public double GetCovariance(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);
      return sf2 * Math.Pow(1 + 0.5 * d / shape, -shape);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double d = i == j
                   ? 0.0
                   : Util.SqrDist(x, i, j, inverseLength);

      double b = 1 + 0.5 * d / shape;
      yield return sf2 * Math.Pow(b, -shape - 1) * d;
      yield return 2 * sf2 * Math.Pow(b, -shape);
      yield return sf2 * Math.Pow(b, -shape) * (0.5 * d / b - shape * Math.Log(b));
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double d = Util.SqrDist(x, i, xt, j, inverseLength);
      return sf2 * Math.Pow(1 + 0.5 * d / shape, -shape);
    }
  }
}
