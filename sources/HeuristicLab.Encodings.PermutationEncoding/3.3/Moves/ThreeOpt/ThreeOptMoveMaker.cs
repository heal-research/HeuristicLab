#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ThreeOptMoveMaker", "Peforms a 3-opt move on a given permutation and updates the quality.")]
  [StorableClass]
  public class ThreeOptMoveMaker : SingleSuccessorOperator, IThreeOptPermutationMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<ThreeOptMove> ThreeOptMoveParameter {
      get { return (ILookupParameter<ThreeOptMove>)Parameters["ThreeOptMove"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public ThreeOptMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<ThreeOptMove>("ThreeOptMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IOperation Apply() {
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      TranslocationManipulator.Apply(permutation, move.Index1, move.Index2, move.Index3);
      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}
