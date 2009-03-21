using System;
using System.Xml;

namespace HeuristicLab.Visualization.LabelProvider {
  public class DiscreteLabelProvider : ILabelProvider {
    public string GetLabel(double value) {
      int index = (int)Math.Round(value);
      double delta = Math.Abs(index - value);

      if (delta < 1e-10)
        return index.ToString();
      else
        return string.Empty;
    }

    public XmlNode GetLabelProviderXmlNode()
    {
      XmlDocument Xdoc = new XmlDocument();

      XmlNode lblProvInfo = Xdoc.CreateNode(XmlNodeType.Element, "LabelProvider", null);
      lblProvInfo.InnerText = "DiscreteLabelProvider";

      return lblProvInfo;
    }

    public ILabelProvider PopulateLabelProviderXmlNode(XmlNode node) {
      var labelProvider = new DiscreteLabelProvider();
      return labelProvider;
    }
  }
}