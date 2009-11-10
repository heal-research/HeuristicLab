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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Base class for all items that are subjects to restrictions.
  /// </summary>
  public abstract class ConstrainedItemBase : ItemBase, IConstrainedItem {
    private List<IConstraint> myConstraints;
    /// <summary>
    /// Gets all current constraints.
    /// <note type="caution"> The constraints are returned read-only.</note>
    /// </summary>
    public virtual ICollection<IConstraint> Constraints {
      get { return myConstraints.AsReadOnly(); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedItemBase"/>.
    /// </summary>
    protected ConstrainedItemBase() {
      myConstraints = new List<IConstraint>();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.Clone
    /// (System.Collections.Generic.IDictionary&lt;System.Guid, object&gt;)"/> 
    /// of base class <see cref="ItemBase"/>.<br/>
    /// Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="ConstrainedItemBase"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedItemBase clone = (ConstrainedItemBase)base.Clone(clonedObjects);
      clone.myConstraints.Clear();
      foreach (IConstraint constraint in Constraints)
        clone.AddConstraint((IConstraint)Auxiliary.Clone(constraint, clonedObjects));
      return clone;
    }

    /// <summary>
    /// Adds the given <paramref name="constraint"/> to the current list.
    /// </summary>
    /// <remarks>Calls <see cref="OnConstraintAdded"/>.</remarks>
    /// <param name="constraint">The constraint to add.</param>
    public virtual void AddConstraint(IConstraint constraint) {
      myConstraints.Add(constraint);
      OnConstraintAdded(constraint);
    }
    /// <summary>
    /// Removes the given <paramref name="constraint"/> from the current list.
    /// </summary>
    /// <remarks>Calls <see cref="OnConstraintRemoved"/> if the constraint can be successfully removed.</remarks>
    /// <param name="constraint">The constraint to remove.</param>
    public virtual void RemoveConstraint(IConstraint constraint) {
      if (myConstraints.Remove(constraint))
        OnConstraintRemoved(constraint);
    }

    /// <summary>
    /// Checks all constraints of the current instance.
    /// </summary>
    /// <returns><c>true</c> if all constraints could be fulfilled, <c>false</c> otherwise.</returns>
    public bool IsValid() {
      bool result = true;
      foreach (IConstraint constraint in Constraints)
        result = result && constraint.Check(this);
      return result;
    }
    /// <summary>
    /// Checks all constraints of the current instance.
    /// </summary>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if all constraints could be fulfilled, <c>false</c> otherwise.</returns>
    public bool IsValid(out ICollection<IConstraint> violatedConstraints) {
      bool result = true;
      violatedConstraints = new List<IConstraint>();
      foreach (IConstraint constraint in Constraints) {
        if (!constraint.Check(this)) {
          result = false;
          violatedConstraints.Add(constraint);
        }
      }
      return result;
    }

    /// <summary>
    /// Creates an instance of <see cref="ConstrainedItemBaseView"/> 
    /// to represent the current instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="ConstrainedItemBaseView"/>.</returns>
    public override IView CreateView() {
      return new ConstrainedItemBaseView(this);
    }

    /// <summary>
    /// Occurs when a constraint is added.
    /// </summary>
    public event EventHandler<EventArgs<IConstraint>> ConstraintAdded;
    /// <summary>
    /// Fires a new <c>ConstraintAdded</c> event.
    /// </summary>
    /// <param name="constraint">The constraint that was added.</param>
    protected virtual void OnConstraintAdded(IConstraint constraint) {
      if (ConstraintAdded != null)
        ConstraintAdded(this, new EventArgs<IConstraint>(constraint));
    }
    /// <summary>
    /// Occurs when a constraint is removed.
    /// </summary>
    public event EventHandler<EventArgs<IConstraint>> ConstraintRemoved;
    /// <summary>
    /// Fires a new <c>ConstraintRemoved</c> event.
    /// </summary>
    /// <param name="constraint">The constraint that was removed.</param>
    protected virtual void OnConstraintRemoved(IConstraint constraint) {
      if (ConstraintRemoved != null)
        ConstraintRemoved(this, new EventArgs<IConstraint>(constraint));
    }

    #region Persistence Methods  
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>. <br/>
    /// For saving the constraints a child node is created having the tag name <c>Constraints</c>. Beyond this
    /// child node all constraints are saved as child nodes themselves.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      if (Constraints.Count > 0) {
        XmlNode constraintsNode = document.CreateNode(XmlNodeType.Element, "Constraints", null);
        foreach (IConstraint constraint in Constraints)
          constraintsNode.AppendChild(PersistenceManager.Persist(constraint, document, persistedObjects));
        node.AppendChild(constraintsNode);
      }
      return node;
    }
    /// <summary>
    /// Loads the persisted item from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>See <see cref="GetXmlNode"/> to get information about how the constrained item must
    /// be saved. <br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the constrained item is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode constraintsNode = node.SelectSingleNode("Constraints");
      if (constraintsNode != null) {
        myConstraints.Clear();
        foreach (XmlNode constraintNode in constraintsNode.ChildNodes)
          AddConstraint((IConstraint)PersistenceManager.Restore(constraintNode, restoredObjects));
      }
    }
    #endregion
  }
}
