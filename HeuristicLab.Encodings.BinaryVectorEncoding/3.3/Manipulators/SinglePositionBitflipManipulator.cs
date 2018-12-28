#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Flips exactly one bit of a binary vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("SinglePositionBitflipManipulator", "Flips exactly one bit of a binary vector. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableType("72BE48BD-F47C-4C81-81F6-FA42D496AC34")]
  public sealed class SinglePositionBitflipManipulator : BinaryVectorManipulator {

    [StorableConstructor]
    private SinglePositionBitflipManipulator(StorableConstructorFlag _) : base(_) { }
    private SinglePositionBitflipManipulator(SinglePositionBitflipManipulator original, Cloner cloner) : base(original, cloner) { }
    public SinglePositionBitflipManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SinglePositionBitflipManipulator(this, cloner);
    }

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
