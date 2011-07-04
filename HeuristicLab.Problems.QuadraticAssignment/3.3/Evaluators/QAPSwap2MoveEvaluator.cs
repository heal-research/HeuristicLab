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
  [Item("QAPSwap2MoveEvaluator", "Evaluated a swap-2 move on a QAP solution.")]
  [StorableClass]
  public class QAPSwap2MoveEvaluator : QAPMoveEvaluator, IPermutationSwap2MoveOperator {
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (ILookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }

    [StorableConstructor]
    protected QAPSwap2MoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPSwap2MoveEvaluator(QAPSwap2MoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPSwap2MoveEvaluator() {
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPSwap2MoveEvaluator(this, cloner);
    }

    public static double Apply(Permutation assignment, Swap2Move move, DoubleMatrix weights, DoubleMatrix distances) {
      if (move.Index1 == move.Index2) return 0;
      double moveQuality = 0;
      int fac1 = move.Index1, fac2 = move.Index2;
      int loc1 = assignment[fac1], loc2 = assignment[fac2];

      for (int j = 0; j < assignment.Length; j++) {
        if (j == fac1) {
          moveQuality += weights[fac1, fac1] * (distances[loc2, loc2] - distances[loc1, loc1]);
          moveQuality += weights[fac1, fac2] * (distances[loc2, loc1] - distances[loc1, loc2]);
        } else if (j == fac2) {
          moveQuality += weights[fac2, fac2] * (distances[loc1, loc1] - distances[loc2, loc2]);
          moveQuality += weights[fac2, fac1] * (distances[loc1, loc2] - distances[loc2, loc1]);
        } else {
          int locJ = assignment[j];
          moveQuality += weights[fac1, j] * (distances[loc2, locJ] - distances[loc1, locJ]);
          moveQuality += weights[j, fac1] * (distances[locJ, loc2] - distances[locJ, loc1]);
          moveQuality += weights[fac2, j] * (distances[loc1, locJ] - distances[loc2, locJ]);
          moveQuality += weights[j, fac2] * (distances[locJ, loc1] - distances[locJ, loc2]);
        }
      }
      return moveQuality;
    }

    public override IOperation Apply() {
      Swap2Move move = Swap2MoveParameter.ActualValue;
      if (move == null) throw new InvalidOperationException("Swap-2 move is not found.");
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
