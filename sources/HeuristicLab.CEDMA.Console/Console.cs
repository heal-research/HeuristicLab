using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Console {
  public class Console : ItemBase, IEditable {
    private AgentList agentList;
    private ChannelFactory<IDatabase> factory;
    private IDatabase database;
    private string serverUri;
    public string ServerUri {
      get { return serverUri; }
    }

    public IAgentList AgentList {
      get { return agentList; }
    }

    public Console()
      : base() {
      agentList = new AgentList();
    }

    public IEditor CreateEditor() {
      return new ConsoleEditor(this);
    }

    public IView CreateView() {
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
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      factory = new ChannelFactory<IDatabase>(binding);
      database = factory.CreateChannel(new EndpointAddress(serverUri));
      agentList.Database = database;
    }
    #endregion

    internal void Connect(string serverUri) {
      this.serverUri = serverUri;
      ResetConnection();
    }
  }
}
