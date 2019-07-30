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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Swap2MoveMaker", "Peforms a swap-2 move on a given permutation and updates the quality.")]
  [StorableType("A5781DC7-E9F0-4926-A3BF-A43EE893E4F9")]
  public class Swap2MoveMaker : SingleSuccessorOperator, IPermutationSwap2MoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (ILookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    protected Swap2MoveMaker(StorableConstructorFlag _) : base(_) { }
    protected Swap2MoveMaker(Swap2MoveMaker original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      Swap2Move move = Swap2MoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      Swap2Manipulator.Apply(permutation, move.Index1, move.Index2);
      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}
