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
    private DataSetList dataSetList;
    private ChannelFactory<IStore> factory;
    private IStore store;
    private string serverUri;
    public string ServerUri {
      get { return serverUri; }
    }

    public DataSetList DataSetList {
      get { return dataSetList; }
    }

    public Console()
      : base() {
      dataSetList = new DataSetList();
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

    #region WCF
    private void ResetConnection() {
      NetTcpBinding binding = new NetTcpBinding();
      binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
      binding.MaxReceivedMessageSize = 1000000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 1000000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 1000000000; // also 100M elements;
      factory = new ChannelFactory<IStore>(binding);
      store = factory.CreateChannel(new EndpointAddress(serverUri));
      dataSetList.Store = store;
    }
    #endregion

    internal void Connect(string serverUri) {
      this.serverUri = serverUri;
      ResetConnection();
    }
  }
}
