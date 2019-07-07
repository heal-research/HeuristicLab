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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarMoveEvaluator", "Evaluates a two opt star move for a VRP representation. ")]
  [StorableType("5CB2E8F6-191E-4EBE-B832-18BECC9BB19B")]
  public sealed class PotvinTwoOptStarMoveEvaluator : PotvinMoveEvaluator, IPotvinTwoOptStarMoveOperator {
    public ILookupParameter<PotvinTwoOptStarMove> TwoOptStarMoveParameter {
      get { return (ILookupParameter<PotvinTwoOptStarMove>)Parameters["PotvinTwoOptStarMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TwoOptStarMoveParameter; }
    }
    [StorableConstructor]
    private PotvinTwoOptStarMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public PotvinTwoOptStarMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinTwoOptStarMove>("PotvinTwoOptStarMove", "The move that should be evaluated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarMoveEvaluator(this, cloner);
    }

    private PotvinTwoOptStarMoveEvaluator(PotvinTwoOptStarMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      PotvinTwoOptStarMove move = TwoOptStarMoveParameter.ActualValue;

      PotvinEncoding newSolution = TwoOptStarMoveParameter.ActualValue.Individual.Clone() as PotvinEncoding;
      PotvinTwoOptStarMoveMaker.Apply(newSolution, move, ProblemInstance);

      UpdateEvaluation(newSolution);
    }
  }
}
