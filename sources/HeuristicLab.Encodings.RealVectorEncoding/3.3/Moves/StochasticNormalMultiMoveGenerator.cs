#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("StochasticNormalMultiMoveGenerator", "Generates normal distributed moves from a given real vector.")]
  [StorableClass]
  public class StochasticNormalMultiMoveGenerator : AdditiveMoveGenerator, IMultiMoveGenerator {
    public IValueLookupParameter<DoubleValue> SigmaParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Sigma"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    protected StochasticNormalMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticNormalMultiMoveGenerator(StochasticNormalMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticNormalMultiMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Sigma", "The standard deviation of the normal distribution.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves that should be generated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticNormalMultiMoveGenerator(this, cloner);
    }

    public static AdditiveMove[] Apply(IRandom random, RealVector vector, double sigma, int sampleSize, DoubleMatrix bounds) {
      AdditiveMove[] moves = new AdditiveMove[sampleSize];
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0, sigma);
      for (int i = 0; i < sampleSize; i++) {
        int index = random.Next(vector.Length);
        double strength = 0, min = bounds[index % bounds.Rows, 0], max = bounds[index % bounds.Rows, 1];
        do {
          strength = N.NextDouble();
        } while (vector[index] + strength < min || vector[index] + strength > max);
        moves[i] = new AdditiveMove(index, strength);
      }
      return moves;
    }

    protected override AdditiveMove[] GenerateMoves(IRandom random, RealVector realVector, DoubleMatrix bounds) {
      return Apply(random, realVector, SigmaParameter.ActualValue.Value, SampleSizeParameter.ActualValue.Value, bounds);
    }
  }
}
