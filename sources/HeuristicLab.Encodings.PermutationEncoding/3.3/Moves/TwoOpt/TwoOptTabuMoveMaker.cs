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
  [Item("TwoOptTabuMoveMaker", "Declares a given 2-opt move as tabu, by adding its attributes to the tabu list. It also removes the oldest entry in the tabu list when its size is greater than tenure.")]
  [StorableClass]
  public class TwoOptTabuMoveMaker : TabuMoveMaker, ITwoOptPermutationMoveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<TwoOptMove> TwoOptMoveParameter {
      get { return (LookupParameter<TwoOptMove>)Parameters["TwoOptMove"]; }
    }

    public TwoOptTabuMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<TwoOptMove>("TwoOptMove", "The move that was made."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    protected override IItem GetTabuAttribute() {
      TwoOptMove move = TwoOptMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      return new TwoOptTabuMoveAttribute( permutation.GetCircular(move.Index1 - 1),
        permutation[move.Index1],
        permutation[move.Index2],
        permutation.GetCircular(move.Index2 + 1));
    }
  }
}
