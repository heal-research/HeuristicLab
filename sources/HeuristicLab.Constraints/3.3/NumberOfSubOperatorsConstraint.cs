#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.Data;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where the number of sub-operators must be within a specific range.
  /// </summary>
  public class NumberOfSubOperatorsConstraint : ConstraintBase {

    [Storable]
    private IntData minOperators;

    [Storable]
    private IntData maxOperators;

    /// <summary>
    /// Gets the maximum number of sub-operators.
    /// </summary>
    public IntData MaxOperators {
      get { return maxOperators; }
    }

    /// <summary>
    /// Gets the minimum number of sub-operators.
    /// </summary>
    public IntData MinOperators {
      get { return minOperators; }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Number of sub-operators has to be between " + MinOperators.ToString() + " and " + MaxOperators.ToString() + "."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumberOfSubOperatorsConstraint"/>.
    /// </summary>
    public NumberOfSubOperatorsConstraint()
      : this(0, 0) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumberOfSubOperatorsConstraint"/> with the minimum and 
    /// the maximum number of sub-operators.
    /// </summary>
    /// <param name="min">The minimum number of sub-operators.</param>
    /// <param name="max">The maximum number of sub-operators.</param>
    public NumberOfSubOperatorsConstraint(int min, int max)
      : base() {
      minOperators = new IntData(min);
      maxOperators = new IntData(max);
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if (data == null) return false;

      return (op.SubOperators.Count >= minOperators.Data && op.SubOperators.Count <= maxOperators.Data);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="AndConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NumberOfSubOperatorsConstraint clone = new NumberOfSubOperatorsConstraint();
      clonedObjects.Add(Guid, clone);
      clone.maxOperators.Data = maxOperators.Data;
      clone.minOperators.Data = minOperators.Data;
      return clone;
    }

    /// <summary>
    /// Creates a new instance of <see cref="NumberOfSubOperatorsConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="NumberOfSubOperatorsConstraintView"/>.</returns>
    public override IView CreateView() {
      return new NumberOfSubOperatorsConstraintView(this);
    }
  }
}
