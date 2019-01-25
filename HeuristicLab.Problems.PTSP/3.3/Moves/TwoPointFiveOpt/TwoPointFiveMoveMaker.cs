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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [Item("TwoPointFiveMoveMaker", "Peforms an inversion and shift (2.5-opt) on a given permutation and updates the quality.")]
  [StorableType("6D5923B3-47CA-47AF-8C93-9BA7134BE5BA")]
  public class TwoPointFiveMoveMaker : SingleSuccessorOperator, ITwoPointFiveMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<TwoPointFiveMove> TwoPointFiveMoveParameter {
      get { return (ILookupParameter<TwoPointFiveMove>)Parameters["TwoPointFiveMove"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    protected TwoPointFiveMoveMaker(StorableConstructorFlag _) : base(_) { }
    protected TwoPointFiveMoveMaker(TwoPointFiveMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public TwoPointFiveMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<TwoPointFiveMove>("TwoPointFiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TwoPointFiveMoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      var move = TwoPointFiveMoveParameter.ActualValue;
      var permutation = PermutationParameter.ActualValue;
      var moveQuality = MoveQualityParameter.ActualValue;
      var quality = QualityParameter.ActualValue;

      Apply(permutation, move);

      quality.Value = moveQuality.Value;

      return base.Apply();
    }

    public static void Apply(Permutation permutation, TwoPointFiveMove move) {
      if (move.IsInvert) {
        InversionManipulator.Apply(permutation, move.Index1, move.Index2);
      } else {
        TranslocationManipulator.Apply(permutation, move.Index1, move.Index1, move.Index2);
      }
    }
  }
}
