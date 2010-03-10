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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Uniformly distributed change of a single position in a real vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformOnePositionManipulator", "Changes a single position in the vector by sampling uniformly from the interval [Minimum, Maximum). It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass(StorableClassType.Empty)]
  public class UniformOnePositionManipulator : RealVectorManipulator {
    /// <summary>
    /// The lower bound of the values in the real vector.
    /// </summary>
    public ValueLookupParameter<DoubleData> MinimumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Minimum"]; }
    }
    /// <summary>
    /// The upper bound of the values in the real vector.
    /// </summary>
    public ValueLookupParameter<DoubleData> MaximumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Maximum"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformOnePositionManipulator"/> with two parameters
    /// (<c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public UniformOnePositionManipulator() {
      Parameters.Add(new ValueLookupParameter<DoubleData>("Minimum", "Minimum of the sampling range for the vector element (included)"));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Maximum", "Maximum of the sampling range for the vector element (excluded)"));
    }

    /// <summary>
    /// Changes randomly a single position in the given real <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="min">The minimum value of the sampling range for 
    /// the vector element to change (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for
    /// the vector element to change (exclusive).</param>
    public static void Apply(IRandom random, DoubleArrayData vector, DoubleData min, DoubleData max) {
      int index = random.Next(vector.Length);
      vector[index] = min.Value + random.NextDouble() * (max.Value - min.Value);
    }

    /// <summary>
    /// Checks if the minimum and maximum parameters are available and forwards the call to <see cref="Apply(IRandom, DoubleArrayData, DoubleData, DoubleData)"/>.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="realVector">The real vector to manipulate.</param>
    protected override void Manipulate(IRandom random, DoubleArrayData realVector) {
      if (MinimumParameter.ActualValue == null) throw new InvalidOperationException("UniformOnePositionManipulator: Parameter " + MinimumParameter.ActualName + " could not be found.");
      if (MaximumParameter.ActualValue == null) throw new InvalidOperationException("UniformOnePositionManipulator: Parameter " + MaximumParameter.ActualName + " could not be found.");
      Apply(random, realVector, MinimumParameter.ActualValue, MaximumParameter.ActualValue);
    }
  }
}
