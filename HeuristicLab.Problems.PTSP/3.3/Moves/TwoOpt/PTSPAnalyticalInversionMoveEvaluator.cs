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

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Analytical Inversion Move Evaluator", "Evaluates an inversion move (2-opt) by a full solution evaluation.")]
  [StorableClass]
  public class PTSPAnalyticalInversionMoveEvaluator : AnalyticalPTSPMoveEvaluator, IPermutationInversionMoveOperator {

    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected PTSPAnalyticalInversionMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected PTSPAnalyticalInversionMoveEvaluator(PTSPAnalyticalInversionMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPAnalyticalInversionMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPAnalyticalInversionMoveEvaluator(this, cloner);
    }

    public static double EvaluateMove(Permutation tour, InversionMove move, Func<int, int, double> distance, DoubleArray probabilities) {
      var afterMove = (Permutation)tour.Clone();
      InversionManipulator.Apply(afterMove, move.Index1, move.Index2);
      return AnalyticalProbabilisticTravelingSalesmanProblem.Evaluate(afterMove, distance, probabilities);
    }

    protected override double EvaluateMove(Permutation tour, Func<int, int, double> distance, DoubleArray probabilities) {
      return EvaluateMove(tour, InversionMoveParameter.ActualValue, distance, probabilities);
    }
  }
}
