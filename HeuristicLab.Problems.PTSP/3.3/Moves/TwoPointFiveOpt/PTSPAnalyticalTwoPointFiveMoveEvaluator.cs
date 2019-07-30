#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Analytical 2.5-MoveEvaluator", "Operator that evaluates 2.5-p-opt moves of PTSP by a full solution evaluation.")]
  [StorableType("59729BB5-2026-44E8-8C19-EBCC2303103C")]
  public class PTSPAnalyticalTwoPointFiveMoveEvaluator : AnalyticalPTSPMoveEvaluator, ITwoPointFiveMoveOperator {

    public ILookupParameter<TwoPointFiveMove> TwoPointFiveMoveParameter {
      get { return (ILookupParameter<TwoPointFiveMove>)Parameters["TwoPointFiveMove"]; }
    }

    [StorableConstructor]
    protected PTSPAnalyticalTwoPointFiveMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PTSPAnalyticalTwoPointFiveMoveEvaluator(PTSPAnalyticalTwoPointFiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPAnalyticalTwoPointFiveMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TwoPointFiveMove>("TwoPointFiveMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPAnalyticalTwoPointFiveMoveEvaluator(this, cloner);
    }

    protected override double EvaluateMove(Permutation permutation, Func<int, int, double> distance, DoubleArray probabilities) {
      return EvaluateMove(permutation, TwoPointFiveMoveParameter.ActualValue, distance, probabilities);
    }

    public static double EvaluateMove(Permutation permutation, TwoPointFiveMove move, Func<int, int, double> distance, DoubleArray probabilities) {
      if (move.IsInvert) {
        return PTSPAnalyticalInversionMoveEvaluator.EvaluateMove(permutation,
          new InversionMove(move.Index1, move.Index2, move.Permutation), distance, probabilities);
      } else {
        return PTSPAnalyticalInsertionMoveEvaluator.EvaluateMove(permutation,
          new TranslocationMove(move.Index1, move.Index1, move.Index2), distance, probabilities);
      }
    }
  }
}
