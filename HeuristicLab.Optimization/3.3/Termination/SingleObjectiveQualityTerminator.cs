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
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("SingleObjectiveQualityTerminator", "A termination criterion which uses a quality parameter (eg. current best quality) for termination.")]
  [StorableType("A32469FB-7370-46E3-AB7F-EAF8053AF105")]
  public class SingleObjectiveQualityTerminator : ComparisonTerminator<DoubleValue> {
    [StorableConstructor]
    protected SingleObjectiveQualityTerminator(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveQualityTerminator(SingleObjectiveQualityTerminator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveQualityTerminator(this, cloner);
    }
    public SingleObjectiveQualityTerminator() { }

    public void Parameterize(IParameter qualityParameter, ISingleObjectiveHeuristicOptimizationProblem problem) {
      ComparisonValueParameter.ActualName = qualityParameter.Name;
      if (problem == null) return;

      var maximizationParameter = (IValueParameter<BoolValue>)problem.MaximizationParameter;
      if (maximizationParameter != null) {
        bool maximization = maximizationParameter.Value.Value;
        ComparisonType = maximization ? ComparisonType.Less : ComparisonType.Greater;
        Threshold.Value = maximization ? double.MaxValue : double.MinValue;
      }
    }
  }
}