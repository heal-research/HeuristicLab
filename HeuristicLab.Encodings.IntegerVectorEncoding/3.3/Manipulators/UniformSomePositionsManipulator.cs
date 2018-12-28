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
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Uniformly distributed change of several, but at least one, positions of an integer vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformSomePositionsManipulator", "Uniformly distributed change of several, but at least one, positions of an integer vector. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableType("1A0F372D-7256-41EA-B825-9173BC1E278B")]
  public class UniformSomePositionsManipulator : BoundedIntegerVectorManipulator {

    public IValueLookupParameter<DoubleValue> ProbabilityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Probability"]; }
    }

    [StorableConstructor]
    protected UniformSomePositionsManipulator(StorableConstructorFlag _) : base(_) { }
    protected UniformSomePositionsManipulator(UniformSomePositionsManipulator original, Cloner cloner) : base(original, cloner) { }
    public UniformSomePositionsManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Probability", "The probability for each dimension to be manipulated.", new DoubleValue(0.5)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformSomePositionsManipulator(this, cloner);
    }

    /// <summary>
    /// Changes randomly several, but at least one, positions in the given integer <paramref name="vector"/>, according to the given probabilities.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <param name="bounds"> Contains the minimum value (inclusive), maximum value (exclusive), and step size of the sampling range for 
    /// the vector element to change.</param>
    /// <param name="probability">The probability for each dimension to be manipulated..</param>
    public static void Apply(IRandom random, IntegerVector vector, IntMatrix bounds, double probability) {
      if (bounds == null || bounds.Rows == 0 || bounds.Columns < 2) throw new ArgumentException("UniformSomePositionsManipulator: Invalid bounds specified", "bounds");
      bool atLeastOneManipulated = false;
      for (int index = 0; index < vector.Length; index++) {
        if (random.NextDouble() < probability) {
          atLeastOneManipulated = true;
          UniformOnePositionManipulator.Manipulate(random, vector, bounds, index);
        }
      }

      if (!atLeastOneManipulated) {
        UniformOnePositionManipulator.Manipulate(random, vector, bounds, random.Next(vector.Length));
      }
    }

    /// <summary>
    /// Changes randomly several, but at least one, positions in the given integer <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    protected override void ManipulateBounded(IRandom random, IntegerVector vector, IntMatrix bounds) {
      Apply(random, vector, bounds, ProbabilityParameter.ActualValue.Value);
    }
  }
}
