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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.NK {

  [Item("NK BitFlip Move Evaluator", "Evaluates a single bit flip on an NK landscape.")]
  [StorableClass]
  public class NKBitFlipMoveEvaluator : NKMoveEvaluator, IOneBitflipMoveOperator {
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (ILookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }
    public ILookupParameter<BinaryVector> MovedBinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["MovedBinaryVector"]; }
    }

    [StorableConstructor]
    protected NKBitFlipMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected NKBitFlipMoveEvaluator(NKBitFlipMoveEvaluator original, Cloner cloner)
      : base(original, cloner) { }
    public NKBitFlipMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BinaryVector>("MovedBinaryVector", "The resulting binary vector after the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NKBitFlipMoveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      BinaryVector binaryVector = BinaryVectorParameter.ActualValue;
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;
      BoolMatrix interactions = GeneInteractionsParameter.ActualValue;
      DoubleArray weights = WeightsParameter.ActualValue;
      int seed = InteractionSeedParameter.ActualValue.Value;
      double moveQuality = QualityParameter.ActualValue.Value;
      int q = QParameter.ActualValue.Value;
      double p = PParameter.ActualValue.Value;

      List<int> affectedFitnessComponents = new List<int>();
      for (int c = 0; c < interactions.Columns; c++)
        if (interactions[move.Index, c])
          affectedFitnessComponents.Add(c);
      BinaryVector moved = new BinaryVector(binaryVector);
      MovedBinaryVectorParameter.ActualValue = moved;
      moved[move.Index] = !moved[move.Index];
      if (affectedFitnessComponents.Count * 2 > interactions.Columns) {
        double[] f_i;
        moveQuality = NKLandscape.Evaluate(moved, interactions, weights, seed, out f_i, q, p);
      } else {
        long x = NKLandscape.Encode(binaryVector);
        long y = NKLandscape.Encode(moved);
        long[] g = NKLandscape.Encode(interactions);
        double[] w = NKLandscape.Normalize(weights);
        foreach (var c in affectedFitnessComponents) {
          moveQuality -= w[c%w.Length]*NKLandscape.F_i(x, c, g[c], seed, q, p);
          moveQuality += w[c%w.Length]*NKLandscape.F_i(y, c, g[c], seed, q, p);
        }
      }

      if (MoveQualityParameter.ActualValue == null) MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      else MoveQualityParameter.ActualValue.Value = moveQuality;
      return base.Apply();
    }
  }
}
