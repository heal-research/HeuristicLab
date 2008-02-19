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
  public class NotConstraint : ConstraintBase {
    private ConstraintBase subConstraint;
    public ConstraintBase SubConstraint {
      get { return subConstraint; }
      set {
        subConstraint = value;
        OnChanged();
      }
    }
    public override string Description {
      get {
        return "Requires that a constraint must be false to be true";
      }
    }
    public NotConstraint()
      : base() {
      subConstraint = new FalseConstraint();
    }

    public override bool Check(IItem data) {
      return !subConstraint.Check(data);
    }

    public override IView CreateView() {
      return new NotConstraintView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NotConstraint clone = new NotConstraint();
      clonedObjects.Add(Guid, clone);
      clone.SubConstraint = (ConstraintBase)SubConstraint.Clone();
      return clone;
    }

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }


    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode sub = PersistenceManager.Persist("SubConstraint", SubConstraint, document, persistedObjects);
      node.AppendChild(sub);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      subConstraint = (ConstraintBase)PersistenceManager.Restore(node.SelectSingleNode("SubConstraint"), restoredObjects);
    }
    #endregion
  }
}
