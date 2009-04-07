using System.Globalization;
using System.Xml;

namespace HeuristicLab.Visualization.LabelProvider {
  public class ContinuousLabelProvider : ILabelProvider {
    private readonly string format;

    public ContinuousLabelProvider() {}

    public ContinuousLabelProvider(string format) {
      this.format = format;
    }

    public string GetLabel(double value) {
      return value.ToString(format, CultureInfo.InvariantCulture);
    }

    public XmlNode GetLabelProviderXmlNode()
    {
      XmlDocument Xdoc = new XmlDocument();

      XmlNode lblProvInfo = Xdoc.CreateNode(XmlNodeType.Element, "LabelProvider", null);
      lblProvInfo.InnerText = "ContinuousLabelProvider";

      XmlAttribute idFormat = Xdoc.CreateAttribute("format");
      idFormat.Value = this.format;

      lblProvInfo.Attributes.Append(idFormat);

      return lblProvInfo;
    }

    public ILabelProvider PopulateLabelProviderXmlNode(XmlNode node) {
      var labelProvider = new ContinuousLabelProvider(node.SelectSingleNode("//LabelProvider").Attributes[0].Value);
      return labelProvider;
    }
  }
}