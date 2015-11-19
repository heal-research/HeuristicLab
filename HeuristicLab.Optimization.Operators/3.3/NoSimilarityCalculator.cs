#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An item that performs similarity calculation between two solutions.
  /// </summary>
  /// <remarks>
  /// The item always considers two solutions to be distinct.
  /// </remarks>
  [Item("NoSimilarityCalculator", "An item that performs similarity calculation between two solutions. The item always considers two solutions to be distinct.")]
  public sealed class NoSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative { get { return true; } }

    private NoSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private NoSimilarityCalculator(NoSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public NoSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NoSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(IScope left, IScope right) {
      return 0.0;
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      return CalculateSimilarity(leftSolution, rightSolution);
    }
  }
}