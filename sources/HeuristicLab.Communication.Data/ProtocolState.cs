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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.Communication.Data {
  public class ProtocolState : ItemBase {
    private string name;
    public string Name {
      get { return name; }
      set {
        name = value;
        OnChanged();
      }
    }
    private bool giveBatch;
    public bool GiveBatch {
      get { return giveBatch; }
      set {
        giveBatch = value;
        OnChanged();
      }
    }
    private ItemList<IVariable> give;
    public ItemList<IVariable> Give {
      get { return give; }
      set {
        give = value;
        OnChanged();
      }
    }
    private bool expectBatch;
    public bool ExpectBatch {
      get { return expectBatch; }
      set {
        expectBatch = value;
        OnChanged();
      }
    }
    private ItemList<IVariable> expect;
    public ItemList<IVariable> Expect {
      get { return expect; }
      set {
        expect = value;
        OnChanged();
      }
    }

    public ProtocolState() {
      name = Guid.NewGuid().ToString();
      giveBatch = false;
      expectBatch = false;
      give = new ItemList<IVariable>();
      expect = new ItemList<IVariable>();
    }

    public override IView CreateView() {
      return new ProtocolStateView(this);
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ProtocolState clone = new ProtocolState();
      clonedObjects.Add(Guid, clone);

      clone.name = (string)name.Clone();
      clone.giveBatch = giveBatch;
      clone.expectBatch = expectBatch;

      clone.give = new ItemList<IVariable>();
      for (int i = 0; i < give.Count; i++)
        clone.give.Add((IVariable)give[i].Clone());

      clone.expect = new ItemList<IVariable>();
      for (int i = 0; i < expect.Count; i++)
        clone.expect.Add((IVariable)expect[i].Clone());

      return clone;
    }

    // use a simpler serialization for the protocol to make reading it in other programming languages easier
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttrib = document.CreateAttribute("Name");
      nameAttrib.Value = this.name;
      node.Attributes.Append(nameAttrib);
      XmlAttribute giveBatchAttrib = document.CreateAttribute("GiveBatch");
      giveBatchAttrib.Value = giveBatch.ToString();
      node.Attributes.Append(giveBatchAttrib);
      XmlAttribute expectBatchAttrib = document.CreateAttribute("ExpectBatch");
      expectBatchAttrib.Value = expectBatch.ToString();
      node.Attributes.Append(expectBatchAttrib);

      XmlNode giveNode = document.CreateNode(XmlNodeType.Element, "Give", null);
      foreach (IVariable param in give) {
        XmlNode tmp = document.CreateNode(XmlNodeType.Element, "Parameter", null);
        XmlAttribute paramNameAttrib = document.CreateAttribute("Name");
        paramNameAttrib.Value = param.Name;
        tmp.Attributes.Append(paramNameAttrib);
        XmlAttribute valueTypeAttrib = document.CreateAttribute("Type");
        Type type = param.Value.GetType();
        valueTypeAttrib.Value = type.FullName + ", " + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(","));
        tmp.Attributes.Append(valueTypeAttrib);
        giveNode.AppendChild(tmp);
      }
      node.AppendChild(giveNode);

      XmlNode expectNode = document.CreateNode(XmlNodeType.Element, "Expect", null);
      foreach (IVariable param in expect) {
        XmlNode tmp = document.CreateNode(XmlNodeType.Element, "Parameter", null);
        XmlAttribute paramNameAttrib = document.CreateAttribute("Name");
        paramNameAttrib.Value = param.Name;
        tmp.Attributes.Append(paramNameAttrib);
        XmlAttribute valueTypeAttrib = document.CreateAttribute("Type");
        Type type = param.Value.GetType();
        valueTypeAttrib.Value = type.FullName + ", " + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(","));
        tmp.Attributes.Append(valueTypeAttrib);
        expectNode.AppendChild(tmp);
      }
      node.AppendChild(expectNode);
      return node;
    }

    // use a simpler serialization for the protocol to make reading it in other programming languages easier
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      name = node.Attributes.GetNamedItem("Name").Value;
      bool.TryParse(node.Attributes.GetNamedItem("GiveBatch").Value, out giveBatch);
      bool.TryParse(node.Attributes.GetNamedItem("ExpectBatch").Value, out expectBatch);

      give = new ItemList<IVariable>();
      XmlNodeList giveParams = node.SelectSingleNode("Give").SelectNodes("Parameter");
      foreach (XmlNode param in giveParams) {
        IItem tmp = (IItem)Activator.CreateInstance(System.Type.GetType(param.Attributes.GetNamedItem("Type").Value));
        IVariable var = new Variable(param.Attributes.GetNamedItem("Name").Value, tmp);
        give.Add(var);
      }

      expect = new ItemList<IVariable>();
      XmlNodeList expectParams = node.SelectSingleNode("Expect").SelectNodes("Parameter");
      foreach (XmlNode param in expectParams) {
        IItem tmp = (IItem)Activator.CreateInstance(System.Type.GetType(param.Attributes.GetNamedItem("Type").Value));
        IVariable var = new Variable(param.Attributes.GetNamedItem("Name").Value, tmp);
        expect.Add(var);
      }
    }
    #endregion clone & persistence

    public override string ToString() {
      return name.ToString();
    }
  }
}
