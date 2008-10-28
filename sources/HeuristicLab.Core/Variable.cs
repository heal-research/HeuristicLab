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
  public class Variable : ItemBase, IVariable {
    private string myName;
    public string Name {
      get { return myName; }
      set {
        if (!myName.Equals(value)) {
          NameChangingEventArgs e = new NameChangingEventArgs(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            myName = value;
            OnNameChanged();
          }
        }
      }
    }
    private IItem myValue;
    public IItem Value {
      get { return myValue; }
      set {
        if (myValue != value) {
          myValue = value;
          OnValueChanged();
        }
      }
    }

    public Variable() {
      myName = "Anonymous";
      myValue = null;
    }
    public Variable(string name, IItem value) {
      myName = name;
      myValue = value;
    }

    public T GetValue<T>() where T : class, IItem {
      return (T)Value;
    }

    public override IView CreateView() {
      return new VariableView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Variable clone = new Variable();
      clonedObjects.Add(Guid, clone);
      clone.myName = Name;
      if (Value != null)
        clone.myValue = (IItem)Auxiliary.Clone(Value, clonedObjects);
      return clone;
    }

    public override string ToString() {
      return Name + ": " + ((Value == null) ? ("null") : (Value.ToString()));
    }

    public event EventHandler<NameChangingEventArgs> NameChanging;
    protected virtual void OnNameChanging(NameChangingEventArgs e) {
      if (NameChanging != null)
        NameChanging(this, e);
    }
    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      if (NameChanged != null)
        NameChanged(this, new EventArgs());
      OnChanged();
    }
    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      OnChanged();
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttribute = document.CreateAttribute("Name");
      nameAttribute.Value = Name;
      node.Attributes.Append(nameAttribute);
      if (Value != null)
        node.AppendChild(PersistenceManager.Persist("Value", Value, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myName = node.Attributes["Name"].Value;
      XmlNode valueNode = node.SelectSingleNode("Value");
      if (valueNode != null)
        myValue = (IItem)PersistenceManager.Restore(valueNode, restoredObjects);
    }
    #endregion
  }
}
