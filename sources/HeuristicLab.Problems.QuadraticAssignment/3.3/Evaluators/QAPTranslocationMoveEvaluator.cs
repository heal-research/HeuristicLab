#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("QAPTranslocationMoveEvaluator", "Evaluates translocation moves on a QAP solution.")]
  [StorableClass]
  public class QAPTranslocationMoveEvaluator : QAPMoveEvaluator, IPermutationTranslocationMoveOperator {

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected QAPTranslocationMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPTranslocationMoveEvaluator(QAPTranslocationMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPTranslocationMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "A move which takes part of a permutation and inserts it in a different part."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPTranslocationMoveEvaluator(this, cloner);
    }

    public static double Apply(Permutation assignment, TranslocationMove move, DoubleMatrix weights, DoubleMatrix distances) {
      double moveQuality = 0;
      int min = Math.Min(move.Index1, move.Index3);
      int max = Math.Max(move.Index2, move.Index3 + (move.Index2 - move.Index1));
      int iOffset, changeOffset;
      if (move.Index1 < move.Index3) {
        iOffset = move.Index2 - move.Index1 + 1;
        changeOffset = min + max - move.Index2;
      } else {
        iOffset = move.Index1 - move.Index3;
        changeOffset = min + move.Index2 - move.Index1 + 1;
      }
      for (int i = min; i <= max; i++) {
        if (i == changeOffset) iOffset -= (max - min + 1);
        int jOffset = ((move.Index1 < move.Index3) ? (move.Index2 - move.Index1 + 1) : (move.Index1 - move.Index3));
        for (int j = 0; j < assignment.Length; j++) {
          moveQuality -= weights[i, j] * distances[assignment[i], assignment[j]];
          if (j < min || j > max) {
            moveQuality -= weights[j, i] * distances[assignment[j], assignment[i]];
            moveQuality += weights[i, j] * distances[assignment[i + iOffset], assignment[j]];
            moveQuality += weights[j, i] * distances[assignment[j], assignment[i + iOffset]];
          } else {
            if (j == changeOffset) jOffset -= (max - min + 1);
            moveQuality += weights[i, j] * distances[assignment[i + iOffset], assignment[j + jOffset]];
          }
        }
      }
      return moveQuality;
    }

    public override IOperation Apply() {
      TranslocationMove move = TranslocationMoveParameter.ActualValue;
      if (move == null) throw new InvalidOperationException("Translocation move is not found.");
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
