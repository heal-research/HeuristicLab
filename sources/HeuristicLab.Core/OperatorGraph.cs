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
  public class OperatorGraph : ItemBase, IOperatorGraph {
    private IDictionary<Guid, IOperator> myOperators;
    public ICollection<IOperator> Operators {
      get { return myOperators.Values; }
    }
    private IOperator myInitialOperator;
    public IOperator InitialOperator {
      get { return myInitialOperator; }
      set {
        if (myInitialOperator != value) {
          myInitialOperator = value;
          OnInitialOperatorChanged();
        }
      }
    }

    public OperatorGraph() {
      myOperators = new Dictionary<Guid, IOperator>();
    }

    public override IView CreateView() {
      return new OperatorGraphView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorGraph clone = new OperatorGraph();
      clonedObjects.Add(Guid, clone);
      foreach (IOperator op in Operators)
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      if (InitialOperator != null)
        clone.myInitialOperator = (IOperator)Auxiliary.Clone(InitialOperator, clonedObjects);
      return clone;
    }

    public void AddOperator(IOperator op) {
      if (!myOperators.ContainsKey(op.Guid)) {
        myOperators.Add(op.Guid, op);
        OnOperatorAdded(op);

        foreach (IOperator subOperator in op.SubOperators)
          AddOperator(subOperator);
      }
    }
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
    public IOperator GetOperator(Guid guid) {
      IOperator op;
      if (myOperators.TryGetValue(guid, out op))
        return op;
      else
        return null;
    }
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

    public event EventHandler<OperatorEventArgs> OperatorAdded;
    protected virtual void OnOperatorAdded(IOperator op) {
      if (OperatorAdded != null)
        OperatorAdded(this, new OperatorEventArgs(op));
    }
    public event EventHandler<OperatorEventArgs> OperatorRemoved;
    protected virtual void OnOperatorRemoved(IOperator op) {
      if (OperatorRemoved != null)
        OperatorRemoved(this, new OperatorEventArgs(op));
    }
    public event EventHandler InitialOperatorChanged;
    protected virtual void OnInitialOperatorChanged() {
      if (InitialOperatorChanged != null)
        InitialOperatorChanged(this, new EventArgs());
    }

    #region Persistence Methods
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
