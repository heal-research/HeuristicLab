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
  [Item(Name = "CovarianceNoise",
    Description = "Noise covariance function for Gaussian processes.")]
  public sealed class CovarianceNoise : ParameterizedNamedItem, ICovarianceFunction {


    [Storable]
    private double sf2;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return scaleParameter; }
    }

    [StorableConstructor]
    private CovarianceNoise(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceNoise(CovarianceNoise original, Cloner cloner)
      : base(original, cloner) {
      this.scaleParameter = cloner.Clone(original.scaleParameter);
      this.sf2 = original.sf2;
      RegisterEvents();
    }

    public CovarianceNoise()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale of noise.");
      Parameters.Add(this.scaleParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceNoise(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { sf2 = scaleParameter.Value.Value; });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return scaleParameter.Fixed ? 0 : 1;
    }

    public void SetParameter(double[] hyp) {
      if (!scaleParameter.Fixed) {
        scaleParameter.SetValue(new DoubleValue(Math.Exp(2 * hyp[0])));
      } else {
        if (hyp.Length > 0) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceNoise", "hyp");
      }
    }

    public double GetCovariance(double[,] x, int i, int j) {
      return sf2;
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      yield return 2 * sf2;
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      return 0.0;
    }
  }
}
