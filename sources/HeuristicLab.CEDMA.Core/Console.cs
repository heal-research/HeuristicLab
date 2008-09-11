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
    private AgentList agentList;
    private ResultList resultsList;
    private DatabaseOperatorLibrary operatorLibary;
    private ChannelFactory<IDatabase> factory;
    private IDatabase database;
    private string serverUri;
    public string ServerUri {
      get { return serverUri; }
    }

    public IAgentList AgentList {
      get { return agentList; }
    }

    public ResultList ResultsList {
      get { return resultsList; }
    }

    public IOperatorLibrary OperatorLibrary {
      get { return operatorLibary; }
    }

    public Console()
      : base() {
      agentList = new AgentList();
      resultsList = new ResultList();
      operatorLibary = new DatabaseOperatorLibrary();
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
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;
      factory = new ChannelFactory<IDatabase>(binding);
      database = factory.CreateChannel(new EndpointAddress(serverUri));
      agentList.Database = database;
      operatorLibary.Database = database;

      ChannelFactory<IStore> storeFactory = new ChannelFactory<IStore>(binding);
      IStore store = storeFactory.CreateChannel(new EndpointAddress(serverUri + "/RdfStore"));
      resultsList.Store = store;
    }
    #endregion

    internal void Connect(string serverUri) {
      this.serverUri = serverUri;
      ResetConnection();
    }
  }
}
