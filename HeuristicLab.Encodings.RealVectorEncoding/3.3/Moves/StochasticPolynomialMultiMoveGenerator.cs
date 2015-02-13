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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("StochasticPolynomialMultiMoveGenerator", "Generates polynomial moves from a given real vector.")]
  [StorableClass]
  public class StochasticPolynomialMultiMoveGenerator : AdditiveMoveGenerator, IMultiMoveGenerator {
    /// <summary>
    /// The maximum manipulation parameter specifies the range of the manipulation. The value specified here is the highest value the mutation will ever add to the current value.
    /// </summary>
    public ValueLookupParameter<DoubleValue> MaximumManipulationParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumManipulation"]; }
    }
    /// <summary>
    /// The contiguity parameter specifies the shape of the probability density function that controls the mutation. Setting it to 0 is similar to a uniform distribution over the entire manipulation range (specified by <see cref="MaximumManipulationParameter"/>.
    /// A higher value will shape the density function such that values closer to 0 (little manipulation) are more likely than values closer to 1 or -1 (maximum manipulation).
    /// </summary>
    public IValueLookupParameter<DoubleValue> ContiguityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Contiguity"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    protected StochasticPolynomialMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticPolynomialMultiMoveGenerator(StochasticPolynomialMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticPolynomialMultiMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Contiguity", "Specifies whether the manipulation should produce far stretching (small value) or close (large value) manipulations with higher probability. Valid values must be greater or equal to 0.", new DoubleValue(2)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves that should be generated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumManipulation", "Specifies the maximum value that should be added or subtracted by the manipulation. If this value is set to 0 no mutation will be performed.", new DoubleValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticPolynomialMultiMoveGenerator(this, cloner);
    }

    public static AdditiveMove[] Apply(IRandom random, RealVector vector, double contiguity, int sampleSize, double maxManipulation, DoubleMatrix bounds) {
      AdditiveMove[] moves = new AdditiveMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        int index = random.Next(vector.Length);
        double strength = 0, min = bounds[index % bounds.Rows, 0], max = bounds[index % bounds.Rows, 1];
        do {
          strength = PolynomialOnePositionManipulator.Apply(random, contiguity) * maxManipulation;
        } while (vector[index] + strength < min || vector[index] + strength > max);
        moves[i] = new AdditiveMove(index, strength);
      }
      return moves;
    }

    protected override AdditiveMove[] GenerateMoves(IRandom random, RealVector realVector, DoubleMatrix bounds) {
      return Apply(random, realVector, ContiguityParameter.ActualValue.Value, SampleSizeParameter.ActualValue.Value, MaximumManipulationParameter.ActualValue.Value, bounds);
    }
  }
}
