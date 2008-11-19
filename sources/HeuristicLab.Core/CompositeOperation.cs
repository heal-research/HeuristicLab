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
  /// <summary>
  /// Represents a class for operations consisting themselves of several operations. 
  /// Can also be executed in parallel.
  /// </summary>
  public class CompositeOperation : ItemBase, IOperation {
    private bool myExecuteInParallel;

    /// <summary>
    /// Gets or sets the bool value, whether the operation should be executed in parallel or not.  
    /// </summary>
    public bool ExecuteInParallel {
      get { return myExecuteInParallel; }
      set { myExecuteInParallel = value; }
    }
    private List<IOperation> myOperations;
    /// <summary>
    /// Gets all current operations.
    /// <note type="caution"> Operations are read-only!</note>
    /// </summary>
    public IList<IOperation> Operations {
      get { return myOperations.AsReadOnly(); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CompositeOperation"/>, the <see cref="ExecuteInParallel"/>
    /// property set to <c>false</c>.
    /// </summary>
    public CompositeOperation() {
      myOperations = new List<IOperation>();
      myExecuteInParallel = false;
    }

    /// <summary>
    /// Adds an operation to the current list of operations.
    /// </summary>
    /// <param name="operation">The operation to add.</param>
    public void AddOperation(IOperation operation) {
      myOperations.Add(operation);
    }
    /// <summary>
    /// Removes an operation from the current list.
    /// </summary>
    /// <param name="operation">The operation to remove.</param>
    public void RemoveOperation(IOperation operation) {
      myOperations.Remove(operation);
    }

    /// <summary>
    /// Clones the current instance of <see cref="CompositeOperation"/> (deep clone).
    /// </summary>
    /// <remarks>All operations of the current instance are cloned, too (deep clone), with the 
    /// <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of the class <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned operation as <see cref="CompositeOperation"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      CompositeOperation clone = new CompositeOperation();
      clonedObjects.Add(Guid, clone);
      clone.myExecuteInParallel = ExecuteInParallel;
      for (int i = 0; i < Operations.Count; i++)
        clone.AddOperation((IOperation)Auxiliary.Clone(Operations[i], clonedObjects));
      return clone;
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.StorableBase.GetXmlNode"/> of base 
    /// class <see cref="ItemBase"/>.<br/>
    /// The <see cref="ExecuteInParallel"/> property is saved as <see cref="XmlAttribute"/> with the
    /// tag name <c>ExecuteInParallel</c>. A child node with tag name <c>Operations</c> is created where
    /// all operations are saved as child nodes.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
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
    /// <summary>
    /// Loads the persisted operation from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The <see cref="ExecuteInParallel"/> property must be saved as <see cref="XmlAttribute"/>
    /// with the tag name <c>ExecuteInParallel</c>. <br/>
    /// The single operations must be saved as child nodes of a node with tag name <c>Operations</c>, 
    /// being a child node of the current instance. <br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the operation is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
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
