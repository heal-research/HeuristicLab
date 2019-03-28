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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ScrambleMoveMaker", "Peforms a scramble move on a given permutation and updates the quality.")]
  [StorableType("5DC6735A-F109-4D86-98CF-B8CCB648D3D3")]
  public class ScrambleMoveMaker : SingleSuccessorOperator, IPermutationScrambleMoveQualityOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<ScrambleMove> ScrambleMoveParameter {
      get { return (ILookupParameter<ScrambleMove>)Parameters["ScrambleMove"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    protected ScrambleMoveMaker(StorableConstructorFlag _) : base(_) { }
    protected ScrambleMoveMaker(ScrambleMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public ScrambleMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<ScrambleMove>("ScrambleMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScrambleMoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      ScrambleMove move = ScrambleMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      ScrambleManipulator.Apply(permutation, move.StartIndex, move.ScrambledIndices);
      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}
