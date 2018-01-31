#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PTSP Analytical Insertion Move Evaluator", "Evaluates an insertion move (1-shift) by a full solution evaluation")]
  [StorableClass]
  public class PTSPAnalyticalInsertionMoveEvaluator : AnalyticalPTSPMoveEvaluator, IPermutationTranslocationMoveOperator {

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected PTSPAnalyticalInsertionMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected PTSPAnalyticalInsertionMoveEvaluator(PTSPAnalyticalInsertionMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPAnalyticalInsertionMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPAnalyticalInsertionMoveEvaluator(this, cloner);
    }

    public static double EvaluateMove(Permutation tour, TranslocationMove move, Func<int, int, double> distance, DoubleArray probabilities) {
      var afterMove = (Permutation)tour.Clone();
      TranslocationManipulator.Apply(afterMove, move.Index1, move.Index1, move.Index3);
      return AnalyticalProbabilisticTravelingSalesmanProblem.Evaluate(afterMove, distance, probabilities);
    }

    protected override double EvaluateMove(Permutation tour, Func<int, int, double> distance, DoubleArray probabilities) {
      return EvaluateMove(tour, TranslocationMoveParameter.ActualValue, distance, probabilities);
    }
  }
}
