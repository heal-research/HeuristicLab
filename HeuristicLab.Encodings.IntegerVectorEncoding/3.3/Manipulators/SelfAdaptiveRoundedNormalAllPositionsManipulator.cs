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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Manipulates each dimension in the integer vector with the mutation strength given 
  /// in the strategy parameter vector.
  /// </summary>
  [Item("SelfAdaptiveRoundedNormalAllPositionsManipulator", "This manipulation operator adds a value sigma_i * N(0,1) to the current value in each position i. The resulting value is rounded to the next feasible value. The values for sigma_i are looked up dynamically. If there are less elements in the strategy vector than positions, then the strategy vector is cycled.")]
  [StorableClass]
  public class SelfAdaptiveRoundedNormalAllPositionsManipulator : BoundedIntegerVectorManipulator, ISelfAdaptiveManipulator {
    public Type StrategyParameterType {
      get { return typeof(IIntegerVectorStdDevStrategyParameterOperator); }
    }
    /// <summary>
    /// Parameter for the strategy vector.
    /// </summary>
    public ILookupParameter<DoubleArray> StrategyParameterParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["StrategyParameter"]; }
    }

    IParameter ISelfAdaptiveManipulator.StrategyParameterParameter {
      get { return StrategyParameterParameter; }
    }

    [StorableConstructor]
    protected SelfAdaptiveRoundedNormalAllPositionsManipulator(bool deserializing) : base(deserializing) { }
    protected SelfAdaptiveRoundedNormalAllPositionsManipulator(SelfAdaptiveRoundedNormalAllPositionsManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="SelfAdaptiveRoundedNormalAllPositionsManipulator"/> with one.
    /// </summary>
    public SelfAdaptiveRoundedNormalAllPositionsManipulator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleArray>("StrategyParameter", "The vector containing the endogenous strategy parameters."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SelfAdaptiveRoundedNormalAllPositionsManipulator(this, cloner);
    }

    /// <summary>
    /// Performs an adaptive normally distributed all position manipulation on the given 
    /// <paramref name="vector"/> and rounding the results to the next feasible value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the strategy vector is not
    /// as long as the vector to get manipulated.</exception>
    /// <param name="strategyParameters">The strategy vector determining the strength of the mutation.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    public static void Apply(IRandom random, IntegerVector vector, IntMatrix bounds, DoubleArray strategyParameters) {
      if (strategyParameters == null || strategyParameters.Length == 0) throw new ArgumentException("SelfAdaptiveRoundedNormalAllPositionsManipulator: Vector containing the standard deviations is not defined.", "sigma");
      if (bounds == null || bounds.Rows == 0 || bounds.Columns < 2) throw new ArgumentException("SelfAdaptiveRoundedNormalAllPositionsManipulator: Invalid bounds specified.", "bounds");
      var N = new NormalDistributedRandom(random, 0.0, 1.0);
      if (strategyParameters != null) {
        for (int i = 0; i < vector.Length; i++) {
          int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1], step = 1;
          if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];

          int value = (vector[i] + (int)Math.Round((N.NextDouble() * strategyParameters[i % strategyParameters.Length])) - min) / step;
          max = FloorFeasible(min, max, step, max - 1);
          vector[i] = RoundFeasible(min, max, step, value);
        }
      }
    }

    /// <summary>
    /// Checks that the strategy vector is not null and forwards the call to the static Apply method.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="vector">The vector of integer values that is manipulated.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    protected override void ManipulateBounded(IRandom random, IntegerVector vector, IntMatrix bounds) {
      Apply(random, vector, bounds, StrategyParameterParameter.ActualValue);
    }
  }
}
