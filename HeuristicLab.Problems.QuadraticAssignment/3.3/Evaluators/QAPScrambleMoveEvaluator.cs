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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAPScrambleMoveEvaluator", "Evaluated a scramble move on a QAP solution.")]
  [StorableClass]
  public class QAPScrambleMoveEvaluator : QAPMoveEvaluator, IPermutationScrambleMoveOperator {
    public ILookupParameter<ScrambleMove> ScrambleMoveParameter {
      get { return (ILookupParameter<ScrambleMove>)Parameters["ScrambleMove"]; }
    }

    [StorableConstructor]
    protected QAPScrambleMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPScrambleMoveEvaluator(QAPScrambleMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPScrambleMoveEvaluator() {
      Parameters.Add(new LookupParameter<ScrambleMove>("ScrambleMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPScrambleMoveEvaluator(this, cloner);
    }

    /// <summary>
    /// Calculates the quality of the move <paramref name="move"/> by evaluating the changes.
    /// The runtime complexity of this method is O(N) with N being the size of the permutation.
    /// </summary>
    /// <param name="assignment">The current permutation.</param>
    /// <param name="move">The move that is to be evaluated if it was applied to the current permutation.</param>
    /// <param name="weights">The weights matrix.</param>
    /// <param name="distances">The distances matrix.</param>
    /// <returns>The relative change in quality if <paramref name="move"/> was applied to <paramref name="assignment"/>.</returns>
    public static double Apply(Permutation assignment, ScrambleMove move, DoubleMatrix weights, DoubleMatrix distances) {
      double moveQuality = 0;
      int min = move.StartIndex;
      int max = min + move.ScrambledIndices.Length - 1;

      for (int i = min; i <= max; i++) {
        int locI = assignment[i];
        int newlocI = assignment[min + move.ScrambledIndices[i - min]];
        if (locI == newlocI) continue;

        for (int j = 0; j < assignment.Length; j++) {
          int locJ = assignment[j];
          if (j >= min && j <= max) {
            int newlocJ = assignment[min + move.ScrambledIndices[j - min]];
            moveQuality += weights[i, j] * (distances[newlocI, newlocJ] - distances[locI, locJ]);
            if (locJ == newlocJ)
              moveQuality += weights[j, i] * (distances[newlocJ, newlocI] - distances[locJ, locI]);
          } else {
            moveQuality += weights[i, j] * (distances[newlocI, locJ] - distances[locI, locJ]);
            moveQuality += weights[j, i] * (distances[locJ, newlocI] - distances[locJ, locI]);
          }
        }
      }
      return moveQuality;
    }

    public override IOperation Apply() {
      ScrambleMove move = ScrambleMoveParameter.ActualValue;
      if (move == null) throw new InvalidOperationException("Scramble move is not found.");
      Permutation assignment = PermutationParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;

      double moveQuality = QualityParameter.ActualValue.Value;
      moveQuality += Apply(assignment, move, weights, distances);
      MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      return base.Apply();
    }
  }
}
