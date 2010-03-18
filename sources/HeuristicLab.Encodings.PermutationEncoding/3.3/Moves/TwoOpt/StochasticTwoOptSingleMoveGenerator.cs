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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticTwoOptSingleMoveGenerator", "Randomly samples a single from all possible 2-opt moves from a given permutation.")]
  [StorableClass]
  public class StochasticTwoOptSingleMoveGenerator : TwoOptMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public StochasticTwoOptSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public static TwoOptMove Apply(Permutation permutation, IRandom random) {
      int length = permutation.Length;
      int index1 = random.Next(length - 1), index2;
      if (index1 >= 2)
        index2 = random.Next(index1 + 1, length);
      else index2 = random.Next(index1 + 1, length - (2 - index1));
      return new TwoOptMove(index1, index2);;
    }

    protected override TwoOptMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return new TwoOptMove[] { Apply(permutation, random) };
    }
  }
}
