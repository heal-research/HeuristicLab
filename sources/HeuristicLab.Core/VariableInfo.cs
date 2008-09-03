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
  public class VariableInfo : ItemBase, IVariableInfo {
    private string myActualName;
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
    public string FormalName {
      get { return myFormalName; }
    }
    private string myDescription;
    public string Description {
      get { return myDescription; }
    }
    private Type myDataType;
    public Type DataType {
      get { return myDataType; }
    }
    private VariableKind myKind;
    public VariableKind Kind {
      get { return myKind; }
    }
    private bool myLocal;
    public bool Local {
      get { return myLocal; }
      set {
        if (myLocal != value) {
          myLocal = value;
          OnLocalChanged();
        }
      }
    }

    public VariableInfo() {
      myActualName = "Anonymous";
      myFormalName = "Anonymous";
      myDescription = "";
      myDataType = null;
      myKind = VariableKind.In;
      myLocal = false;
    }
    public VariableInfo(string formalName, string description, Type dataType, VariableKind kind)
      : this() {
      myActualName = formalName;
      myFormalName = formalName;
      myDescription = description;
      myDataType = dataType;
      myKind = kind;
    }

    public override IView CreateView() {
      return new VariableInfoView(this);
    }

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

    public event EventHandler ActualNameChanged;
    protected virtual void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
    }
    public event EventHandler LocalChanged;
    protected virtual void OnLocalChanged() {
      if (LocalChanged != null)
        LocalChanged(this, new EventArgs());
    }

    #region Persistence Methods
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
