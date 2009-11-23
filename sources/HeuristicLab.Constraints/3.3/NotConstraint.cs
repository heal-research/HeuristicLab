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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where its sub-constraint must be false to be true.
  /// </summary>
  public class NotConstraint : ConstraintBase {

    [Storable]
    private ConstraintBase subConstraint;
    /// <summary>
    /// Gets or sets the sub-constraint.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class 
    /// <see cref="ConstraintBase"/> in the setter.</remarks>
    public ConstraintBase SubConstraint {
      get { return subConstraint; }
      set {
        subConstraint = value;
        OnChanged();
      }
    }
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return "Requires that a constraint must be false to be true";
      }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="NotConstraint"/>.
    /// </summary>
    public NotConstraint()
      : base() {
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      return (subConstraint == null) || (!subConstraint.Check(data));
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="NotConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NotConstraint clone = new NotConstraint();
      clonedObjects.Add(Guid, clone);
      if (subConstraint != null)
        clone.SubConstraint = (ConstraintBase)SubConstraint.Clone();
      return clone;
    }
  }
}
