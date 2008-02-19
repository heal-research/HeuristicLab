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

namespace HeuristicLab.Core {
  public abstract class ConstrainedItemBase : ItemBase, IConstrainedItem {
    private List<IConstraint> myConstraints;
    public virtual ICollection<IConstraint> Constraints {
      get { return myConstraints.AsReadOnly(); }
    }

    protected ConstrainedItemBase() {
      myConstraints = new List<IConstraint>();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedItemBase clone = (ConstrainedItemBase)base.Clone(clonedObjects);
      clone.myConstraints.Clear();
      foreach (IConstraint constraint in Constraints)
        clone.AddConstraint((IConstraint)Auxiliary.Clone(constraint, clonedObjects));
      return clone;
    }

    public virtual void AddConstraint(IConstraint constraint) {
      myConstraints.Add(constraint);
      OnConstraintAdded(constraint);
    }
    public virtual void RemoveConstraint(IConstraint constraint) {
      if (myConstraints.Remove(constraint))
        OnConstraintRemoved(constraint);
    }

    public bool IsValid() {
      bool result = true;
      foreach (IConstraint constraint in Constraints)
        result = result && constraint.Check(this);
      return result;
    }
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

    public override IView CreateView() {
      return new ConstrainedItemBaseView(this);
    }

    public event EventHandler<ConstraintEventArgs> ConstraintAdded;
    protected virtual void OnConstraintAdded(IConstraint constraint) {
      if (ConstraintAdded != null)
        ConstraintAdded(this, new ConstraintEventArgs(constraint));
    }
    public event EventHandler<ConstraintEventArgs> ConstraintRemoved;
    protected virtual void OnConstraintRemoved(IConstraint constraint) {
      if (ConstraintRemoved != null)
        ConstraintRemoved(this, new ConstraintEventArgs(constraint));
    }

    #region Persistence Methods
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
