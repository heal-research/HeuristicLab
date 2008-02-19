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
  public class CompositeOperation : ItemBase, IOperation {
    private bool myExecuteInParallel;
    public bool ExecuteInParallel {
      get { return myExecuteInParallel; }
      set { myExecuteInParallel = value; }
    }
    private List<IOperation> myOperations;
    public IList<IOperation> Operations {
      get { return myOperations.AsReadOnly(); }
    }

    public CompositeOperation() {
      myOperations = new List<IOperation>();
      myExecuteInParallel = false;
    }

    public void AddOperation(IOperation operation) {
      myOperations.Add(operation);
    }
    public void RemoveOperation(IOperation operation) {
      myOperations.Remove(operation);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      CompositeOperation clone = new CompositeOperation();
      clonedObjects.Add(Guid, clone);
      clone.myExecuteInParallel = ExecuteInParallel;
      for (int i = 0; i < Operations.Count; i++)
        clone.AddOperation((IOperation)Auxiliary.Clone(Operations[i], clonedObjects));
      return clone;
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute parallelAttribute = document.CreateAttribute("ExecuteInParallel");
      parallelAttribute.Value = ExecuteInParallel.ToString();
      node.Attributes.Append(parallelAttribute);

      XmlNode operationsNode = document.CreateNode(XmlNodeType.Element, "Operations", null);
      for (int i = 0; i < Operations.Count; i++)
        operationsNode.AppendChild(PersistenceManager.Persist(Operations[i], document, persistedObjects));
      node.AppendChild(operationsNode);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      myExecuteInParallel = bool.Parse(node.Attributes["ExecuteInParallel"].Value);

      XmlNode operationsNode = node.SelectSingleNode("Operations");
      for (int i = 0; i < operationsNode.ChildNodes.Count; i++)
        AddOperation((IOperation)PersistenceManager.Restore(operationsNode.ChildNodes[i], restoredObjects));
    }
    #endregion
  }
}
