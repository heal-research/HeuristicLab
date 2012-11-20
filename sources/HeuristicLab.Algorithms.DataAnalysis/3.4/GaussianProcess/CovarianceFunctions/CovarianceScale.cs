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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceScale",
    Description = "Scale covariance function for Gaussian processes.")]
  public sealed class CovarianceScale : ParameterizedNamedItem, ICovarianceFunction {
    [Storable]
    private double sf2;
    [Storable]
    private readonly HyperParameter<DoubleValue> scaleParameter;
    public IValueParameter<DoubleValue> ScaleParameter {
      get { return scaleParameter; }
    }

    [Storable]
    private ICovarianceFunction cov;
    [Storable]
    private readonly ValueParameter<ICovarianceFunction> covParameter;
    public IValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return covParameter; }
    }

    [StorableConstructor]
    private CovarianceScale(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceScale(CovarianceScale original, Cloner cloner)
      : base(original, cloner) {
      this.scaleParameter = cloner.Clone(original.scaleParameter);
      this.sf2 = original.sf2;

      this.covParameter = cloner.Clone(original.covParameter);
      this.cov = cloner.Clone(original.cov);
      RegisterEvents();
    }

    public CovarianceScale()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.scaleParameter = new HyperParameter<DoubleValue>("Scale", "The scale parameter.");
      this.covParameter = new ValueParameter<ICovarianceFunction>("CovarianceFunction", "The covariance function that should be scaled.", new CovarianceSquaredExponentialIso());
      cov = covParameter.Value;

      Parameters.Add(this.scaleParameter);
      Parameters.Add(covParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceScale(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachValueChangeHandler<DoubleValue, double>(scaleParameter, () => { sf2 = scaleParameter.Value.Value; });
      covParameter.ValueChanged += (sender, args) => { cov = covParameter.Value; };
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return (scaleParameter.Fixed ? 0 : 1) + cov.GetNumberOfParameters(numberOfVariables);
    }

    public void SetParameter(double[] hyp) {
      int i = 0;
      if (!scaleParameter.Fixed) {
        scaleParameter.SetValue(new DoubleValue(Math.Exp(2 * hyp[i])));
        i++;
      }
      cov.SetParameter(hyp.Skip(i).ToArray());
    }

    public double GetCovariance(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      return sf2 * cov.GetCovariance(x, i, j, columnIndices);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      yield return 2 * sf2 * cov.GetCovariance(x, i, j, columnIndices);
      foreach (var g in cov.GetGradient(x, i, j, columnIndices))
        yield return sf2 * g;
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      return sf2 * cov.GetCrossCovariance(x, xt, i, j, columnIndices);
    }
  }
}
