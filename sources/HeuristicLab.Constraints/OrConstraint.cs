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

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint where at least one sub-constraint must be true.
  /// </summary>
  public class OrConstraint : ConstraintBase, IViewable {
    private ItemList<IConstraint> clauses;
    /// <summary>
    /// Gets or sets the sub-constraints.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class 
    /// <see cref="ConstraintBase"/> in the setter.</remarks>
    public ItemList<IConstraint> Clauses {
      get { return clauses; }
      set {
        clauses = value;
        OnChanged();
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return "To return true, at least one subconstraint has to be true";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OrConstraint"/>.
    /// </summary>
    public OrConstraint() {
      clauses = new ItemList<IConstraint>();
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      bool result = false;
      for (int i = 0 ; i < clauses.Count ; i++) {
        result = clauses[i].Check(data);
        if (result) return true;
      }
      return result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="OrConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="OrConstraintView"/>.</returns>
    public override IView CreateView() {
      return new OrConstraintView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OrConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OrConstraint clone = new OrConstraint();
      clonedObjects.Add(Guid, clone);
      clone.Clauses = (ItemList<IConstraint>)Auxiliary.Clone(Clauses, clonedObjects);
      return clone;
    }

    #region persistence
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The sub-constraints are saved as a child node with tag name 
    /// <c>Clauses</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode clausesNode = PersistenceManager.Persist("Clauses", Clauses, document, persistedObjects);
      node.AppendChild(clausesNode);

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
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      clauses = (ItemList<IConstraint>)PersistenceManager.Restore(node.SelectSingleNode("Clauses"), restoredObjects);
    }
    #endregion persistence
  }
}
