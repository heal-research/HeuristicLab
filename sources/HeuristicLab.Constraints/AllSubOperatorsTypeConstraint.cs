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
using System.Xml;
using System.Diagnostics;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where all sub-operators have to be elements of a pre-defined group.
  /// </summary>
  public class AllSubOperatorsTypeConstraint : ConstraintBase {

    private SubOperatorTypeConstraint groupConstraint;
    /// <summary>
    /// Gets all allowed sub-operators.
    /// </summary>
    public IList<IOperator> AllowedSubOperators {
      get {
        return groupConstraint.AllowedSubOperators;
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "All sub-operators have to be elements of a pre-defined group."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AllSubOperatorsTypeConstraint"/>.
    /// </summary>
    public AllSubOperatorsTypeConstraint()
      : base() {
      groupConstraint = new SubOperatorTypeConstraint();
    }

    /// <summary>
    /// Adds the given operator to the constraint.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.FireChanged"/> of base class 
    /// <see cref="ConstraintBase"/>.</remarks>
    /// <param name="op">The operator to add.</param>
    public void AddOperator(IOperator op) {
      groupConstraint.AddOperator(op);
      FireChanged();
    }

    /// <summary>
    /// Removes the given operator from the contraint.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.FireChanged"/> of base class 
    /// <see cref="ConstraintBase"/>.</remarks>
    /// <param name="op">The operator to remove.</param>
    public void RemoveOperator(IOperator op) {
      groupConstraint.RemoveOperator(op);
      FireChanged();
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if(data == null) return false;

      for(int i = 0; i < op.SubOperators.Count; i++) {
        groupConstraint.SubOperatorIndex.Data = i;
        if(groupConstraint.Check(data) == false) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Empties the current instance.
    /// </summary>
    public void Clear() {
      groupConstraint.Clear();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="AllSubOperatorsTypeConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AllSubOperatorsTypeConstraint clone = new AllSubOperatorsTypeConstraint();
      clonedObjects.Add(Guid, clone);
      clone.groupConstraint = (SubOperatorTypeConstraint)Auxiliary.Clone(groupConstraint, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AllSubOperatorsTypeConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="AllSubOperatorsTypeConstraintView"/>.</returns>
    public override IView CreateView() {
      return new AllSubOperatorsTypeConstraintView(groupConstraint);
    }

    #region persistence
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The sub-operators are saved as a child node with tag name 
    /// <c>SubOperatorsGroupConstraint</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode subOperatorsNode = PersistenceManager.Persist("SubOperatorsGroupConstraint", groupConstraint, document, persistedObjects);
      node.AppendChild(subOperatorsNode);

      return node;
    }

    /// <summary>
    /// Loads the persisted constraint from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The constraint must be saved in a specific way, see <see cref="GetXmlNode"/> for 
    /// more information.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      groupConstraint = (SubOperatorTypeConstraint)PersistenceManager.Restore(node.SelectSingleNode("SubOperatorsGroupConstraint"), restoredObjects);
    }
    #endregion persistence
  }
}
