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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticTranslocationSingleMoveGenerator", "Randomly samples one from all possible translocation and insertion moves (3-opt) from a given permutation.")]
  [StorableClass]
  public class StochasticTranslocationSingleMoveGenerator : TranslocationMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticTranslocationSingleMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticTranslocationSingleMoveGenerator(StochasticTranslocationSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticTranslocationSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticTranslocationSingleMoveGenerator(this, cloner);
    }

    public static TranslocationMove Apply(Permutation permutation, IRandom random) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("StochasticThreeOptSingleMoveGenerator: There cannot be an insertion move given a permutation of length 1.", "permutation");
      if (permutation.PermutationType != PermutationTypes.Absolute && length == 2) return new TranslocationMove(0, 0, 1);
      int index1, index2, index3;
      do {
        index1 = random.Next(length);
        do {
          index2 = random.Next(index1, length);
        } while (index2 - index1 >= length - 2);
        do {
          index3 = random.Next(length - index2 + index1);
        } while (index3 == index1);
      } while (permutation.PermutationType != PermutationTypes.Absolute && (index1 == 0 && index3 == length - 1 || index1 == length - 1 && index3 == 0));
      return new TranslocationMove(index1, index2, index3);
    }

    protected override TranslocationMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return new TranslocationMove[] { Apply(permutation, random) };
    }
  }
}
