#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Checks if all elements of a real vector are inside the bounds.
  /// If not, elements are set to the respective values of the bounds.
  /// </summary>
  [Item("BoundsChecker", "Checks if all elements of a real vector are inside the bounds. If not, elements are set to the respective values of the bounds.")]
  [StorableType("9A3D7E10-6138-4B36-A284-BDDE8ECDBF36")]
  public class BoundsChecker : SingleSuccessorOperator, IRealVectorBoundsChecker {
    public LookupParameter<RealVector> RealVectorParameter {
      get { return (LookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected BoundsChecker(StorableConstructorFlag _) : base(_) { }
    protected BoundsChecker(BoundsChecker original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="BoundsChecker"/> with two parameters
    /// (<c>RealVector</c>, <c>Bounds</c>).
    /// </summary>
    public BoundsChecker()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The real-valued vector for which the bounds should be checked."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoundsChecker(this, cloner);
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the bounds and if not, elements are set to the respective values of the bounds.
    /// </summary>
    /// <param name="bounds">The lower and upper bound (1st and 2nd column) of the positions in the vector. If there are less rows than dimensions, the rows are cycled.</param>
    /// <param name="vector">The vector to check.</param>
    /// <returns>The corrected real vector.</returns>
    public static void Apply(RealVector vector, DoubleMatrix bounds) {
      for (int i = 0; i < vector.Length; i++) {
        double min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1];
        if (vector[i] < min) vector[i] = min;
        if (vector[i] > max) vector[i] = max;
      }
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the bounds and if not, elements are set to the respective values of the bounds.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when either vector or bounds could not be found.</exception>
    /// <remarks>Calls <see cref="Apply(RealVector, DoubleMatrix)"/>.</remarks>
    /// <inheritdoc select="returns" />
    public override IOperation Apply() {
      if (RealVectorParameter.ActualValue == null) throw new InvalidOperationException("BoundsChecker: Parameter " + RealVectorParameter.ActualName + " could not be found.");
      if (BoundsParameter.ActualValue == null) throw new InvalidOperationException("BoundsChecker: Parameter " + BoundsParameter.ActualName + " could not be found.");
      Apply(RealVectorParameter.ActualValue, BoundsParameter.ActualValue);
      return base.Apply();
    }
  }
}
