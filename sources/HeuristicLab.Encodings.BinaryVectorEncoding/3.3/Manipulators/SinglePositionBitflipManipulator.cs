using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Flips exactly one bit of a binary vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("SinglePositionBitflipManipulator", "Flips exactly one bit of a binary vector. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class SinglePositionBitflipManipulator: BinaryVectorManipulator {
    /// <summary>
    /// Performs the single position bitflip mutation on a binary vector.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="vector">The vector that should be manipulated.</param>
    public static void Apply(IRandom random, BinaryVector vector) {
      int position = random.Next(vector.Length);

      vector[position] = !vector[position];
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, BinaryVector)"/>.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="realVector">The vector of binary values to manipulate.</param>
    protected override void Manipulate(IRandom random, BinaryVector binaryVector) {
      Apply(random, binaryVector);
    }
  }
}
