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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// The solution is manipulated with diminishing strength over time. In addition the mutated values are not sampled over the entire domain, but additive.<br/>
  /// Initially, the space will be searched uniformly and very locally at later stages. This increases the probability of generating the new numbers closer to the current value.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("MichalewiczNonUniformAllPositionsManipulator", "It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class MichalewiczNonUniformAllPositionsManipulator : RealVectorManipulator, IIterationBasedOperator {
    /// <summary>
    /// The current iteration.
    /// </summary>
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    /// <summary>
    /// The maximum iteration.
    /// </summary>
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    /// <summary>
    /// The parameter describing how much the mutation should depend on the progress towards the maximum iteration.
    /// </summary>
    public ValueLookupParameter<DoubleValue> IterationDependencyParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["IterationDependency"]; }
    }

    [StorableConstructor]
    protected MichalewiczNonUniformAllPositionsManipulator(bool deserializing) : base(deserializing) { }
    protected MichalewiczNonUniformAllPositionsManipulator(MichalewiczNonUniformAllPositionsManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="MichalewiczNonUniformAllPositionsManipulator"/> with three 
    /// parameters (<c>Iterations</c>, <c>MaximumIterations</c> and <c>IterationDependency</c>).
    /// </summary>
    public MichalewiczNonUniformAllPositionsManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "Current iteration of the algorithm"));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "Maximum number of iterations"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("IterationDependency", "Specifies the degree of dependency on the number of iterations. A value of 0 means no dependency and the higher the value the stronger the progress towards maximum iterations will be taken into account by sampling closer around the current position. Value must be >= 0.", new DoubleValue(5)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MichalewiczNonUniformAllPositionsManipulator(this, cloner);
    }

    /// <summary>
    /// Performs a non uniformly distributed all positions manipulation on the given 
    /// real <paramref name="vector"/>. The probability of stronger mutations reduces the more <see cref="currentIteration"/> approaches <see cref="maximumIterations"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="currentIteration"/> is greater than <paramref name="maximumIterations"/>.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <param name="currentIteration">The current iteration of the algorithm.</param>
    /// <param name="maximumIterations">Maximum number of iterations.</param>
    /// <param name="iterationDependency">Specifies the degree of dependency on the number of iterations. A value of 0 means no dependency and the higher the value the stronger the progress towards maximum iterations will be taken into account by sampling closer around the current position. Value must be >= 0.</param>
    /// <returns>The manipulated real vector.</returns>
    public static void Apply(IRandom random, RealVector vector, DoubleMatrix bounds, IntValue currentIteration, IntValue maximumIterations, DoubleValue iterationDependency) {
      if (currentIteration.Value > maximumIterations.Value) throw new ArgumentException("MichalewiczNonUniformAllPositionsManipulator: CurrentIteration must be smaller or equal than MaximumIterations", "currentIteration");
      if (iterationDependency.Value < 0) throw new ArgumentException("MichalewiczNonUniformAllPositionsManipulator: iterationDependency must be >= 0.", "iterationDependency");
      int length = vector.Length;

      double prob = Math.Pow(1 - currentIteration.Value / maximumIterations.Value, iterationDependency.Value);

      for (int i = 0; i < length; i++) {
        double min = bounds[i % bounds.Rows, 0];
        double max = bounds[i % bounds.Rows, 1];
        if (random.NextDouble() < 0.5) {
          vector[i] = vector[i] + (max - vector[i]) * (1 - Math.Pow(random.NextDouble(), prob));
        } else {
          vector[i] = vector[i] - (vector[i] - min) * (1 - Math.Pow(random.NextDouble(), prob));
        }
      }
    }

    /// <summary>
    /// Checks if all parameters are available and forwards the call to <see cref="Apply(IRandom, RealVector, DoubleValue, DoubleValue, IntValue, IntValue, DoubleValue)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="realVector">The real vector that should be manipulated.</param>
    protected override void Manipulate(IRandom random, RealVector realVector) {
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + BoundsParameter.ActualName + " could not be found.");
      if (IterationsParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + IterationsParameter.ActualName + " could not be found.");
      if (MaximumIterationsParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + MaximumIterationsParameter.ActualName + " could not be found.");
      if (IterationDependencyParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + IterationDependencyParameter.ActualName + " could not be found.");
      Apply(random, realVector, BoundsParameter.ActualValue, IterationsParameter.ActualValue, MaximumIterationsParameter.ActualValue, IterationDependencyParameter.ActualValue);
    }
  }
}
