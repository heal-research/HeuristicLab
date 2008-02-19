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
  public class AtomicOperation : ItemBase, IOperation {
    private IOperator myOperator;
    public IOperator Operator {
      get { return myOperator; }
    }
    private IScope myScope;
    public IScope Scope {
      get { return myScope; }
    }

    public AtomicOperation() { }
    public AtomicOperation(IOperator op, IScope scope) {
      myOperator = op;
      myScope = scope;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AtomicOperation clone = new AtomicOperation();
      clonedObjects.Add(Guid, clone);
      clone.myOperator = (IOperator)Auxiliary.Clone(Operator, clonedObjects);
      clone.myScope = (IScope)Auxiliary.Clone(Scope, clonedObjects);
      return clone;
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Operator", Operator, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Scope", Scope, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myOperator = (IOperator)PersistenceManager.Restore(node.SelectSingleNode("Operator"), restoredObjects);
      myScope = (IScope)PersistenceManager.Restore(node.SelectSingleNode("Scope"), restoredObjects);
    }
    #endregion
  }
}
