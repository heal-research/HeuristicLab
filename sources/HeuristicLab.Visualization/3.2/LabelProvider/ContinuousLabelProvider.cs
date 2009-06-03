using System.Globalization;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.LabelProvider {
  public class ContinuousLabelProvider : StorableBase, ILabelProvider {
    private string format;

    public ContinuousLabelProvider() {}

    public ContinuousLabelProvider(string format) {
      this.format = format;
    }

    public string GetLabel(double value) {
      return value.ToString(format, CultureInfo.InvariantCulture);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, System.Collections.Generic.IDictionary<System.Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlSupport.SetAttribute("Format", format, node);

      return node;
    }

    public override void Populate(XmlNode node, System.Collections.Generic.IDictionary<System.Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      this.format = XmlSupport.GetAttribute("Format", "0", node);
    }
  }
}