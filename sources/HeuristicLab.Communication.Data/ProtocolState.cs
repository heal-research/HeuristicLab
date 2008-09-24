using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.Communication.Data {
  public class ProtocolState : ItemBase {
    private StringData name;
    public StringData Name {
      get { return name; }
      set {
        name.Changed -= new EventHandler(Name_Changed);
        name = value;
        name.Changed += new EventHandler(Name_Changed);
        OnChanged();
      }
    }
    private BoolData acceptingState;
    public BoolData AcceptingState {
      get { return acceptingState; }
      set {
        acceptingState = value;
        OnChanged();
      }
    }
    private ConstrainedItemList sendingData;
    public ConstrainedItemList SendingData {
      get { return sendingData; }
      set {
        sendingData = value;
        OnChanged();
      }
    }
    private ConstrainedItemList receivingData;
    public ConstrainedItemList ReceivingData {
      get { return receivingData; }
      set {
        receivingData = value;
        OnChanged();
      }
    }
    private ItemList<StateTransition> stateTransitions;
    public ItemList<StateTransition> StateTransitions {
      get { return stateTransitions; }
      set {
        stateTransitions = value;
        OnChanged();
      }
    }
    private Protocol protocol;
    public Protocol Protocol {
      get { return protocol; }
      set {
        protocol = value;
        OnChanged();
      }
    }

    public ProtocolState() {
      name = new StringData("Unnamed state");
      name.Changed += new EventHandler(Name_Changed);
      acceptingState = new BoolData(true);
      sendingData = new ConstrainedItemList();
      sendingData.AddConstraint(new ItemTypeConstraint(typeof(Variable)));
      receivingData = new ConstrainedItemList();
      receivingData.AddConstraint(new ItemTypeConstraint(typeof(Variable)));
      stateTransitions = null;
      protocol = null;
    }

    public void Dispose() {
      name.Changed -= new EventHandler(Name_Changed);
    }

    public override IView CreateView() {
      return new ProtocolStateView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ProtocolState clone = new ProtocolState();
      clonedObjects.Add(Guid, clone);
      clone.name = (StringData)Auxiliary.Clone(Name, clonedObjects);
      clone.acceptingState = (BoolData)Auxiliary.Clone(AcceptingState, clonedObjects);
      clone.sendingData = (ConstrainedItemList)Auxiliary.Clone(SendingData, clonedObjects);
      clone.receivingData = (ConstrainedItemList)Auxiliary.Clone(ReceivingData, clonedObjects);
      if (StateTransitions != null)
        clone.stateTransitions = (ItemList<StateTransition>)Auxiliary.Clone(StateTransitions, clonedObjects);
      else clone.StateTransitions = null;
      clone.protocol = Protocol;
      return clone;
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode protocolNode = PersistenceManager.Persist("ParentProtocol", Protocol, document, persistedObjects);
      node.AppendChild(protocolNode);
      XmlNode nameNode = PersistenceManager.Persist("Name", Name, document, persistedObjects);
      node.AppendChild(nameNode);
      XmlNode acceptingNode = PersistenceManager.Persist("AcceptingState", AcceptingState, document, persistedObjects);
      node.AppendChild(acceptingNode);
      XmlNode requestNode = PersistenceManager.Persist("Request", SendingData, document, persistedObjects);
      node.AppendChild(requestNode);
      XmlNode responseNode = PersistenceManager.Persist("Response", ReceivingData, document, persistedObjects);
      node.AppendChild(responseNode);
      if (StateTransitions != null) {
        XmlNode transitionsNode = PersistenceManager.Persist("StateTransitions", StateTransitions, document, persistedObjects);
        node.AppendChild(transitionsNode);
      }
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      protocol = (Protocol)PersistenceManager.Restore(node.SelectSingleNode("ParentProtocol"), restoredObjects);
      name = (StringData)PersistenceManager.Restore(node.SelectSingleNode("Name"), restoredObjects);
      acceptingState = (BoolData)PersistenceManager.Restore(node.SelectSingleNode("AcceptingState"), restoredObjects);
      sendingData = (ConstrainedItemList)PersistenceManager.Restore(node.SelectSingleNode("Request"), restoredObjects);
      receivingData = (ConstrainedItemList)PersistenceManager.Restore(node.SelectSingleNode("Response"), restoredObjects);
      XmlNode transitions = node.SelectSingleNode("StateTransitions");
      if (transitions != null)
        stateTransitions = (ItemList<StateTransition>)PersistenceManager.Restore(transitions, restoredObjects);
      else
        stateTransitions = null;
    }
    #endregion persistence

    private void Name_Changed(object sender, EventArgs e) {
      OnChanged();
    }

    public override string ToString() {
      return Name.ToString();
    }
  }
}
