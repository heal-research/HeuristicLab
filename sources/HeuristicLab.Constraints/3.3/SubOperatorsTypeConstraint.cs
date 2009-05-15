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
using HeuristicLab.Data;
using System.Diagnostics;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where the sub-operator at a specific index has to be an element of a pre-defined group.
  /// </summary>
  public class SubOperatorTypeConstraint : ConstraintBase {

    [Storable]
    private IntData subOperatorIndex;
    /// <summary>
    /// Gets the index of the sub-operator.
    /// </summary>
    public IntData SubOperatorIndex {
      get { return subOperatorIndex; }
    }

    [Storable]
    private List<IOperator> subOperators;
    /// <summary>
    /// Gets all allowed sub-operators.
    /// </summary>
    public IList<IOperator> AllowedSubOperators {
      get {
        return subOperators.AsReadOnly();
      }
    }

    ///<inheritdoc select="summary"/>
    public override string Description {
      get { return "The sub-operator at a specific index has to be an element of a pre-defined group."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SubOperatorTypeConstraint"/>.
    /// </summary>
    public SubOperatorTypeConstraint()
      : base() {
      subOperatorIndex = new IntData(0);
      subOperators = new List<IOperator>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SubOperatorTypeConstraint"/> with the given
    /// <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the sub-operator.</param>
    public SubOperatorTypeConstraint(int index)
      : base() {
      subOperatorIndex = new IntData(index);
      subOperators = new List<IOperator>();
    }

    /// <summary>
    /// Adds the given operator to the list of sub-operators.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.FireChanged"/> of base class <see cref="ConstraintBase"/>.</remarks>
    /// <param name="op">The operator to add.</param>
    public void AddOperator(IOperator op) {
      if (!subOperators.Contains(op)) {
        subOperators.Add(op);
        FireChanged();
      }
    }

    /// <summary>
    /// Removes the given operator from the list of sub-operators.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.FireChanged"/> of base class <see cref="ConstraintBase"/>.</remarks>
    /// <param name="op">The operator to remove.</param>
    public void RemoveOperator(IOperator op) {
      if (subOperators.Contains(op)) {
        subOperators.Remove(op);
        FireChanged();
      }
    }

    /// <summary>
    /// Empties the list of sub-operators.
    /// </summary>
    public void Clear() {
      subOperators.Clear();
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if (data == null) return false;

      if (op.SubOperators.Count <= subOperatorIndex.Data) {
        return false;
      }
      return subOperators.Contains(op.SubOperators[subOperatorIndex.Data]);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="SubOperatorTypeConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SubOperatorTypeConstraint clone = new SubOperatorTypeConstraint();
      clonedObjects.Add(Guid, clone);
      clone.subOperatorIndex.Data = subOperatorIndex.Data;
      foreach (IOperator op in subOperators) {
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      }
      return clone;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SubOperatorsTypeConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="SubOperatorsTypeConstraintView"/>.</returns>
    public override IView CreateView() {
      return new SubOperatorsTypeConstraintView(this);
    }
  }
}
