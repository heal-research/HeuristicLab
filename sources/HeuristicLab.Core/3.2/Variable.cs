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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a variable of an operator having a name and a value.
  /// </summary>
  public class Variable : ItemBase, IVariable {
    private string myName;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnNameChanging"/> and also <see cref="OnNameChanged"/> 
    /// eventually in the setter.</remarks>
    public string Name {
      get { return myName; }
      set {
        if (!myName.Equals(value)) {
          CancelEventArgs<string> e = new CancelEventArgs<string>(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            myName = value;
            OnNameChanged();
          }
        }
      }
    }
    private IItem myValue;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnValueChanged"/> in the setter.</remarks>
    public IItem Value {
      get { return myValue; }
      set {
        if (myValue != value) {
          myValue = value;
          OnValueChanged();
        }
      }
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with name <c>Anonymous</c> 
    /// and value <c>null</c>.
    /// </summary>
    public Variable() {
      myName = "Anonymous";
      myValue = null;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with the specified <paramref name="name"/>
    /// and the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the current instance.</param>
    /// <param name="value">The value of the current instance.</param>
    public Variable(string name, IItem value) {
      myName = name;
      myValue = value;
    }

    /// <inheritdoc cref="IVariable.GetValue&lt;T&gt;"/>
    public T GetValue<T>() where T : class, IItem {
      return (T)Value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="VariableView"/> to represent the current instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="VariableView"/>.</returns>
    public override IView CreateView() {
      return new VariableView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Variable clone = new Variable();
      clonedObjects.Add(Guid, clone);
      clone.myName = Name;
      if (Value != null)
        clone.myValue = (IItem)Auxiliary.Clone(Value, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Gets the string representation of the current instance in the format: <c>Name: [null|Value]</c>.
    /// </summary>
    /// <returns>The current instance as a string.</returns>
    public override string ToString() {
      return Name + ": " + ((Value == null) ? ("null") : (Value.ToString()));
    }

    /// <inheritdoc/>
    public event EventHandler<CancelEventArgs<string>> NameChanging;
    /// <summary>
    /// Fires a new <c>NameChanging</c> event.
    /// </summary>
    /// <param name="e">The event arguments of the changing.</param>
    protected virtual void OnNameChanging(CancelEventArgs<string> e) {
      if (NameChanging != null)
        NameChanging(this, e);
    }
    /// <inheritdoc/>
    public event EventHandler NameChanged;
    /// <summary>
    /// Fires a new <c>NameChanged</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnNameChanged() {
      if (NameChanged != null)
        NameChanged(this, new EventArgs());
      OnChanged();
    }
    /// <inheritdoc/>
    public event EventHandler ValueChanged;
    /// <summary>
    /// Fires a new <c>ValueChanged</c> even.
    /// </summary>
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      OnChanged();
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>.<br/>
    /// The name of the current instance is saved as an <see cref="XmlAttribute"/> with the 
    /// tag name <c>Name</c>, the value is saved as child node with the tag name <c>Value</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttribute = document.CreateAttribute("Name");
      nameAttribute.Value = Name;
      node.Attributes.Append(nameAttribute);
      if (Value != null)
        node.AppendChild(PersistenceManager.Persist("Value", Value, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted variable from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>See <see cref="GetXmlNode"/> to get information on how the variable must be saved.<br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the variable is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
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
