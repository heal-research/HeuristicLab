using System;
using System.Collections.Generic;
using HeuristicLab.Visualization.LabelProvider;
using System.Xml;

namespace HeuristicLab.Visualization.LabelProvider {
  public class StringLabelProvider : ILabelProvider {
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

    public XmlNode GetLabelProviderXmlNode() {
      XmlDocument Xdoc = new XmlDocument();

      XmlNode lblProvInfo = Xdoc.CreateNode(XmlNodeType.Element, "LabelProvider", null);
      lblProvInfo.InnerText = "StringLabelProvider";

      foreach (KeyValuePair<int, string> pair in labels)
      {
        XmlNode strLbl = Xdoc.CreateNode(XmlNodeType.Element, "String", null);

        XmlAttribute idStrLbl = Xdoc.CreateAttribute("id");
        idStrLbl.Value = pair.Key.ToString();
        strLbl.Attributes.Append(idStrLbl);

        strLbl.InnerText = pair.Value;
        lblProvInfo.AppendChild(strLbl);
      }
      return lblProvInfo;
    }

    public ILabelProvider PopulateLabelProviderXmlNode(XmlNode node) {
      var labelProvider = new StringLabelProvider();

      foreach (XmlNode strLbl in node.SelectNodes("//String"))
      {
        labelProvider.SetLabel(int.Parse(strLbl.Attributes[0].Value), strLbl.InnerText);
      }
      return labelProvider;
    }
  }
}