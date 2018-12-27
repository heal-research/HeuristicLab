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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// A base class for items that perform similarity calculation between two single objective solutions.
  /// </summary>
  [Item("SimilarityCalculator", "A base class for items that perform similarity calculation between two solutions.")]
  [StorableClass]
#pragma warning disable 0618
  public abstract class SingleObjectiveSolutionSimilarityCalculator : SolutionSimilarityCalculator, ISingleObjectiveSolutionSimilarityCalculator {
#pragma warning restore 0618
    [StorableConstructor]
    protected SingleObjectiveSolutionSimilarityCalculator(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveSolutionSimilarityCalculator(SingleObjectiveSolutionSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {

    }
    protected SingleObjectiveSolutionSimilarityCalculator() : base() { }

    public override bool Equals(IScope x, IScope y) {
      if (object.ReferenceEquals(x, y)) return true;
      if (x == null || y == null) return false;
      double q1 = ((DoubleValue)x.Variables[QualityVariableName].Value).Value;
      double q2 = ((DoubleValue)y.Variables[QualityVariableName].Value).Value;
      return q1.IsAlmost(q2) && CalculateSolutionSimilarity(x, y).IsAlmost(1.0);
    }

    public override int GetHashCode(IScope scope) {
      return ((DoubleValue)scope.Variables[QualityVariableName].Value).Value.GetHashCode();
    }
  }
}
