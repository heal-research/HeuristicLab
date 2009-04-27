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
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint that compares variables in a <see cref="ConstrainedItemList"/>.
  /// </summary>
  public class VariableComparisonConstraint : ConstraintBase {

    [Storable]
    private StringData leftVarName;
    /// <summary>
    /// Gets or sets the variable name of the left item to compare.
    /// </summary>
    public StringData LeftVarName {
      get { return leftVarName; }
      set { leftVarName = value; }
    }

    [Storable]
    private StringData rightVarName;
    /// <summary>
    /// Gets or sets the variable name of the right item to compare.
    /// </summary>
    public StringData RightVarName {
      get { return rightVarName; }
      set { rightVarName = value; }
    }

    [Storable]
    private IntData comparer;
    /// <summary>
    /// Gets or sets the comparer.
    /// </summary>
    public IntData Comparer {
      get { return comparer; }
      set { comparer = value; }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Compares variables in a ConstrainedItemList";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableComparisonConstraint"/>.
    /// </summary>
    public VariableComparisonConstraint() {
      leftVarName = new StringData();
      rightVarName = new StringData();
      comparer = new IntData(-1);
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the data is no <c>ConstrainedItemList</c>.</exception>
    /// <exception cref="InvalidCastException">Thrown when the left varible is not of type <c>IComparable</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the comparer is undefined.</exception>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list == null) throw new InvalidOperationException("ERROR in VariableComparisonConstraint: Can only be applied on ConstrainedItemLists");
      IComparable left = null;
      IItem right = null;
      foreach (IItem item in list) {
        Variable tmp = (item as Variable);
        if (tmp != null && tmp.Name.Equals(leftVarName.Data)) {
          left = (tmp.Value as IComparable);
          if (left == null) throw new InvalidCastException("ERROR in VariableComparisonConstraint: Value of the variable on the left side needs to be of type IComparable");
        } else if (tmp != null && tmp.Name.Equals(rightVarName.Data)) {
          right = tmp.Value;
        }
      }
      if (left != null && right != null) {
        switch (comparer.Data) {
          case 0:
            return left.CompareTo(right) < 0;
          case 1:
            return left.CompareTo(right) <= 0;
          case 2:
            return left.CompareTo(right) == 0;
          case 3:
            return left.CompareTo(right) >= 0;
          case 4:
            return left.CompareTo(right) > 0;
          default:
            throw new InvalidOperationException("ERROR in VariableComparisonConstraint: Comparer undefined");
        }
      }
      return true;
    }

    /// <summary>
    /// Creates a new instance of <see cref="VariableComparisonConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="VariableComparisonConstraintView"/>.</returns>
    public override IView CreateView() {
      return new VariableComparisonConstraintView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="VariableComparisonConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableComparisonConstraint clone = new VariableComparisonConstraint();
      clonedObjects.Add(Guid, clone);
      clone.LeftVarName = (StringData)Auxiliary.Clone(LeftVarName, clonedObjects);
      clone.RightVarName = (StringData)Auxiliary.Clone(RightVarName, clonedObjects);
      clone.Comparer = (IntData)Auxiliary.Clone(Comparer, clonedObjects);
      return clone;
    }
  }
}
