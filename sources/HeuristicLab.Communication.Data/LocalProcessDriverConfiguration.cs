using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class LocalProcessDriverConfiguration : ItemBase, IDriverConfiguration {
    private StringData executablePath;
    public StringData ExecutablePath {
      get { return executablePath; }
      set {
        executablePath = value;
        OnChanged();
      }
    }
    private StringData arguments;
    public StringData Arguments {
      get { return arguments; }
      set {
        arguments = value;
        OnChanged();
      }
    }
    public LocalProcessDriverConfiguration() {
      executablePath = new StringData("");
      arguments = new StringData("");
    }

    public override IView CreateView() {
      return new LocalProcessDriverConfigurationView(this);
    }

    #region persistence & clone
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode executablePathNode = PersistenceManager.Persist("ExecutablePath", ExecutablePath, document, persistedObjects);
      node.AppendChild(executablePathNode);
      XmlNode argumentsNode = PersistenceManager.Persist("Arguments", Arguments, document, persistedObjects);
      node.AppendChild(argumentsNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      executablePath = (StringData)PersistenceManager.Restore(node.SelectSingleNode("ExecutablePath"), restoredObjects);
      arguments = (StringData)PersistenceManager.Restore(node.SelectSingleNode("Arguments"), restoredObjects);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      LocalProcessDriverConfiguration clone = new LocalProcessDriverConfiguration();
      clonedObjects.Add(Guid, clone);
      clone.executablePath = (StringData)Auxiliary.Clone(executablePath, clonedObjects);
      clone.arguments = (StringData)Auxiliary.Clone(arguments, clonedObjects);
      return clone;
    }
    #endregion
  }
}
