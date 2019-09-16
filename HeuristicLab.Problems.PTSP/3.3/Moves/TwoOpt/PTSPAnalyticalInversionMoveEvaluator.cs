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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Analytical Inversion Move Evaluator", "Evaluates an inversion move (2-opt) by a full solution evaluation.")]
  [StorableType("D24FF41B-DA6E-42C6-9304-21EF01BFC61B")]
  public class PTSPAnalyticalInversionMoveEvaluator : AnalyticalPTSPMoveEvaluator, IPermutationInversionMoveOperator {

    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected PTSPAnalyticalInversionMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PTSPAnalyticalInversionMoveEvaluator(PTSPAnalyticalInversionMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPAnalyticalInversionMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPAnalyticalInversionMoveEvaluator(this, cloner);
    }

    public static double EvaluateMove(Permutation tour, InversionMove move, IProbabilisticTSPData data) {
      var afterMove = (Permutation)tour.Clone();
      InversionManipulator.Apply(afterMove, move.Index1, move.Index2);
      return AnalyticalPTSP.Evaluate(afterMove, data);
    }

    protected override double EvaluateMove(Permutation tour, IProbabilisticTSPData data) {
      return EvaluateMove(tour, InversionMoveParameter.ActualValue, data);
    }
  }
}
