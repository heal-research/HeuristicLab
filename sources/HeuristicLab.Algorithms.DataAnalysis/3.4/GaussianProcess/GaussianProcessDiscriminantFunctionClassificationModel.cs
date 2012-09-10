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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a Gaussian process model.
  /// </summary>
  [StorableClass]
  [Item("GaussianProcessDiscriminantFunctionClassificationModel",
    "Represents a Gaussian process discriminant function classification model.")]
  public sealed class GaussianProcessDiscriminantFunctionClassificationModel : DiscriminantFunctionClassificationModel {
    [StorableConstructor]
    private GaussianProcessDiscriminantFunctionClassificationModel(bool deserializing)
      : base(deserializing) {
    }

    private GaussianProcessDiscriminantFunctionClassificationModel(
      GaussianProcessDiscriminantFunctionClassificationModel original, Cloner cloner)
      : base(original, cloner) {
    }

    public GaussianProcessDiscriminantFunctionClassificationModel(IGaussianProcessModel model, IDiscriminantFunctionThresholdCalculator thresholdCalculator)
      : base(model, thresholdCalculator) {
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessDiscriminantFunctionClassificationModel(this, cloner);
    }


    public override IDiscriminantFunctionClassificationSolution CreateDiscriminantFunctionClassificationSolution(IClassificationProblemData problemData) {
      return new GaussianProcessDiscriminantFunctionClassificationSolution(this, problemData);
    }

    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateDiscriminantFunctionClassificationSolution(problemData);
    }
  }
}
