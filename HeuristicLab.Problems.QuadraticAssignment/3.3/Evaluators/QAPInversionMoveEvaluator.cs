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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAPInversionMoveEvaluator", "Evaluated an inversion move on a QAP solution.")]
  [StorableClass]
  public class QAPInversionMoveEvaluator : QAPMoveEvaluator, IPermutationInversionMoveOperator {
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected QAPInversionMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPInversionMoveEvaluator(QAPInversionMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPInversionMoveEvaluator() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPInversionMoveEvaluator(this, cloner);
    }

    public static double Apply(Permutation assignment, InversionMove move, DoubleMatrix weights, DoubleMatrix distances) {
      if (move.Index1 == move.Index2) return 0;
      double moveQuality = 0;
      int min = Math.Min(move.Index1, move.Index2);
      int max = Math.Max(move.Index1, move.Index2);

      for (int i = min; i <= max; i++) {
        int locI = assignment[i];
        int newlocI = assignment[max - i + min];

        for (int j = 0; j < assignment.Length; j++) {
          int locJ = assignment[j];
          if (j >= min && j <= max) {
            int newlocJ = assignment[max - j + min];
            moveQuality += weights[i, j] * (distances[newlocI, newlocJ] - distances[locI, locJ]);
          } else {
            moveQuality += weights[i, j] * (distances[newlocI, locJ] - distances[locI, locJ]);
            moveQuality += weights[j, i] * (distances[locJ, newlocI] - distances[locJ, locI]);
          }
        }
      }
      return moveQuality;
    }

    public override IOperation Apply() {
      InversionMove move = InversionMoveParameter.ActualValue;
      if (move == null) throw new InvalidOperationException("Inversion move is not found.");
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
