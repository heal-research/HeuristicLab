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
  public class AllSubOperatorsTypeConstraint : ConstraintBase {

    private SubOperatorTypeConstraint groupConstraint;
    public IList<IOperator> AllowedSubOperators {
      get {
        return groupConstraint.AllowedSubOperators;
      }
    }

    public override string Description {
      get { return "All sub-operators have to be elements of a pre-defined group."; }
    }

    public AllSubOperatorsTypeConstraint()
      : base() {
      groupConstraint = new SubOperatorTypeConstraint();
    }

    public void AddOperator(IOperator op) {
      groupConstraint.AddOperator(op);
    }

    public void RemoveOperator(IOperator op) {
      groupConstraint.RemoveOperator(op);
    }

    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if (data == null) return false;

      for (int i = 0; i < op.SubOperators.Count; i++ ) {
        groupConstraint.SubOperatorIndex.Data = i;
        if(groupConstraint.Check(data)==false) {
          return false;
        }
      }
      return true;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AllSubOperatorsTypeConstraint clone = new AllSubOperatorsTypeConstraint();
      clonedObjects.Add(Guid, clone);
      clone.groupConstraint = (SubOperatorTypeConstraint)Auxiliary.Clone(groupConstraint, clonedObjects);
      return clone;
    }

    public override IView CreateView() {
      return new AllSubOperatorsTypeConstraintView(groupConstraint);
    }

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode subOperatorsNode = PersistenceManager.Persist("SubOperatorsGroupConstraint", groupConstraint, document, persistedObjects);
      node.AppendChild(subOperatorsNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      groupConstraint = (SubOperatorTypeConstraint)PersistenceManager.Restore(node.SelectSingleNode("SubOperatorsGroupConstraint"), restoredObjects);
    }
    #endregion persistence
  }
}
