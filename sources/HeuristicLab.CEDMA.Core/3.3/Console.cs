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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Core {
  public class Console : ItemBase, IEditable {
    private string serverUri;
    public string ServerUri {
      get { return serverUri; }
    }

    private DataSet dataSet;
    public DataSet DataSet {
      get { return dataSet; }
    }

    public Console()
      : base() {
      dataSet = new DataSet();
    }

    public IEditor CreateEditor() {
      return new ConsoleEditor(this);
    }

    public override IView CreateView() {
      return new ConsoleEditor(this);
    }

    #region serialization
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute uriAttribute = document.CreateAttribute("ServerURI");
      uriAttribute.Value = serverUri;
      node.Attributes.Append(uriAttribute);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      serverUri = node.Attributes["ServerURI"].Value;
    }
    #endregion

    internal void Connect(string serverUri) {
      IStore store = new StoreProxy(serverUri);
      var variableBindings = store.Query("?Dataset <" + Ontology.InstanceOf + "> <" + Ontology.TypeDataSet + "> .", 0, 10).ToArray();
      if (variableBindings.Length > 0) {
        dataSet = new DataSet(store, (Entity)variableBindings[0].Get("Dataset"));
      } else {
        dataSet.Store = store;
      }
    }
  }
}
