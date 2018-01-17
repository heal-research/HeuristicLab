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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// An operator which creates a new random real vector with each element uniformly distributed in a specified range.
  /// </summary>
  [Item("UniformRandomRealVectorCreator", "An operator which creates a new random real vector with each element uniformly distributed in a specified range.")]
  [StorableClass]
  public class UniformRandomRealVectorCreator : RealVectorCreator, IStrategyParameterCreator {
    [StorableConstructor]
    protected UniformRandomRealVectorCreator(bool deserializing) : base(deserializing) { }
    protected UniformRandomRealVectorCreator(UniformRandomRealVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public UniformRandomRealVectorCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformRandomRealVectorCreator(this, cloner);
    }
    
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
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <returns>The newly created real vector.</returns>
    public static RealVector Apply(IRandom random, int length, DoubleMatrix bounds) {
      RealVector result = new RealVector(length);
      result.Randomize(random, bounds);
      return result;
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, int, DoubleMatrix)"/>.
    /// </summary>
    /// <param name="random">The pseudo random number generator to use.</param>
    /// <param name="length">The length of the real vector.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <returns>The newly created real vector.</returns>
    protected override RealVector Create(IRandom random, IntValue length, DoubleMatrix bounds) {
      return Apply(random, length.Value, bounds);
    }
  }
}
