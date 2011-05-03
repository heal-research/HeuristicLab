#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Generates a new random binary vector with each element randomly initialized.
  /// </summary>
  [Item("RandomBinaryVectorCreator", "An operator which creates a new random binary vector with each element randomly initialized.")]
  [StorableClass]
  public sealed class RandomBinaryVectorCreator : BinaryVectorCreator {
    [StorableConstructor]
    private RandomBinaryVectorCreator(bool deserializing) : base(deserializing) { }
    private RandomBinaryVectorCreator(RandomBinaryVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public RandomBinaryVectorCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomBinaryVectorCreator(this, cloner);
    }

    /// <summary>
    /// Generates a new random binary vector with the given <paramref name="length"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="length">The length of the binary vector.</param>
    /// <returns>The newly created binary vector.</returns>
    public static BinaryVector Apply(IRandom random, int length) {
      BinaryVector result = new BinaryVector(length, random);
      return result;
    }

    protected override BinaryVector Create(IRandom random, IntValue length) {
      return Apply(random, length.Value);
    }
  }
}
