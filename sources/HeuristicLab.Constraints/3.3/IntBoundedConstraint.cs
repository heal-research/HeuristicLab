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
  /// Constraint where an integer value is limited by a one or two sided boundary.
  /// </summary>
  public class IntBoundedConstraint : ConstraintBase {

    [Storable]
    private int lowerBound;
    /// <summary>
    /// Gets or sets the lower bound of the limit.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public int LowerBound {
      get { return lowerBound; }
      set {
        lowerBound = value;
        OnChanged();
      }
    }

    [Storable]
    private bool lowerBoundIncluded;
    /// <summary>
    /// Gets or sets the boolean flag whether the lower bound should be included.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool LowerBoundIncluded {
      get { return lowerBoundIncluded; }
      set {
        lowerBoundIncluded = value;
        OnChanged();
      }
    }

    [Storable]
    private bool lowerBoundEnabled;
    /// <summary>
    /// Gets or sets the boolean flag whether the lower bound should be enabled.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool LowerBoundEnabled {
      get { return lowerBoundEnabled; }
      set {
        lowerBoundEnabled = value;
        OnChanged();
      }
    }

    [Storable]
    private int upperBound;
    /// <summary>
    /// Gets or sets the upper bound of the limit.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public int UpperBound {
      get { return upperBound; }
      set {
        upperBound = value;
        OnChanged();
      }
    }

    [Storable]
    private bool upperBoundIncluded;
    /// <summary>
    /// Gets or sets the boolean flag whether the upper bound should be included.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool UpperBoundIncluded {
      get { return upperBoundIncluded; }
      set {
        upperBoundIncluded = value;
        OnChanged();
      }
    }

    [Storable]
    private bool upperBoundEnabled;
    /// <summary>
    /// Gets or sets the boolean flag whether the upper bound should be enabled.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public bool UpperBoundEnabled {
      get { return upperBoundEnabled; }
      set {
        upperBoundEnabled = value;
        OnChanged();
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "The integer is limited one or two sided by a lower and/or upper boundary"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntBoundedConstraint"/>.
    /// </summary>
    public IntBoundedConstraint()
      : this(int.MinValue, int.MaxValue) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntBoundedConstraint"/> with the two given boundaries.
    /// </summary>
    /// <param name="low">The lower bound of the constraint.</param>
    /// <param name="high">The upper bound of the constraint.</param>
    public IntBoundedConstraint(int low, int high)
      : base() {
      lowerBound = low;
      lowerBoundIncluded = false;
      lowerBoundEnabled = true;
      upperBound = high;
      upperBoundIncluded = false;
      upperBoundEnabled = true;
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      ConstrainedIntData d = (data as ConstrainedIntData);
      if (d == null) return false;
      if (LowerBoundEnabled && ((LowerBoundIncluded && d.CompareTo(LowerBound) < 0)
        || (!LowerBoundIncluded && d.CompareTo(LowerBound) <= 0))) return false;
      if (UpperBoundEnabled && ((UpperBoundIncluded && d.CompareTo(UpperBound) > 0)
        || (!UpperBoundIncluded && d.CompareTo(UpperBound) >= 0))) return false;
      return true;
    }

    /// <summary>
    /// Creates a new instance of <see cref="IntBoundedConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="IntBoundedConstraintView"/>.</returns>
    public override IView CreateView() {
      return new IntBoundedConstraintView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="IntBoundedConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      IntBoundedConstraint clone = new IntBoundedConstraint();
      clonedObjects.Add(Guid, clone);
      clone.upperBound = UpperBound;
      clone.upperBoundIncluded = UpperBoundIncluded;
      clone.upperBoundEnabled = UpperBoundEnabled;
      clone.lowerBound = LowerBound;
      clone.lowerBoundIncluded = LowerBoundIncluded;
      clone.lowerBoundEnabled = LowerBoundEnabled;
      return clone;
    }
  }
}
