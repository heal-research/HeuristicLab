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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Checks if all elements of a real vector are inside a given minimum and maximum value. 
  /// If not, the elements are corrected.
  /// </summary>
  [Item("BoundsChecker", "Checks if all elements of a real vector are inside a given minimum and maximum value. If not, elements are corrected.")]
  [StorableClass]
  public class BoundsChecker : SingleSuccessorOperator {
    public LookupParameter<DoubleArrayData> RealVectorParameter {
      get { return (LookupParameter<DoubleArrayData>)Parameters["RealVector"]; }
    }
    public ValueLookupParameter<DoubleData> MinimumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Minimum"]; }
    }
    public ValueLookupParameter<DoubleData> MaximumParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Maximum"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BoundsChecker"/> with three parameters
    /// (<c>RealVector</c>, <c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public BoundsChecker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleArrayData>("RealVector", "The real-valued vector for which the bounds should be checked."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Minimum", "The lower bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Maximum", "The upper bound for each element in the vector."));
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the given minimum 
    /// and maximum value and if not they are corrected.
    /// </summary>
    /// <param name="min">The minimum value of the range (inclusive).</param>
    /// <param name="max">The maximum value of the range (inclusive).</param>
    /// <param name="vector">The vector to check.</param>
    /// <returns>The corrected real vector.</returns>
    public static void Apply(DoubleArrayData vector, DoubleData min, DoubleData max) {
      for (int i = 0; i < vector.Length; i++) {
        if (vector[i] < min.Value) vector[i] = min.Value;
        if (vector[i] > max.Value) vector[i] = max.Value;
      }
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the given minimum 
    /// and maximum value and if not they are corrected.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when one of the three parameters (vector, minimum, or maximum) could not be found.</exception>
    /// <remarks>Calls <see cref="Apply(double, double, double[])"/>.</remarks>
    /// <inheritdoc select="returns" />
    public override IOperation Apply() {
      if (RealVectorParameter.ActualValue == null) throw new InvalidOperationException("BoundsChecker: Parameter " + RealVectorParameter.ActualName + " could not be found.");
      if (MinimumParameter.ActualValue == null) throw new InvalidOperationException("BoundsChecker: Parameter " + MinimumParameter.ActualName + " could not be found.");
      if (MaximumParameter.ActualValue == null) throw new InvalidOperationException("BoundsChecker: Parameter " + MaximumParameter.ActualName + " could not be found.");
      Apply(RealVectorParameter.ActualValue, MinimumParameter.ActualValue, MaximumParameter.ActualValue);
      return base.Apply();
    }
  }
}
