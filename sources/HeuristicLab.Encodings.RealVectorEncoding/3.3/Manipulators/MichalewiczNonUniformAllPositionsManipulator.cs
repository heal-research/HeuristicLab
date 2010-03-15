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
  /// The solution is manipulated with diminishing strength over time. In addition the mutated values are not sampled over the entire domain, but additive.<br/>
  /// Initially, the space will be searched uniformly and very locally at later stages. This increases the probability of generating the new numbers closer to the current value.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("MichalewiczNonUniformOnePositionManipulator", "It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class MichalewiczNonUniformAllPositionsManipulator : RealVectorManipulator {
    /// <summary>
    /// The lower bound of the values in the real vector.
    /// </summary>
    public ValueLookupParameter<DoubleValue> MinimumParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Minimum"]; }
    }
    /// <summary>
    /// The upper bound of the values in the real vector.
    /// </summary>
    public ValueLookupParameter<DoubleValue> MaximumParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Maximum"]; }
    }
    /// <summary>
    /// The current generation.
    /// </summary>
    public LookupParameter<IntValue> GenerationParameter {
      get { return (LookupParameter<IntValue>)Parameters["Generation"]; }
    }
    /// <summary>
    /// The maximum generation.
    /// </summary>
    public LookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (LookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    /// <summary>
    /// The parameter describing how much the mutation should depend on the progress towards the maximum generation.
    /// </summary>
    public ValueLookupParameter<DoubleValue> GenerationDependencyParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["GenerationDependency"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="MichalewiczNonUniformAllPositionsManipulator"/> with
    /// five parameters (<c>Minimum</c>, <c>Maximum</c>, <c>CurrentGeneration</c>, 
    /// <c>MaximumGenerations</c> and <c>GenerationDependency</c>).
    /// </summary>
    public MichalewiczNonUniformAllPositionsManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Minimum", "Minimum of the sampling range for the vector element (included)"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Maximum", "Maximum of the sampling range for the vector element (excluded)"));
      Parameters.Add(new LookupParameter<IntValue>("Generation", "Current generation of the algorithm"));
      Parameters.Add(new LookupParameter<IntValue>("MaximumGenerations", "Maximum number of generations"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("GenerationDependency", "Specifies the degree of dependency on the number of generations", new DoubleValue(5)));
    }

    /// <summary>
    /// Performs a non uniformly distributed all position manipulation on the given 
    /// real <paramref name="vector"/>. The probability of stronger mutations reduces the more <see cref="currentGeneration"/> approaches <see cref="maximumGenerations"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="currentGeneration"/> is greater than <paramref name="maximumGenerations"/>.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="min">The minimum value of the sampling range for the vector element (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for the vector element (exclusive).</param>
    /// <param name="currentGeneration">The current generation of the algorithm.</param>
    /// <param name="maximumGenerations">Maximum number of generations.</param>
    /// <param name="generationsDependency">Specifies the degree of dependency on the number of generations.</param>
    /// <returns>The manipulated real vector.</returns>
    public static void Apply(IRandom random, DoubleArray vector, DoubleValue min, DoubleValue max, IntValue currentGeneration, IntValue maximumGenerations, DoubleValue generationsDependency) {
      if (currentGeneration.Value > maximumGenerations.Value) throw new ArgumentException("MichalewiczNonUniformAllPositionManipulator: CurrentGeneration must be smaller or equal than MaximumGeneration", "currentGeneration");
      int length = vector.Length;

      double prob = Math.Pow(1 - currentGeneration.Value / maximumGenerations.Value, generationsDependency.Value);

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5) {
          vector[i] = vector[i] + (max.Value - vector[i]) * (1 - Math.Pow(random.NextDouble(), prob));
        } else {
          vector[i] = vector[i] - (vector[i] - min.Value) * (1 - Math.Pow(random.NextDouble(), prob));
        }
      }
    }

    /// <summary>
    /// Checks if all parameters are available and forwards the call to <see cref="Apply(IRandom, DoubleArray, DoubleValue, DoubleValue, IntValue, IntValue, DoubleValue)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="realVector">The real vector that should be manipulated.</param>
    protected override void Manipulate(IRandom random, DoubleArray realVector) {
      if (MinimumParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + MinimumParameter.ActualName + " could not be found.");
      if (MaximumParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + MaximumParameter.ActualName + " could not be found.");
      if (GenerationParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + GenerationParameter.ActualName + " could not be found.");
      if (MaximumGenerationsParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + MaximumGenerationsParameter.ActualName + " could not be found.");
      if (GenerationDependencyParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformAllPositionManipulator: Parameter " + GenerationDependencyParameter.ActualName + " could not be found.");
      Apply(random, realVector, MinimumParameter.ActualValue, MaximumParameter.ActualValue, GenerationParameter.ActualValue, MaximumGenerationsParameter.ActualValue, GenerationDependencyParameter.ActualValue);
    }
  }
}
