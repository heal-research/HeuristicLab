using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class Message : ItemBase {
    public string Protocol;
    public string Source;
    public string Destination;
    public DateTime Timestamp;
    public IList<IVariable> Give;
    public IList<IVariable> Expect;

    public Message()
      : base() {
      Protocol = "unknown";
      Source = "unknown";
      Destination = "unknown";
      Timestamp = DateTime.Now;
      Give = new List<IVariable>();
      Expect = new List<IVariable>();
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Message clone = new Message();
      clonedObjects.Add(Guid, clone);
      clone.Protocol = (string)Protocol.Clone();
      clone.Source = (string)Source.Clone();
      clone.Destination = (string)Destination.Clone();
      clone.Timestamp = new DateTime(Timestamp.Ticks);
      foreach (IVariable var in Give)
        clone.Give.Add((IVariable)var.Clone(clonedObjects));
      foreach (IVariable var in Expect)
        clone.Expect.Add((IVariable)var.Clone(clonedObjects));
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute protoAttrib = document.CreateAttribute("Protocol");
      protoAttrib.Value = (string)Protocol.Clone();
      XmlAttribute sourceAttrib = document.CreateAttribute("Source");
      sourceAttrib.Value = (string)Source.Clone();
      XmlAttribute destinationAttrib = document.CreateAttribute("Destination");
      destinationAttrib.Value = (string)Destination.Clone();
      XmlAttribute timeStampAttrib = document.CreateAttribute("Timestamp");
      StringBuilder tsBuilder = new StringBuilder();
      tsBuilder.Append(Timestamp.Year);tsBuilder.Append("/");
      tsBuilder.Append(Timestamp.Month);tsBuilder.Append("/");
      tsBuilder.Append(Timestamp.Day);tsBuilder.Append(" ");
      tsBuilder.Append(Timestamp.Hour);tsBuilder.Append(":");
      tsBuilder.Append(Timestamp.Minute);tsBuilder.Append(":");
      tsBuilder.Append(Timestamp.Second);tsBuilder.Append(":");
      tsBuilder.Append(Timestamp.Millisecond);
      timeStampAttrib.Value = tsBuilder.ToString();

      node.Attributes.Append(protoAttrib);
      node.Attributes.Append(sourceAttrib);
      node.Attributes.Append(destinationAttrib);
      node.Attributes.Append(timeStampAttrib);

      XmlNode dataNode = document.CreateNode(XmlNodeType.Element, "Data", null);
      XmlNode giveNode = document.CreateNode(XmlNodeType.Element, "Give", null);
      foreach (IVariable var in Give) {
        XmlNode varNode = document.CreateNode(XmlNodeType.Element, "Parameter", null);
        XmlAttribute varNameAttrib = document.CreateAttribute("Name");
        varNameAttrib.Value = (string)var.Name.Clone();
        varNode.Attributes.Append(varNameAttrib);
        varNode.AppendChild(var.Value.GetXmlNode("Value", document, persistedObjects));
        giveNode.AppendChild(varNode);
      }
      dataNode.AppendChild(giveNode);
      XmlNode expectNode = document.CreateNode(XmlNodeType.Element, "Expect", null);
      foreach (IVariable var in Expect) {
        XmlNode varNode = document.CreateNode(XmlNodeType.Element, "Parameter", null);
        XmlAttribute varNameAttrib = document.CreateAttribute("Name");
        varNameAttrib.Value = (string)var.Name.Clone();
        varNode.Attributes.Append(varNameAttrib);
        varNode.AppendChild(var.Value.GetXmlNode("Value", document, persistedObjects));
        expectNode.AppendChild(varNode);
      }
      dataNode.AppendChild(expectNode);
      node.AppendChild(dataNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Protocol = node.Attributes.GetNamedItem("Protocol").Value;
      Source = node.Attributes.GetNamedItem("Source").Value;
      Destination = node.Attributes.GetNamedItem("Destination").Value;
      string ts = node.Attributes.GetNamedItem("Timestamp").Value;
      string[] tsTokens = ts.Split(new string[] { "/", " ", ":" }, StringSplitOptions.RemoveEmptyEntries);
      int year = int.Parse(tsTokens[0]);
      int month = int.Parse(tsTokens[1]);
      int day = int.Parse(tsTokens[2]);
      int hour = int.Parse(tsTokens[3]);
      int minute = int.Parse(tsTokens[4]);
      int seconds = int.Parse(tsTokens[5]);
      int milliseconds = int.Parse(tsTokens[6]);
      Timestamp = new DateTime(year, month, day, hour, minute, seconds, milliseconds);
      XmlNode dataNode = node.SelectSingleNode("Data");
      XmlNode giveNode = dataNode.SelectSingleNode("Give");
      XmlNodeList giveParams = giveNode.SelectNodes("Parameter");
      Give = new List<IVariable>();
      foreach (XmlNode n in giveParams) {
        Give.Add(new Variable(n.Attributes.GetNamedItem("Name").Value, (IItem)PersistenceManager.Restore(n.SelectSingleNode("Value"), restoredObjects)));
      }
      XmlNode expectNode = dataNode.SelectSingleNode("Expect");
      XmlNodeList expectParams = expectNode.SelectNodes("Parameter");
      Expect = new List<IVariable>();
      foreach (XmlNode n in expectParams) {
        Expect.Add(new Variable(n.Attributes.GetNamedItem("Name").Value, (IItem)PersistenceManager.Restore(n.SelectSingleNode("Value"), restoredObjects)));
      }
    }
    #endregion
  }
}
