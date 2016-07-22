#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [StorableClass]
  [Item(Name = "OneR Classification Solution", Description = "Represents a OneR classification solution which uses only a single feature with potentially multiple thresholds for class prediction.")]
  public class OneRClassificationSolution : ClassificationSolution {
    public new OneRClassificationModel Model {
      get { return (OneRClassificationModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    protected OneRClassificationSolution(bool deserializing) : base(deserializing) { }
    protected OneRClassificationSolution(OneRClassificationSolution original, Cloner cloner) : base(original, cloner) { }
    public OneRClassificationSolution(OneRClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneRClassificationSolution(this, cloner);
    }
  }
}
