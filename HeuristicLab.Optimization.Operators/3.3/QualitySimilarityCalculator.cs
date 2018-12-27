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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An item that performs similarity calculation between two solutions.
  /// </summary>
  /// <remarks>
  /// The item always considers two solutions to be equal if they have the same quality.
  /// </remarks>
  [Item("QualitySimilarityCalculator", "An item that performs similarity calculation between two solutions. The item only considers the qualities of the two solutions.")]
  public sealed class QualitySimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative { get { return true; } }

    private QualitySimilarityCalculator(bool deserializing) : base(deserializing) { }
    private QualitySimilarityCalculator(QualitySimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public QualitySimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualitySimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(IScope left, IScope right, string qualityVariableName) {
      double leftQuality = ((DoubleValue)left.Variables[qualityVariableName].Value).Value;
      double rightQuality = ((DoubleValue)right.Variables[qualityVariableName].Value).Value;
      double delta = leftQuality.IsAlmost(rightQuality) ? 0.0 : Math.Abs(leftQuality - rightQuality);
      return 1.0 - delta / Math.Max(leftQuality, rightQuality);
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      return CalculateSimilarity(leftSolution, rightSolution, QualityVariableName);
    }
  }
}