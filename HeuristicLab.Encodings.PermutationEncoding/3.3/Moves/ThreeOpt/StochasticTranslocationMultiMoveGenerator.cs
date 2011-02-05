#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticTranslocationMultiMoveGenerator", "Randomly samples n from all possible translocation and insertion moves (3-opt) from a given permutation.")]
  [StorableClass]
  public class StochasticTranslocationMultiMoveGenerator : TranslocationMoveGenerator, IStochasticOperator, IMultiMoveGenerator {
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
    protected StochasticTranslocationMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticTranslocationMultiMoveGenerator(StochasticTranslocationMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticTranslocationMultiMoveGenerator() : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticTranslocationMultiMoveGenerator(this, cloner);
    }

    public static TranslocationMove[] Apply(Permutation permutation, IRandom random, int sampleSize) {
      int length = permutation.Length;
      TranslocationMove[] moves = new TranslocationMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = StochasticTranslocationSingleMoveGenerator.Apply(permutation, random);
      }
      return moves;
    }

    protected override TranslocationMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(permutation, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
