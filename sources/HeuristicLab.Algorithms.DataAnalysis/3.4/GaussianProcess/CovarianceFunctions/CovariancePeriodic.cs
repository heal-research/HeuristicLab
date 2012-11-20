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
  [Item(Name = "CovariancePeriodic", Description = "Periodic covariance function for Gaussian processes.")]
  public sealed class CovariancePeriodic : ParameterizedNamedItem, ICovarianceFunction {

    [Storable]
    private double scale;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return scaleParameter; }
    }

    [Storable]
    private double inverseLength;
    [Storable]
    private readonly HyperParameter<DoubleValue> inverseLengthParameter;
    public IValueParameter<DoubleValue> InverseLengthParameter {
      get { return inverseLengthParameter; }
    }

    [Storable]
    private double period;
    [Storable]
    private readonly HyperParameter<DoubleValue> periodParameter;
    public IValueParameter<DoubleValue> PeriodParameter {
      get { return periodParameter; }
    }


    [StorableConstructor]
    private CovariancePeriodic(bool deserializing) : base(deserializing) { }
    private CovariancePeriodic(CovariancePeriodic original, Cloner cloner)
      : base(original, cloner) {
      this.scaleParameter = cloner.Clone(original.scaleParameter);
      this.inverseLengthParameter = cloner.Clone(original.inverseLengthParameter);
      this.periodParameter = cloner.Clone(original.periodParameter);
      this.scale = original.scale;
      this.inverseLength = original.inverseLength;
      this.period = original.period;

      RegisterEvents();
    }

    public CovariancePeriodic()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale of the periodic covariance function.");
      inverseLengthParameter = new HyperParameter<DoubleValue>("InverseLength", "The inverse length parameter for the periodic covariance function.");
      periodParameter = new HyperParameter<DoubleValue>("Period", "The period parameter for the periodic covariance function.");
      Parameters.Add(scaleParameter);
      Parameters.Add(inverseLengthParameter);
      Parameters.Add(periodParameter);

      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovariancePeriodic(this, cloner);
    }

    // caching
    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { scale = scaleParameter.Value.Value; });
      Util.AttachValueChangeHandler<DoubleValue, double>(inverseLengthParameter, () => { inverseLength = inverseLengthParameter.Value.Value; });
      Util.AttachValueChangeHandler<DoubleValue, double>(periodParameter, () => { period = periodParameter.Value.Value; });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return
        (new[] { scaleParameter, inverseLengthParameter, periodParameter }).Count(p => !p.Fixed);
    }

    public void SetParameter(double[] hyp) {
      int i = 0;
      if (!inverseLengthParameter.Fixed) {
        inverseLengthParameter.SetValue(new DoubleValue(1.0 / Math.Exp(hyp[i])));
        i++;
      }
      if (!periodParameter.Fixed) {
        periodParameter.SetValue(new DoubleValue(Math.Exp(hyp[i])));
        i++;
      }
      if (!scaleParameter.Fixed) {
        scaleParameter.SetValue(new DoubleValue(Math.Exp(2 * hyp[i])));
        i++;
      }
      if (hyp.Length != i) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovariancePeriod", "hyp");
    }

    public double GetCovariance(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      double k = i == j ? 0.0 : GetDistance(x, x, i, j, columnIndices);
      k = Math.PI * k / period;
      k = Math.Sin(k) * inverseLength;
      k = k * k;

      return scale * Math.Exp(-2.0 * k);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      double v = i == j ? 0.0 : Math.PI * GetDistance(x, x, i, j, columnIndices) / period;
      double gradient = Math.Sin(v) * inverseLength;
      gradient *= gradient;
      yield return 4.0 * scale * Math.Exp(-2.0 * gradient) * gradient;
      double r = Math.Sin(v) * inverseLength;
      yield return 4.0 * scale * inverseLength * Math.Exp(-2 * r * r) * r * Math.Cos(v) * v;
      yield return 2.0 * scale * Math.Exp(-2 * gradient);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      double k = GetDistance(x, xt, i, j, columnIndices);
      k = Math.PI * k / period;
      k = Math.Sin(k) * inverseLength;
      k = k * k;

      return scale * Math.Exp(-2.0 * k);
    }

    private double GetDistance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      return Math.Sqrt(Util.SqrDist(x, i, xt, j, 1, columnIndices));
    }
  }
}
