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
  /// Class for storing meta-information about variables/parameters of an operator.
  /// </summary>
  public class VariableInfo : ItemBase, IVariableInfo {
    private string myActualName;
    /// <summary>
    /// Gets or sets the actual name of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnActualNameChanged"/> in the setter.</remarks>
    public string ActualName {
      get { return myActualName; }
      set {
        if (myActualName != value) {
          myActualName = value;
          OnActualNameChanged();
        }
      }
    }
    private string myFormalName;
    /// <summary>
    /// Gets the formal name of the current instance.
    /// </summary>
    public string FormalName {
      get { return myFormalName; }
    }
    private string myDescription;
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    public string Description {
      get { return myDescription; }
    }
    private Type myDataType;
    /// <summary>
    /// Gets the data type of the parameter.
    /// </summary>
    public Type DataType {
      get { return myDataType; }
    }
    private VariableKind myKind;
    /// <summary>
    /// Gets the kind of the parameter (input parameter, output parameter,...).
    /// </summary>
    public VariableKind Kind {
      get { return myKind; }
    }
    private bool myLocal;
    /// <summary>
    /// Gets or sets a boolean value, whether the variable is a local one. 
    /// </summary>
    /// <remarks>Calls <see cref="OnLocalChanged"/> in the setter.</remarks>
    public bool Local {
      get { return myLocal; }
      set {
        if (myLocal != value) {
          myLocal = value;
          OnLocalChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableInfo"/> with actual and formal name "Anonymous",
    /// no description, a <c>null</c> data type and the <c>local</c> flag set to <c>false</c>. The type of 
    /// the variable is an input parameter.
    /// </summary>
    public VariableInfo() {
      myActualName = "Anonymous";
      myFormalName = "Anonymous";
      myDescription = "";
      myDataType = null;
      myKind = VariableKind.In;
      myLocal = false;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableInfo"/> with the given parameters.
    /// </summary>
    /// <remarks>Calls <see cref="VariableInfo()"/>.<br/>
    /// The formal name is assigned to the actual name, too.</remarks>
    /// <param name="formalName">The formal name of the current instance.</param>
    /// <param name="description">The description of the current instance.</param>
    /// <param name="dataType">The data type of the parameter.</param>
    /// <param name="kind">The type of the parameter.</param>
    public VariableInfo(string formalName, string description, Type dataType, VariableKind kind)
      : this() {
      myActualName = formalName;
      myFormalName = formalName;
      myDescription = description;
      myDataType = dataType;
      myKind = kind;
    }

    /// <summary>
    /// Creates a new instance of <see cref="VariableInfoView"/> to represent the current instance
    /// visually.
    /// </summary>
    /// <returns>The created view as <see cref="VariableInfoView"/>.</returns>
    public override IView CreateView() {
      return new VariableInfoView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="VariableInfo"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableInfo clone = new VariableInfo();
      clonedObjects.Add(Guid, clone);
      clone.myActualName = ActualName;
      clone.myFormalName = FormalName;
      clone.myDescription = Description;
      clone.myDataType = DataType;
      clone.myKind = Kind;
      clone.myLocal = Local;
      return clone;
    }

    /// <inheritdoc/>
    public event EventHandler ActualNameChanged;
    /// <summary>
    /// Fires a new <c>ActualNameChanged</c> event.
    /// </summary>
    protected virtual void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
    }
    /// <inheritdoc />
    public event EventHandler LocalChanged;
    /// <summary>
    /// Fires a new <c>LocalChanged</c> event.
    /// </summary>
    protected virtual void OnLocalChanged() {
      if (LocalChanged != null)
        LocalChanged(this, new EventArgs());
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>. <br/>
    /// A quick overview how the single elements of the current instance are saved:
    /// <list type="bullet">
    /// <item>
    /// <term>Actual name: </term>
    /// <description>Saved as <see cref="XmlAttribute"/> with tag name <c>ActualName</c>.</description>
    /// </item>
    /// <item>
    /// <term>Formal name: </term>
    /// <description>Saves as <see cref="XmlAttribute"/> with tag name <c>FormalName</c>.</description>
    /// </item> 
    /// <item>
    /// <term>Description: </term>
    /// <description>Saved as <see cref="XmlAttribute"/> with tag name <c>Description</c>.</description>
    /// </item>
    /// <item><term>Data type: </term>
    /// <description>Saved as <see cref="XmlAttribute"/> with tag name <c>DataType</c>.</description>
    /// </item>
    /// <item>
    /// <term>Kind: </term>
    /// <description>Saved as <see cref="XmlAttribute"/> with tag name <c>Kind</c>.</description>
    /// </item>
    /// <item>
    /// <term>Local: </term>
    /// <description>Saved as <see cref="XmlAttribute"/> with tag name <c>Local</c>.</description>
    /// </item>
    /// </list></remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute actualNameAttribute = document.CreateAttribute("ActualName");
      actualNameAttribute.Value = ActualName;
      node.Attributes.Append(actualNameAttribute);

      XmlAttribute formalNameAttribute = document.CreateAttribute("FormalName");
      formalNameAttribute.Value = FormalName;
      node.Attributes.Append(formalNameAttribute);

      XmlAttribute descriptionAttribute = document.CreateAttribute("Description");
      descriptionAttribute.Value = Description;
      node.Attributes.Append(descriptionAttribute);

      XmlAttribute dataTypeAttribute = document.CreateAttribute("DataType");
      dataTypeAttribute.Value = PersistenceManager.BuildTypeString(DataType);
      node.Attributes.Append(dataTypeAttribute);

      XmlAttribute kindAttribute = document.CreateAttribute("Kind");
      kindAttribute.Value = Kind.ToString();
      node.Attributes.Append(kindAttribute);

      XmlAttribute localAttribute = document.CreateAttribute("Local");
      localAttribute.Value = Local.ToString();
      node.Attributes.Append(localAttribute);

      return node;
    }
    /// <summary>
    /// Loads the persisted variable info from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>See <see cref="GetXmlNode"/> for further information on how the variable info must be 
    /// saved. <br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the variable info is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myActualName = node.Attributes["ActualName"].Value;
      myFormalName = node.Attributes["FormalName"].Value;
      myDescription = node.Attributes["Description"].Value;
      myDataType = Type.GetType(node.Attributes["DataType"].Value, true);
      myKind = (VariableKind)Enum.Parse(typeof(VariableKind), node.Attributes["Kind"].Value);
      myLocal = bool.Parse(node.Attributes["Local"].Value);
    }
    #endregion
  }
}
