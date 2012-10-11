#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Uniformly distributed change of a single position of an integer vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformOnePositionManipulator", " Uniformly distributed change of a single position of an integer vector. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class UniformOnePositionManipulator : BoundedIntegerVectorManipulator {

    [StorableConstructor]
    protected UniformOnePositionManipulator(bool deserializing) : base(deserializing) { }
    protected UniformOnePositionManipulator(UniformOnePositionManipulator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="UniformOnePositionManipulator"/> with two parameters
    /// (<c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public UniformOnePositionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformOnePositionManipulator(this, cloner);
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Bounds")) {
        var min = ((IValueLookupParameter<IntValue>)Parameters["Minimum"]).Value as IntValue;
        var max = ((IValueLookupParameter<IntValue>)Parameters["Maximum"]).Value as IntValue;
        Parameters.Remove("Minimum");
        Parameters.Remove("Maximum");
        Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "The bounds matrix can contain one row for each dimension with three columns specifying minimum (inclusive), maximum (exclusive), and step size. If less rows are given the matrix is cycled."));
        if (min != null && max != null) {
          BoundsParameter.Value = new IntMatrix(new int[,] { { min.Value, max.Value, 1 } });
        }
      }
    }
    #endregion

    /// <summary>
    /// Changes randomly a single position in the given integer <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <param name="min">The minimum value of the sampling range for 
    /// the vector element to change (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for
    /// the vector element to change (exclusive).</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    public static void Apply(IRandom random, IntegerVector vector, IntMatrix bounds) {
      Manipulate(random, vector, bounds, random.Next(vector.Length));
    }

    public static void Manipulate(IRandom random, IntegerVector vector, IntMatrix bounds, int index) {
      if (bounds == null || bounds.Rows == 0 || bounds.Columns < 2) throw new ArgumentException("UniformOnePositionManipulator: Invalid bounds specified", "bounds");
      int min = bounds[index % bounds.Rows, 0], max = bounds[index % bounds.Rows, 1], step = 1;
      if (min == max) {
        vector[index] = min;
      } else {
        if (bounds.Columns > 2) step = bounds[index % bounds.Rows, 2];
        // max has to be rounded to the lower feasible value
        // e.g. min...max / step = 0...100 / 5, max is exclusive so it would be 0..99
        // but 99 is not a feasible value, so max needs to be adjusted => min = 0, max = 95
        max = FloorFeasible(min, max, step, max - 1);
        vector[index] = RoundFeasible(min, max, step, random.Next(min, max));
      }
    }

    /// <summary>
    /// Changes randomly a single position in the given integer <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    protected override void ManipulateBounded(IRandom random, IntegerVector vector, IntMatrix bounds) {
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("UniformOnePositionManipulator: Parameter " + BoundsParameter.ActualName + " could not be found.");
      Apply(random, vector, bounds);
    }
  }
}
