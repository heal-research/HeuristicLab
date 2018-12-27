#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticInsertionMultiMoveGenerator", "Generates all possible insertion moves (3-opt) from a few numbers in a given permutation.")]
  [StorableClass]
  public class StochasticInsertionMultiMoveGenerator : TranslocationMoveGenerator, IMultiMoveGenerator, IStochasticOperator {
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
    protected StochasticInsertionMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticInsertionMultiMoveGenerator(StochasticInsertionMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticInsertionMultiMoveGenerator() : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticInsertionMultiMoveGenerator(this, cloner);
    }

    public static TranslocationMove[] Apply(Permutation permutation, IRandom random, int sampleSize) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("ExhaustiveSingleInsertionMoveGenerator: There cannot be an insertion move given a permutation of length 1.", "permutation");
      TranslocationMove[] moves = new TranslocationMove[sampleSize];
      int count = 0;
      HashSet<int> usedIndices = new HashSet<int>();
      while (count < sampleSize) {

        int index = random.Next(length);

        if (usedIndices.Count != length)
          while (usedIndices.Contains(index))
            index = random.Next(length);
        usedIndices.Add(index);

        if (permutation.PermutationType == PermutationTypes.Absolute) {
          for (int j = 1; j <= length - 1; j++) {
            moves[count++] = new TranslocationMove(index, index, (index + j) % length);
            if (count == sampleSize) break;
          }
        } else {
          if (length > 2) {
            for (int j = 1; j < length - 1; j++) {
              int insertPoint = (index + j) % length;
              if (index + j >= length) insertPoint++;
              moves[count++] = new TranslocationMove(index, index, insertPoint);
              if (count == sampleSize) break;
            }
          } else { // doesn't make sense, but just create a dummy move to not crash the algorithms
            moves = new TranslocationMove[1];
            moves[0] = new TranslocationMove(0, 0, 1);
            count = sampleSize;
          }
        }
      }
      return moves;
    }

    protected override TranslocationMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      if (random == null) throw new InvalidOperationException(Name + ": No random number generator found.");
      return Apply(permutation, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
