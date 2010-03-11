#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Changes one position of a real vector by adding/substracting a value of the interval [(2^-15)*range;~2*range], where range is SearchIntervalFactor * (max - min).
  /// Note that the interval is not uniformly sampled, but smaller values are sampled more often.
  /// </summary>
  /// <remarks>
  /// It is implemented as described by Mühlenbein, H. and Schlierkamp-Voosen, D. 1993. Predictive Models for the Breeder Genetic Algorithm - I. Continuous Parameter Optimization. Evolutionary Computation, 1(1), pp. 25-49.<br/>
  /// </remarks>
  [Item("BreederGeneticAlgorithmManipulator", "It is implemented as described by Mühlenbein, H. and Schlierkamp-Voosen, D. 1993. Predictive Models for the Breeder Genetic Algorithm - I. Continuous Parameter Optimization. Evolutionary Computation, 1(1), pp. 25-49.")]
  [StorableClass(StorableClassType.Empty)]
  public class BreederGeneticAlgorithmManipulator : RealVectorManipulator {
    private static readonly double[] powerOfTwo = new double[] { 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625, 0.0078125, 0.00390625, 0.001953125, 0.0009765625, 0.00048828125, 0.000244140625, 0.0001220703125, 0.00006103515625, 0.000030517578125 };
    public ValueLookupParameter<DoubleData> MinimumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Minimum"]; }
    }
    public ValueLookupParameter<DoubleData> MaximumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Maximum"]; }
    }
    public ValueLookupParameter<DoubleData> SearchIntervalFactorParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["SearchIntervalFactor"]; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="BreederGeneticAlgorithmManipulator"/> with three variable
    /// infos (<c>Minimum</c>, <c>Maximum</c> and <c>SearchIntervalFactor</c>).
    /// </summary>
    public BreederGeneticAlgorithmManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleData>("Minimum", "The lower bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Maximum", "The upper bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("SearchIntervalFactor", "The factor determining the size of the search interval, that will be added/removed to/from the allele selected for manipulation.", new DoubleData(0.1)));
    }

    /// <summary>
    /// Performs a Breeder Genetic Algorithm Manipulation on the given <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="min">The minimum number of the sampling range for the vector element (inclusive).</param>
    /// <param name="max">The maximum number of the sampling range for the vector element (exclusive).</param>
    /// <param name="searchIntervalFactor">The factor determining the size of the search interval.</param>
    public static void Apply(IRandom random, DoubleArrayData vector, DoubleData min, DoubleData max, DoubleData searchIntervalFactor) {
      int length = vector.Length;
      double prob, value;
      do {
        value = Sigma(random);
      } while (value == 0);
      value *= searchIntervalFactor.Value * (max.Value - min.Value);

      prob = 1.0 / (double)length;
      bool wasMutated = false;

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < prob) {
          if (random.NextDouble() < 0.5) {
            vector[i] = vector[i] + value;
          } else {
            vector[i] = vector[i] - value;
          }
          wasMutated = true;
        }
      }

      // make sure at least one gene was mutated
      if (!wasMutated) {
        int pos = random.Next(length);
        if (random.NextDouble() < 0.5) {
          vector[pos] = vector[pos] + value;
        } else {
          vector[pos] = vector[pos] - value;
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
    /// Checks the parameters Minimum, Maximum, and SearchIntervalFactor and forwards the call to <see cref="Apply(IRandom, DoubleArrayData, DoubleData, DoubleData, DoubleData)"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="realVector">The real vector to manipulate.</param>
    protected override void Manipulate(IRandom random, DoubleArrayData realVector) {
      if (MinimumParameter.ActualValue == null) throw new InvalidOperationException("BreederGeneticAlgorithmManipulator: Parameter " + MinimumParameter.ActualName + " could not be found.");
      if (MaximumParameter.ActualValue == null) throw new InvalidOperationException("BreederGeneticAlgorithmManipulator: Paraemter " + MaximumParameter.ActualName + " could not be found.");
      if (SearchIntervalFactorParameter.ActualValue == null) throw new InvalidOperationException("BreederGeneticAlgorithmManipulator: Paraemter " + SearchIntervalFactorParameter.ActualName + " could not be found.");
      Apply(random, realVector, MinimumParameter.ActualValue, MaximumParameter.ActualValue, SearchIntervalFactorParameter.ActualValue);
    }
  }
}
