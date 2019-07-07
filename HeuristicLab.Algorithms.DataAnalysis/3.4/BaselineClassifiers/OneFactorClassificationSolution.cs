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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("7A180AA0-E963-4714-A33A-2C87C4108B93")]
  [Item(Name = "OneR Classification Solution", Description = "Represents a OneR classification solution which uses only a single factor for class prediction.")]
  public sealed class OneFactorClassificationSolution : ClassificationSolution {
    public new OneFactorClassificationModel Model {
      get { return (OneFactorClassificationModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    private OneFactorClassificationSolution(StorableConstructorFlag _) : base(_) { }
    private OneFactorClassificationSolution(OneFactorClassificationSolution original, Cloner cloner) : base(original, cloner) { }
    public OneFactorClassificationSolution(OneFactorClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneFactorClassificationSolution(this, cloner);
    }
  }
}
