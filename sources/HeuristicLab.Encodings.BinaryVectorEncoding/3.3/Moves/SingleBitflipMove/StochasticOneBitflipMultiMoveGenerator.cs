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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("StochasticOneBitflipMultiMoveGenerator", "Randomly samples n from all possible one bitflip moves from a given BinaryVector.")]
  [StorableClass]
  public class StochasticOneBitflipMultiMoveGenerator : StochasticOneBitflipSingleMoveGenerator, IMultiMoveGenerator {
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public IntValue SampleSize {
      get { return SampleSizeParameter.Value; }
      set { SampleSizeParameter.Value = value; }
    }

    public StochasticOneBitflipMultiMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public static OneBitflipMove[] Apply(BinaryVector binaryVector, IRandom random, int sampleSize) {
      OneBitflipMove[] moves = new OneBitflipMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = Apply(binaryVector, random);
      }
      return moves;
    }

    protected override OneBitflipMove[] GenerateMoves(BinaryVector binaryVector) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(binaryVector, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
