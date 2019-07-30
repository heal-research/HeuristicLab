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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("AlbaTranslocationMoveEvaluator", "Evaluates a translocation or insertion move (3-opt) for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("36E23651-0091-4CE9-869F-BACA65464D72")]
  public sealed class AlbaTranslocationMoveEvaluator : AlbaMoveEvaluator, IAlbaTranslocationMoveOperator {
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
      set { Parameters["TranslocationMove"].ActualValue = value.ActualValue; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TranslocationMoveParameter; }
    }

    [StorableConstructor]
    private AlbaTranslocationMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public AlbaTranslocationMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaTranslocationMoveEvaluator(this, cloner);
    }

    private AlbaTranslocationMoveEvaluator(AlbaTranslocationMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      TranslocationMove move = TranslocationMoveParameter.ActualValue;
      //perform move
      AlbaEncoding newSolution = move.Permutation.Clone() as AlbaEncoding;
      TranslocationManipulator.Apply(newSolution, move.Index1, move.Index2, move.Index3);

      UpdateEvaluation(newSolution);
    }
  }
}
