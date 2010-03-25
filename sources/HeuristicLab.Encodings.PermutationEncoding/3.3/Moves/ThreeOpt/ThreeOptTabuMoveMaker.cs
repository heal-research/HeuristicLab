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
  [Item("ThreeOptTabuMoveMaker", "Declares a given 3-opt move as tabu, by adding its attributes to the tabu list. It also removes the oldest entry in the tabu list when its size is greater than tenure.")]
  [StorableClass]
  public class ThreeOptTabuMoveMaker : TabuMoveMaker, IThreeOptPermutationMoveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<ThreeOptMove> ThreeOptMoveParameter {
      get { return (LookupParameter<ThreeOptMove>)Parameters["ThreeOptMove"]; }
    }

    public ThreeOptTabuMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<ThreeOptMove>("ThreeOptMove", "The move that was made."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    protected override IItem GetTabuAttribute() {
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      if (move.Index3 > move.Index1)
        return new ThreeOptTabuMoveAttribute(permutation.GetCircular(move.Index1 - 1),
        permutation[move.Index1],
        permutation[move.Index2],
        permutation.GetCircular(move.Index2 + 1),
        permutation.GetCircular(move.Index3 + move.Index2 - move.Index1),
        permutation.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1));
      else
        return new ThreeOptTabuMoveAttribute(permutation.GetCircular(move.Index1 - 1),
        permutation[move.Index1],
        permutation[move.Index2],
        permutation.GetCircular(move.Index2 + 1),
        permutation.GetCircular(move.Index3 - 1),
        permutation.GetCircular(move.Index3));
    }
  }
}
