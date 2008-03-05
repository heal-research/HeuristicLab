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
  public class AndConstraint : ConstraintBase, IViewable {
    private ItemList<IConstraint> clauses;
    public ItemList<IConstraint> Clauses {
      get { return clauses; }
      set {
        clauses = value;
        OnChanged();
      }
    }

    public override string Description {
      get {
        return "To return true, all subconstraints have to be true";
      }
    }

    public AndConstraint() {
      clauses = new ItemList<IConstraint>();
    }

    public override bool Check(IItem data) {
      bool result = true;
      for (int i = 0 ; i < clauses.Count ; i++) {
        result = clauses[i].Check(data);
        if (!result) return false;
      }
      return result;
    }

    public override IView CreateView() {
      return new AndConstraintView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AndConstraint clone = new AndConstraint();
      clonedObjects.Add(Guid, clone);
      clone.Clauses = (ItemList<IConstraint>)Auxiliary.Clone(Clauses, clonedObjects);
      return clone;
    }

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode clausesNode = PersistenceManager.Persist("Clauses", Clauses, document, persistedObjects);
      node.AppendChild(clausesNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      clauses = (ItemList<IConstraint>)PersistenceManager.Restore(node.SelectSingleNode("Clauses"), restoredObjects);
    }
    #endregion persistence
  }
}
