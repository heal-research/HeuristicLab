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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("StochasticOneBitflipMultiMoveGenerator", "Randomly samples n from all possible one bitflip moves from a given BinaryVector.")]
  [StorableType("11A9E43A-6291-4F9D-90AB-DC205923EE68")]
  public class StochasticOneBitflipMultiMoveGenerator : OneBitflipMoveGenerator, IStochasticOperator, IMultiMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public IntValue SampleSize {
      get { return SampleSizeParameter.Value; }
      set { SampleSizeParameter.Value = value; }
    }

    [StorableConstructor]
    protected StochasticOneBitflipMultiMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected StochasticOneBitflipMultiMoveGenerator(StochasticOneBitflipMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticOneBitflipMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticOneBitflipMultiMoveGenerator(this, cloner);
    }

    public static OneBitflipMove[] Apply(BinaryVector binaryVector, IRandom random, int sampleSize) {
      OneBitflipMove[] moves = new OneBitflipMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = StochasticOneBitflipSingleMoveGenerator.Apply(binaryVector, random);
      }
      return moves;
    }

    protected override OneBitflipMove[] GenerateMoves(BinaryVector binaryVector) {
      IRandom random = RandomParameter.ActualValue;
      if (SampleSizeParameter.ActualValue == null) throw new InvalidOperationException("StochasticOneBitflipMultiMoveGenerator: Parameter " + SampleSizeParameter.ActualName + " could not be found.");
      return Apply(binaryVector, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
