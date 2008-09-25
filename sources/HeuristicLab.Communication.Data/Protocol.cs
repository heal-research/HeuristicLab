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
using System.Xml.XPath;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class Protocol : ItemBase, IEditable {
    private StringData name;
    public StringData Name {
      get { return name; }
      set {
        name = value;
        OnChanged();
      }
    }
    private ItemList<ProtocolState> states;
    public ItemList<ProtocolState> States {
      get { return states; }
      set {
        states.ItemAdded -= new EventHandler<ItemIndexEventArgs>(States_ItemAdded);
        states.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(States_ItemRemoved);
        states = value;
        states.ItemAdded += new EventHandler<ItemIndexEventArgs>(States_ItemAdded);
        states.ItemRemoved += new EventHandler<ItemIndexEventArgs>(States_ItemRemoved);
        // if the newly assigned states don't contain the currently selected initial state
        if (!states.Contains(initialState))
          if (states.Count > 0)
            initialState = states[0];
          else initialState = null;
        OnChanged();
      }
    }
    private ProtocolState initialState;
    public ProtocolState InitialState {
      get { return initialState; }
      set {
        initialState = value;
        OnChanged();
      }
    }

    public Protocol() {
      name = new StringData("Unnamed protocol");
      states = new ItemList<ProtocolState>();
      states.ItemAdded += new EventHandler<ItemIndexEventArgs>(States_ItemAdded);
      states.ItemRemoved += new EventHandler<ItemIndexEventArgs>(States_ItemRemoved);
      ProtocolState firstState = new ProtocolState();
      firstState.Name = new StringData("InitialState");
      firstState.AcceptingState = new BoolData(true);
      firstState.Protocol = this;
      states.Add(firstState);
      initialState = firstState;
    }

    public void Dispose() {
      states.ItemAdded -= new EventHandler<ItemIndexEventArgs>(States_ItemAdded);
      states.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(States_ItemRemoved);
    }

    public override IView CreateView() {
      return new ProtocolEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new ProtocolEditor(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Protocol clone = new Protocol();
      clonedObjects.Add(Guid, clone);
      clone.Name = (StringData)Auxiliary.Clone(Name, clonedObjects);
      clone.States = (ItemList<ProtocolState>)Auxiliary.Clone(States, clonedObjects);
      // iterate through the states and select the appropriate state in the clone
      for (int i = 0 ; i < states.Count ; i++)
        if (states[i].Equals(initialState))
          clone.InitialState = clone.States[i];
      return clone;
    }

    #region States Events
    void States_ItemRemoved(object sender, ItemIndexEventArgs e) {
      e.Item.Changed -= new EventHandler(State_Changed);
      State_Changed(this, new EventArgs());
    }

    void States_ItemAdded(object sender, ItemIndexEventArgs e) {
      e.Item.Changed += new EventHandler(State_Changed);
      ((ProtocolState)e.Item).Protocol = this;
      State_Changed(this, new EventArgs());
    }
    #endregion

    public EventHandler StatesChanged;
    void State_Changed(object sender, EventArgs e) {
      if (StatesChanged != null) StatesChanged(sender, e);
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode nameNode = PersistenceManager.Persist("Name", Name, document, persistedObjects);
      XmlNode statesNode = PersistenceManager.Persist("States", States, document, persistedObjects);
      XmlNode initialStatesNode = PersistenceManager.Persist("InitialState", InitialState, document, persistedObjects);
      node.AppendChild(nameNode);
      node.AppendChild(statesNode);
      node.AppendChild(initialStatesNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      name = (StringData)PersistenceManager.Restore(node.SelectSingleNode("Name"), restoredObjects);
      states = (ItemList<ProtocolState>)PersistenceManager.Restore(node.SelectSingleNode("States"), restoredObjects);
      states.ItemAdded += new EventHandler<ItemIndexEventArgs>(States_ItemAdded);
      states.ItemRemoved += new EventHandler<ItemIndexEventArgs>(States_ItemRemoved);
      initialState = (ProtocolState)PersistenceManager.Restore(node.SelectSingleNode("InitialState"), restoredObjects);
    }
    #endregion persistence
  }
}
