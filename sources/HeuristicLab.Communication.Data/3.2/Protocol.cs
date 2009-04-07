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
    private string name;
    public string Name {
      get { return name; }
      set {
        name = value;
        OnChanged();
      }
    }
    private IList<ProtocolState> states;
    public IList<ProtocolState> States {
      get { return states; }
      set {
        states = value;
        // if the newly assigned states list doesn't contain the currently selected initial state
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
      name = "Unnamed protocol";
      states = new List<ProtocolState>();

      ProtocolState firstState = new ProtocolState();
      firstState.Name = "InitialState";
      states.Add(firstState);
      initialState = firstState;
    }

    public override IView CreateView() {
      return new ProtocolEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new ProtocolEditor(this);
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Protocol clone = new Protocol();
      clonedObjects.Add(Guid, clone);
      clone.Name = (string)name.Clone();
      clone.States = new List<ProtocolState>(states.Count);
      foreach (ProtocolState state in states)
        clone.States.Add((ProtocolState)state.Clone(clonedObjects));
      // iterate through the states and select the appropriate state in the clone
      int index = states.IndexOf(initialState);
      if (index >= 0) clone.InitialState = clone.States[index];
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      
      XmlAttribute nameAttrib = document.CreateAttribute("Name");
      nameAttrib.Value = this.name;
      node.Attributes.Append(nameAttrib);

      XmlNode statesNode = document.CreateNode(XmlNodeType.Element, "States", null);
      foreach (ProtocolState state in states) {
        XmlNode tmp = state.GetXmlNode("State", document, persistedObjects);
        if (state.Equals(initialState)) {
          XmlAttribute initialStateAttrib = document.CreateAttribute("InitialState");
          initialStateAttrib.Value = "1";
          tmp.Attributes.Append(initialStateAttrib);
        }
        statesNode.AppendChild(tmp);
      }
      node.AppendChild(statesNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      name = node.Attributes.GetNamedItem("Name").Value;

      XmlNode statesNode = node.SelectSingleNode("States");
      states = new List<ProtocolState>(statesNode.ChildNodes.Count);
      foreach (XmlNode childNode in statesNode.ChildNodes) {
        ProtocolState tmp = new ProtocolState();
        tmp.Populate(childNode, restoredObjects);
        states.Add(tmp);
        XmlNode initialStateNode = childNode.Attributes.GetNamedItem("InitialState");
        if (initialStateNode != null && initialStateNode.Value.Equals("1")) initialState = tmp;
      }
    }
    #endregion clone & persistence
  }
}
