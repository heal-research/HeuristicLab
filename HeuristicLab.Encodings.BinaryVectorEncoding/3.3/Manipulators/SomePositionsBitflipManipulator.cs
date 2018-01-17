#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Flips some bits of a binary vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 43.
  /// </remarks>
  [Item("SomePositionsBitflipManipulator", "Flips some bits of a binary vector, each position is flipped with a probability of pm. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 43.")]
  [StorableClass]
  public sealed class SomePositionsBitflipManipulator : BinaryVectorManipulator {
    /// <summary>
    /// Mmutation probability for each position.
    /// </summary>
    public IValueLookupParameter<DoubleValue> MutationProbabilityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MutationProbability"]; }
    }

    [StorableConstructor]
    private SomePositionsBitflipManipulator(bool deserializing) : base(deserializing) { }
    private SomePositionsBitflipManipulator(SomePositionsBitflipManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="NPointCrossover"/>
    /// </summary>
    public SomePositionsBitflipManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MutationProbability", "The mutation probability for each position", new DoubleValue(0.2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SomePositionsBitflipManipulator(this, cloner);
    }

    /// <summary>
    /// Performs the some positions bitflip mutation on a binary vector.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="vector">The vector that should be manipulated.</param>
    /// <param name="pm">The probability a bit is flipped.</param>
    public static void Apply(IRandom random, BinaryVector vector, DoubleValue pm) {
      for (int i = 0; i < vector.Length; i++) {
        if (random.NextDouble() < pm.Value) {
          vector[i] = !vector[i];
        }
      }
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, BinaryVector)"/>.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="realVector">The vector of binary values to manipulate.</param>
    protected override void Manipulate(IRandom random, BinaryVector binaryVector) {
      if (MutationProbabilityParameter.ActualValue == null) throw new InvalidOperationException("SomePositionsBitflipManipulator: Parameter " + MutationProbabilityParameter.ActualName + " could not be found.");
      Apply(random, binaryVector, MutationProbabilityParameter.ActualValue);
    }
  }
}
