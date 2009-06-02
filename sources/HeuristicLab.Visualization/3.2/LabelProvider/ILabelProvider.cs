using System.Xml;

namespace HeuristicLab.Visualization.LabelProvider {
  public interface ILabelProvider {
    string GetLabel(double value);
    XmlNode GetLabelProviderXmlNode(XmlDocument document);
    ILabelProvider PopulateLabelProviderXmlNode(XmlNode node);
  }
}