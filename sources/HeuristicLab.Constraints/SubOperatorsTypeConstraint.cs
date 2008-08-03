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

namespace HeuristicLab.Constraints {
  public class SubOperatorTypeConstraint : ConstraintBase {
    private IntData subOperatorIndex;
    public IntData SubOperatorIndex {
      get { return subOperatorIndex; }
    }

    private List<IOperator> subOperators;
    public IList<IOperator> AllowedSubOperators {
      get {
        return subOperators.AsReadOnly();
      }
    }

    public override string Description {
      get { return "The sub-operator at a specific index has to be an element of a pre-defined group."; }
    }

    public SubOperatorTypeConstraint()
      : base() {
      subOperatorIndex = new IntData(0);
      subOperators = new List<IOperator>();
    }

    public void AddOperator(IOperator op) {
      // check if already in the list of allowed functions
      foreach(IOperator existingOp in subOperators) {
        if(existingOp == op) {
          return;
        }
      }

      subOperators.Add(op);
    }

    public void RemoveOperator(IOperator op) {
      IOperator matchingOperator = null;
      foreach(IOperator existingOp in subOperators) {
        if(existingOp == op) {
          matchingOperator = existingOp;
          break;
        }
      }

      if(matchingOperator != null) {
        subOperators.Remove(matchingOperator);
      }
    }

    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if(data == null) return false;

      if(op.SubOperators.Count <= subOperatorIndex.Data) {
        return false;
      }

      return subOperators.Exists(delegate(IOperator curOp) { return curOp == op.SubOperators[subOperatorIndex.Data]; });
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SubOperatorTypeConstraint clone = new SubOperatorTypeConstraint();
      clonedObjects.Add(Guid, clone);
      clone.subOperatorIndex.Data = subOperatorIndex.Data;
      foreach(IOperator op in subOperators) {
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      }
      return clone;
    }

    public override IView CreateView() {
      return new SubOperatorsTypeConstraintView(this);
    }

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }


    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode indexNode = PersistenceManager.Persist("SubOperatorIndex", subOperatorIndex, document, persistedObjects);
      node.AppendChild(indexNode);
      XmlNode listNode = document.CreateNode(XmlNodeType.Element, "AllowedSubOperators", document.NamespaceURI);
      foreach(IOperator op in subOperators) {
        XmlNode opNode = PersistenceManager.Persist(op, document, persistedObjects);
        listNode.AppendChild(opNode);
      }
      node.AppendChild(listNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      subOperatorIndex = (IntData)PersistenceManager.Restore(node.SelectSingleNode("SubOperatorIndex"), restoredObjects);
      subOperators = new List<IOperator>();
      foreach(XmlNode childNode in node.SelectSingleNode("AllowedSubOperators").ChildNodes) {
        subOperators.Add((IOperator)PersistenceManager.Restore(childNode, restoredObjects));
      }
    }
    #endregion persistence

  }
}
