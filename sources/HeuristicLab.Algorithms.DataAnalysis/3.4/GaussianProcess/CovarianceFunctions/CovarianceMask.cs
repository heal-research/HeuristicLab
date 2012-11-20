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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceMask",
    Description = "Masking covariance function for dimension selection can be used to apply a covariance function only on certain input dimensions.")]
  public sealed class CovarianceMask : ParameterizedNamedItem, ICovarianceFunction {
    [Storable]
    private int[] selectedDimensions;
    [Storable]
    private readonly ValueParameter<IntArray> selectedDimensionsParameter;
    public IValueParameter<IntArray> SelectedDimensionsParameter {
      get { return selectedDimensionsParameter; }
    }

    [Storable]
    private ICovarianceFunction cov;
    [Storable]
    private readonly ValueParameter<ICovarianceFunction> covParameter;
    public IValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return covParameter; }
    }

    [StorableConstructor]
    private CovarianceMask(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceMask(CovarianceMask original, Cloner cloner)
      : base(original, cloner) {
      this.selectedDimensionsParameter = cloner.Clone(original.selectedDimensionsParameter);
      if (original.selectedDimensions != null) {
        this.selectedDimensions = (int[])original.selectedDimensions.Clone();
      }

      this.covParameter = cloner.Clone(original.covParameter);
      this.cov = cloner.Clone(original.cov);
      RegisterEvents();
    }

    public CovarianceMask()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      this.selectedDimensionsParameter = new ValueParameter<IntArray>("SelectedDimensions", "The dimensions on which the specified covariance function should be applied to.");
      this.covParameter = new ValueParameter<ICovarianceFunction>("CovarianceFunction", "The covariance function that should be scaled.", new CovarianceSquaredExponentialIso());
      cov = covParameter.Value;

      Parameters.Add(selectedDimensionsParameter);
      Parameters.Add(covParameter);

      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceMask(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachArrayChangeHandler<IntArray, int>(selectedDimensionsParameter, () => {
        selectedDimensions = selectedDimensionsParameter.Value
          .OrderBy(x => x)
          .Distinct()
          .ToArray();
        if (selectedDimensions.Length == 0) selectedDimensions = null;
      });
      covParameter.ValueChanged += (sender, args) => { cov = covParameter.Value; };
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      if (selectedDimensions == null) return cov.GetNumberOfParameters(numberOfVariables);
      else return cov.GetNumberOfParameters(selectedDimensions.Length);
    }

    public void SetParameter(double[] hyp) {
      cov.SetParameter(hyp);
    }

    public double GetCovariance(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      return cov.GetCovariance(x, i, j, selectedDimensions);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j, IEnumerable<int> columnIndices) {
      return cov.GetGradient(x, i, j, selectedDimensions);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j, IEnumerable<int> columnIndices) {
      return cov.GetCrossCovariance(x, xt, i, j, selectedDimensions);
    }
  }
}
