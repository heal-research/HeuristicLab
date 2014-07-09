#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Changes one position of a real vector by adding/substracting a value of the interval [(2^-15)*range;~2*range], where range is SearchIntervalFactor * (max - min).
  /// Note that the interval is not uniformly sampled, but smaller values are sampled more often.
  /// </summary>
  /// <remarks>
  /// It is implemented as described by Mühlenbein, H. and Schlierkamp-Voosen, D. 1993. Predictive Models for the Breeder Genetic Algorithm - I. Continuous Parameter Optimization. Evolutionary Computation, 1(1), pp. 25-49.<br/>
  /// </remarks>
  [Item("BreederGeneticAlgorithmManipulator", "It is implemented as described by Mühlenbein, H. and Schlierkamp-Voosen, D. 1993. Predictive Models for the Breeder Genetic Algorithm - I. Continuous Parameter Optimization. Evolutionary Computation, 1(1), pp. 25-49.")]
  [StorableClass]
  public class BreederGeneticAlgorithmManipulator : RealVectorManipulator {
    private static readonly double[] powerOfTwo = new double[] { 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625, 0.0078125, 0.00390625, 0.001953125, 0.0009765625, 0.00048828125, 0.000244140625, 0.0001220703125, 0.00006103515625, 0.000030517578125 };
    public ValueLookupParameter<DoubleValue> SearchIntervalFactorParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SearchIntervalFactor"]; }
    }

    [StorableConstructor]
    protected BreederGeneticAlgorithmManipulator(bool deserializing) : base(deserializing) { }
    protected BreederGeneticAlgorithmManipulator(BreederGeneticAlgorithmManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="BreederGeneticAlgorithmManipulator"/> with two
    /// parameters (<c>Bounds</c> and <c>SearchIntervalFactor</c>).
    /// </summary>
    public BreederGeneticAlgorithmManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SearchIntervalFactor", @"Scales the manipulation strength as a factor of the range. The range is determined by the bounds interval.
A value of 0.1 means that certain components of the vector are moved by values in the non-uniform interval [0;0.1*range].", new DoubleValue(0.1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BreederGeneticAlgorithmManipulator(this, cloner);
    }

    /// <summary>
    /// Performs a breeder genetic algorithm manipulation on the given <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <param name="searchIntervalFactor">The factor determining the size of the search interval.</param>
    public static void Apply(IRandom random, RealVector vector, DoubleMatrix bounds, DoubleValue searchIntervalFactor) {
      int length = vector.Length;
      double prob, value;
      do {
        value = Sigma(random);
      } while (value == 0);

      prob = 1.0 / (double)length;
      bool wasMutated = false;

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < prob) {
          double range = bounds[i % bounds.Rows, 1] - bounds[i % bounds.Rows, 0];
          if (random.NextDouble() < 0.5) {
            vector[i] = vector[i] + value * searchIntervalFactor.Value * range;
          } else {
            vector[i] = vector[i] - value * searchIntervalFactor.Value * range;
          }
          wasMutated = true;
        }
      }

      // make sure at least one gene was mutated
      if (!wasMutated) {
        int pos = random.Next(length);
        double range = bounds[pos % bounds.Rows, 1] - bounds[pos % bounds.Rows, 0];
        if (random.NextDouble() < 0.5) {
          vector[pos] = vector[pos] + value * searchIntervalFactor.Value * range;
        } else {
          vector[pos] = vector[pos] - value * searchIntervalFactor.Value * range;
        }
      }
    }

    private static double Sigma(IRandom random) {
      double sigma = 0;
      int limit = 16;

      for (int i = 0; i < limit; i++) {
        if (random.Next(limit) == 15) {
          // execute this statement with a probability of 1/16
          sigma += powerOfTwo[i];
        }
      }

      return sigma;
    }

    /// <summary>
    /// Checks the parameters Bounds, and SearchIntervalFactor and forwards the call to <see cref="Apply(IRandom, RealVector, DoubleValue, DoubleValue, DoubleValue)"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="realVector">The real vector to manipulate.</param>
    protected override void Manipulate(IRandom random, RealVector realVector) {
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("BreederGeneticAlgorithmManipulator: Parameter " + BoundsParameter.ActualName + " could not be found.");
      if (SearchIntervalFactorParameter.ActualValue == null) throw new InvalidOperationException("BreederGeneticAlgorithmManipulator: Paraemter " + SearchIntervalFactorParameter.ActualName + " could not be found.");
      Apply(random, realVector, BoundsParameter.ActualValue, SearchIntervalFactorParameter.ActualValue);
    }
  }
}
