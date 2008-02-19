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
  public class OperatorGroup : StorableBase, IOperatorGroup {
    private string myName;
    public string Name {
      get { return myName; }
      set {
        if (myName != value) {
          myName = value;
          OnNameChanged();
        }
      }
    }
    private List<IOperatorGroup> mySubGroups;
    public ICollection<IOperatorGroup> SubGroups {
      get { return mySubGroups.AsReadOnly(); }
    }
    private List<IOperator> myOperators;
    public ICollection<IOperator> Operators {
      get { return myOperators.AsReadOnly(); }
    }

    public OperatorGroup() {
      myName = "Anonymous";
      mySubGroups = new List<IOperatorGroup>();
      myOperators = new List<IOperator>();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorGroup clone = (OperatorGroup)base.Clone(clonedObjects);
      clone.myName = Name;
      foreach (IOperatorGroup group in SubGroups)
        clone.AddSubGroup((IOperatorGroup)Auxiliary.Clone(group, clonedObjects));
      foreach (IOperator op in Operators)
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      return clone;
    }

    public virtual void AddSubGroup(IOperatorGroup group) {
      mySubGroups.Add(group);
    }
    public virtual void RemoveSubGroup(IOperatorGroup group) {
      mySubGroups.Remove(group);
    }
    public virtual void AddOperator(IOperator op) {
      myOperators.Add(op);
    }
    public virtual void RemoveOperator(IOperator op) {
      myOperators.Remove(op);
    }

    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      if (NameChanged != null) {
        NameChanged(this, new EventArgs());
      }
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttribute = document.CreateAttribute("Name");
      nameAttribute.Value = Name;
      node.Attributes.Append(nameAttribute);
      XmlNode subGroupsNode = document.CreateNode(XmlNodeType.Element, "SubGroups", null);
      foreach (IOperatorGroup group in SubGroups)
        subGroupsNode.AppendChild(PersistenceManager.Persist(group, document, persistedObjects));
      node.AppendChild(subGroupsNode);
      XmlNode operatorsNode = document.CreateNode(XmlNodeType.Element, "Operators", null);
      foreach (IOperator op in Operators)
        operatorsNode.AppendChild(PersistenceManager.Persist(op, document, persistedObjects));
      node.AppendChild(operatorsNode);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myName = node.Attributes["Name"].Value;
      XmlNode subGroupsNode = node.SelectSingleNode("SubGroups");
      foreach (XmlNode subGroupNode in subGroupsNode.ChildNodes)
        AddSubGroup((IOperatorGroup)PersistenceManager.Restore(subGroupNode, restoredObjects));
      XmlNode operatorsNode = node.SelectSingleNode("Operators");
      foreach (XmlNode operatorNode in operatorsNode.ChildNodes)
        AddOperator((IOperator)PersistenceManager.Restore(operatorNode, restoredObjects));
    }
    #endregion
  }
}
