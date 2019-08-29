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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticInversionSingleMoveGenerator", "Randomly samples a single from all possible inversion moves (2-opt) from a given permutation.")]
  [StorableType("F7190264-4BE1-45A2-A908-E2C7869B4F1F")]
  public class StochasticInversionSingleMoveGenerator : InversionMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticInversionSingleMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected StochasticInversionSingleMoveGenerator(StochasticInversionSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticInversionSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticInversionSingleMoveGenerator(this, cloner);
    }

    public static InversionMove Apply(Permutation permutation, IRandom random) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("StochasticInversionSingleMoveGenerator: There cannot be an inversion move given a permutation of length 1.", "permutation");
      int index1 = random.Next(length - 1);
      int index2 = random.Next(index1 + 1, length);
      if (permutation.PermutationType == PermutationTypes.RelativeUndirected) {
        if (length > 3) {
          while (index2 - index1 >= length - 2)
            index2 = random.Next(index1 + 1, length);
        }
      }
      return new InversionMove(index1, index2);
    }

    protected override InversionMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return new InversionMove[] { Apply(permutation, random) };
    }
  }
}
