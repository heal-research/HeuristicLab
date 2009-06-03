using System;
using System.Collections.Generic;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.LabelProvider {
  public class StringLabelProvider : StorableBase, ILabelProvider {
    private readonly Dictionary<int, string> labels = new Dictionary<int, string>();

    public void ClearLabels() {
      labels.Clear();
    }

    public void SetLabel(int index, string label) {
      labels[index] = label;
    }

    public string GetLabel(double value) {
      int index = (int)Math.Round(value);
      double delta = Math.Abs(index - value);

      string label;

      if (delta < 1e-10 && labels.TryGetValue(index, out label))
        return label;
      else
        return string.Empty;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlNode labelsNode = document.CreateElement("Labels");

      foreach (KeyValuePair<int, string> pair in labels) {
        int index = pair.Key;
        string label = pair.Value;

        XmlNode labelNode = document.CreateElement("Label");

        XmlSupport.SetAttribute("Index", index.ToString(), labelNode);
        XmlSupport.SetAttribute("Value", label, labelNode);

        labelsNode.AppendChild(labelNode);
      }

      node.AppendChild(labelsNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      foreach (XmlNode labelNode in node.SelectNodes("Labels/Label")) {
        int index = int.Parse(XmlSupport.GetAttribute("Index", labelNode));
        string label = XmlSupport.GetAttribute("Value", labelNode);

        this.SetLabel(index, label);
      }
    }
  }
}