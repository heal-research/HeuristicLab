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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// An operator which creates a new random real vector with each element uniformly distributed in a specified range.
  /// </summary>
  [Item("UniformRandomRealVectorCreator", "An operator which creates a new random real vector with each element uniformly distributed in a specified range.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class UniformRandomRealVectorCreator : RealVectorCreator {
    /// <summary>
    /// Generates a new random real vector with the given <paramref name="length"/> and in the interval [min,max).
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="length"/> is smaller or equal to 0.<br />
    /// Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.
    /// </exception>
    /// <remarks>
    /// Note that if <paramref name="min"/> is equal to <paramref name="max"/>, all elements of the vector will be equal to <paramref name="min"/>.
    /// </remarks>
    /// <param name="random">The random number generator.</param>
    /// <param name="length">The length of the real vector.</param>
    /// <param name="min">The minimum value of the sampling range for each vector element (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for each vector element (exclusive).</param>
    /// <returns>The newly created real vector.</returns>
    public static DoubleArrayData Apply(IRandom random, int length, double min, double max) {
      if (length <= 0) throw new ArgumentException("UniformRandomRealVectorCreator: Length is smaller or equal to 0.", "length");
      if (min > max) throw new ArgumentException("UniformRandomRealVectorCreator: Minimum is greater than Maximum.", "min");
      DoubleArrayData result = new DoubleArrayData(length);
      for (int i = 0; i < length; i++)
        result[i] = min + random.NextDouble() * (max - min);
      return result;
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, int, double, double)"/>.
    /// </summary>
    /// <param name="random">The pseudo random number generator to use.</param>
    /// <param name="length">The length of the real vector.</param>
    /// <param name="minimum">The minimum value of the sampling range for each vector element (inclusive).</param>
    /// <param name="maximum">The maximum value of the sampling range for each vector element (exclusive).</param>
    /// <returns>The newly created real vector.</returns>
    protected override DoubleArrayData Create(IRandom random, IntData length, DoubleData minimum, DoubleData maximum) {
      return Apply(random, length.Value, minimum.Value, maximum.Value);
    }
  }
}
