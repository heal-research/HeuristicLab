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
  /// Represents a single operation with one operator and one scope.
  /// </summary>
  public class AtomicOperation : ItemBase, IOperation {
    private IOperator myOperator;
    /// <summary>
    /// Gets the current operator as <see cref="IOperator"/>.
    /// </summary>
    public IOperator Operator {
      get { return myOperator; }
    }
    private IScope myScope;  
    /// <summary>
    /// Gets the current scope as <see cref="IScope"/>.
    /// </summary>
    public IScope Scope {
      get { return myScope; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AtomicOperation"/>.
    /// </summary>
    public AtomicOperation() { }
    /// <summary>
    /// Initializes a new instance of <see cref="AtomicOperation"/> with the given <paramref name="op"/> 
    /// and the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="op">The operator to assign.</param>
    /// <param name="scope">The scope to assign.</param>
    public AtomicOperation(IOperator op, IScope scope) {
      myOperator = op;
      myScope = scope;
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>The operator and the scope objects are cloned with the 
    /// <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of the <see cref="Auxiliary"/> class.</remarks>
    /// <param name="clonedObjects">All already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="AtomicOperation"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AtomicOperation clone = new AtomicOperation();
      clonedObjects.Add(Guid, clone);
      clone.myOperator = (IOperator)Auxiliary.Clone(Operator, clonedObjects);
      clone.myScope = (IScope)Auxiliary.Clone(Scope, clonedObjects);
      return clone;
    }

    #region Persistence Methods

    /// <summary>
    ///  Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.StorableBase.GetXmlNode"/> of base 
    /// class <see cref="ItemBase"/>. <br/>
    /// The operator is saved as child node having the tag name <c>Operator</c>.<br/>
    /// The scope is also saved as a child node having the tag name <c>Scope</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Operator", Operator, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Scope", Scope, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted operation from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.StorableBase.Populate"/> of base class 
    /// <see cref="ItemBase"/>.<br/>
    /// The operator must be saved as a child node with the tag name <c>Operator</c>, also the scope must 
    /// be saved as a child node having the tag name <c>Scope</c> (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the operation is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myOperator = (IOperator)PersistenceManager.Restore(node.SelectSingleNode("Operator"), restoredObjects);
      myScope = (IScope)PersistenceManager.Restore(node.SelectSingleNode("Scope"), restoredObjects);
    }
    #endregion
  }
}
