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
  /// Represents a graph of operators.
  /// </summary>
  public class OperatorGraph : ItemBase, IOperatorGraph {
    private IDictionary<Guid, IOperator> myOperators;
    /// <summary>
    /// Gets all operators of the current instance.
    /// </summary>
    public ICollection<IOperator> Operators {
      get { return myOperators.Values; }
    }
    private IOperator myInitialOperator;
    /// <summary>
    /// Gets or sets the initial operator (the starting one).
    /// </summary>
    /// <remarks>Calls <see cref="OnInitialOperatorChanged"/> in the setter.</remarks>
    public IOperator InitialOperator {
      get { return myInitialOperator; }
      set {
        if (myInitialOperator != value) {
          myInitialOperator = value;
          OnInitialOperatorChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraph"/>.
    /// </summary>
    public OperatorGraph() {
      myOperators = new Dictionary<Guid, IOperator>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="OperatorGraphView"/> to represent the current instance
    /// visually.
    /// </summary>
    /// <returns>The created view as <see cref="OperatorGraphView"/>.</returns>
    public override IView CreateView() {
      return new OperatorGraphView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorGraph"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorGraph clone = new OperatorGraph();
      clonedObjects.Add(Guid, clone);
      foreach (IOperator op in Operators)
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      if (InitialOperator != null)
        clone.myInitialOperator = (IOperator)Auxiliary.Clone(InitialOperator, clonedObjects);
      return clone;
    }

    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnOperatorAdded"/>.</remarks>
    public void AddOperator(IOperator op) {
      if (!myOperators.ContainsKey(op.Guid)) {
        myOperators.Add(op.Guid, op);
        OnOperatorAdded(op);

        foreach (IOperator subOperator in op.SubOperators)
          AddOperator(subOperator);
      }
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnOperatorRemoved"/>.</remarks>
    public void RemoveOperator(Guid guid) {
      IOperator op = GetOperator(guid);
      if (op != null) {
        foreach (IOperator o in Operators) {
          int i = 0;
          while (i < o.SubOperators.Count) {
            if (o.SubOperators[i] == op)
              o.RemoveSubOperator(i);
            else
              i++;
          }
        }
        if (InitialOperator == op)
          InitialOperator = null;
        myOperators.Remove(op.Guid);
        OnOperatorRemoved(op);
      }
    }
    /// <inheritdoc/>
    public IOperator GetOperator(Guid guid) {
      IOperator op;
      if (myOperators.TryGetValue(guid, out op))
        return op;
      else
        return null;
    }
    /// <inheritdoc/>
    public void Clear() {
      Guid[] guids = new Guid[Operators.Count];
      int i = 0;
      foreach (IOperator op in Operators) {
        guids[i] = op.Guid;
        i++;
      }
      for (int j = 0; j < guids.Length; j++)
        RemoveOperator(guids[j]);
    }

    /// <inheritdoc/>
    public event EventHandler<OperatorEventArgs> OperatorAdded;
    /// <summary>
    /// Fires a new <c>OperatorAdded</c> event.
    /// </summary>
    /// <param name="op">The operator that has been added.</param>
    protected virtual void OnOperatorAdded(IOperator op) {
      if (OperatorAdded != null)
        OperatorAdded(this, new OperatorEventArgs(op));
    }
    /// <inheritdoc/>
    public event EventHandler<OperatorEventArgs> OperatorRemoved;
    /// <summary>
    /// Fires a new <c>OperatorRemoved</c> event.
    /// </summary>
    /// <param name="op">The operator that has been removed.</param>
    protected virtual void OnOperatorRemoved(IOperator op) {
      if (OperatorRemoved != null)
        OperatorRemoved(this, new OperatorEventArgs(op));
    }
    /// <inheritdoc/>
    public event EventHandler InitialOperatorChanged;
    /// <summary>
    /// Fires a new <c>InitialOperatorChanged</c> event.
    /// </summary>
    protected virtual void OnInitialOperatorChanged() {
      if (InitialOperatorChanged != null)
        InitialOperatorChanged(this, new EventArgs());
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>.<br/>
    /// To save the operators of the current instance a child node is created with the tag name 
    /// <c>Operators</c>. Beyond this child node all operators are saved as child nodes themselves.<br/>
    /// The initial operator is saved as child node with the tag name <c>InitialOperator</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode ops = document.CreateNode(XmlNodeType.Element, "Operators", null);
      foreach (IOperator op in myOperators.Values)
        ops.AppendChild(PersistenceManager.Persist(op, document, persistedObjects));
      node.AppendChild(ops);
      if (InitialOperator != null)
        node.AppendChild(PersistenceManager.Persist("InitialOperator", InitialOperator, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted operator graph from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>See <see cref="GetXmlNode"/> to get more information about how the graph must be saved. <br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the operator graph is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      XmlNode ops = node.SelectSingleNode("Operators");
      for (int i = 0; i < ops.ChildNodes.Count; i++) {
        XmlNode opNode = ops.ChildNodes[i];
        IOperator op = (IOperator)PersistenceManager.Restore(opNode, restoredObjects);
        myOperators.Add(op.Guid, op);
      }

      XmlNode initial = node.SelectSingleNode("InitialOperator");
      if (initial != null)
        myInitialOperator = (IOperator)PersistenceManager.Restore(initial, restoredObjects);
    }
    #endregion
  }
}
