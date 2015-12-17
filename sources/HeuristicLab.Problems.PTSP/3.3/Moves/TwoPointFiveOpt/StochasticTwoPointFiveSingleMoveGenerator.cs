#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.PTSP {
  [Item("Stochastic 2.5-SingleMoveGenerator", "Randomly samples a single from all possible inversion and shift moves (2.5-opt) from a given permutation.")]
  [StorableClass]
  public sealed class StochasticTwoPointFiveSingleMoveGenerator : TwoPointFiveMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private StochasticTwoPointFiveSingleMoveGenerator(bool deserializing) : base(deserializing) { }
    private StochasticTwoPointFiveSingleMoveGenerator(StochasticTwoPointFiveSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticTwoPointFiveSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticTwoPointFiveSingleMoveGenerator(this, cloner);
    }

    public static TwoPointFiveMove Apply(Permutation permutation, IRandom random) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("Stochastic 2.5-SingleMoveGenerator: There cannot be an inversion move given a permutation of length 1.", "permutation");
      int index1 = random.Next(length - 1);
      int index2 = random.Next(index1 + 1, length);
      if (permutation.PermutationType == PermutationTypes.RelativeUndirected) {
        if (length > 3) {
          while (index2 - index1 >= length - 2)
            index2 = random.Next(index1 + 1, length);
        }
      }
      bool isInvert = random.NextDouble() > 0.5;
      return new TwoPointFiveMove(index1, index2, isInvert);

    }

    protected override TwoPointFiveMove[] GenerateMoves(Permutation permutation) {
      return new[] { Apply(permutation, RandomParameter.ActualValue) };
    }
  }
}
