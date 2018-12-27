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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Uniformly distributed change of a single position in a real vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformOnePositionManipulator", "Changes a single position in the vector by sampling uniformly from the interval [Minimum_i, Maximum_i) in dimension i. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class UniformOnePositionManipulator : RealVectorManipulator {
    [StorableConstructor]
    protected UniformOnePositionManipulator(bool deserializing) : base(deserializing) { }
    protected UniformOnePositionManipulator(UniformOnePositionManipulator original, Cloner cloner) : base(original, cloner) { }
    public UniformOnePositionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformOnePositionManipulator(this, cloner);
    }
    
    /// <summary>
    /// Changes randomly a single position in the given real <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    public static void Apply(IRandom random, RealVector vector, DoubleMatrix bounds) {
      int index = random.Next(vector.Length);
      double min = bounds[index % bounds.Rows, 0];
      double max = bounds[index % bounds.Rows, 1];
      vector[index] = min + random.NextDouble() * (max - min);
    }

    /// <summary>
    /// Checks if the bounds parameters is available and forwards the call to <see cref="Apply(IRandom, RealVector, DoubleMatrix)"/>.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="realVector">The real vector to manipulate.</param>
    protected override void Manipulate(IRandom random, RealVector realVector) {
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("MichalewiczNonUniformOnePositionManipulator: Parameter " + BoundsParameter.ActualName + " could not be found.");
      Apply(random, realVector, BoundsParameter.ActualValue);
    }
  }
}
