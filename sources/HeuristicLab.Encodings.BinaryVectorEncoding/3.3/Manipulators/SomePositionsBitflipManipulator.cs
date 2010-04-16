using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Flips some bits of a binary vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 43.
  /// </remarks>
  [Item("SomePositionsBitflipManipulator", "Flips some bits of a binary vector, each position is flipped with a probability of pm. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 43.")]
  [StorableClass]
  public class SomePositionsBitflipManipulator : BinaryVectorManipulator {
    /// <summary>
    /// Mmutation probability for each position.
    /// </summary>
    public ValueLookupParameter<DoubleValue> MutationProbabilityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MutationProbability"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NPointCrossover"/>
    /// </summary>
    public SomePositionsBitflipManipulator() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MutationProbability", "The mutation probability for each position", new DoubleValue(0.2)));
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
      Apply(random, binaryVector, MutationProbabilityParameter.Value);
    }
  }
}
