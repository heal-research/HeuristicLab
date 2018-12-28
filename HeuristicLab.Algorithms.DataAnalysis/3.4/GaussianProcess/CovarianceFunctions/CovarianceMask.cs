#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("21D5F261-B9DF-4DC4-AF42-7B223C45840F")]
  [Item(Name = "CovarianceMask",
    Description = "Masking covariance function for dimension selection can be used to apply a covariance function only on certain input dimensions.")]
  public sealed class CovarianceMask : ParameterizedNamedItem, ICovarianceFunction {
    public IValueParameter<IntArray> SelectedDimensionsParameter {
      get { return (IValueParameter<IntArray>)Parameters["SelectedDimensions"]; }
    }
    public IValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return (IValueParameter<ICovarianceFunction>)Parameters["CovarianceFunction"]; }
    }

    [StorableConstructor]
    private CovarianceMask(StorableConstructorFlag _) : base(_) {
    }

    private CovarianceMask(CovarianceMask original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceMask()
      : base() {
      Name = ItemName;
      Description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<IntArray>("SelectedDimensions", "The dimensions on which the specified covariance function should be applied to."));
      Parameters.Add(new ValueParameter<ICovarianceFunction>("CovarianceFunction", "The covariance function that should be scaled.", new CovarianceSquaredExponentialIso()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceMask(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      if (SelectedDimensionsParameter.Value == null) return CovarianceFunctionParameter.Value.GetNumberOfParameters(numberOfVariables);
      else return CovarianceFunctionParameter.Value.GetNumberOfParameters(SelectedDimensionsParameter.Value.Length);
    }

    public void SetParameter(double[] p) {
      CovarianceFunctionParameter.Value.SetParameter(p);
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      var cov = CovarianceFunctionParameter.Value;
      var selectedDimensions = SelectedDimensionsParameter.Value;

      return cov.GetParameterizedCovarianceFunction(p, selectedDimensions.Intersect(columnIndices).ToArray());
    }
  }
}
