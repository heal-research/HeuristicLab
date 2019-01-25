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
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Estimated 2.5-MoveEvaluator", "Operator that evaluates 2.5-p-opt moves of PTSP")]
  [StorableType("3E67BDD5-5A80-46F7-A7D6-9A67595CFD8C")]
  public class PTSPEstimatedTwoPointFiveMoveEvaluator : EstimatedPTSPMoveEvaluator, ITwoPointFiveMoveOperator {

    public ILookupParameter<TwoPointFiveMove> TwoPointFiveMoveParameter {
      get { return (ILookupParameter<TwoPointFiveMove>)Parameters["TwoPointFiveMove"]; }
    }

    [StorableConstructor]
    protected PTSPEstimatedTwoPointFiveMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PTSPEstimatedTwoPointFiveMoveEvaluator(PTSPEstimatedTwoPointFiveMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPEstimatedTwoPointFiveMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TwoPointFiveMove>("TwoPointFiveMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPEstimatedTwoPointFiveMoveEvaluator(this, cloner);
    }

    protected override double EvaluateMove(Permutation permutation, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      return EvaluateMove(permutation, TwoPointFiveMoveParameter.ActualValue, distance, realizations);
    }

    public static double EvaluateMove(Permutation permutation, TwoPointFiveMove move, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      if (move.IsInvert) {
        return PTSPEstimatedInversionMoveEvaluator.EvaluateMove(permutation,
          new InversionMove(move.Index1, move.Index2, move.Permutation), distance, realizations);
      } else {
        return PTSPEstimatedInsertionMoveEvaluator.EvaluateMove(permutation,
          new TranslocationMove(move.Index1, move.Index1, move.Index2), distance, realizations);
      }
    }
  }
}
