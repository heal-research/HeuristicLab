using System;
using System.Xml;

namespace HeuristicLab.Visualization {
  public static class XmlSupport {
    public static void SetAttribute(string name, string value, XmlNode element) {
      XmlAttribute attributeNode = element.OwnerDocument.CreateAttribute(name);
      attributeNode.Value = value;
      element.Attributes.Append(attributeNode);
    }

    public static string GetAttribute(string name, string @default, XmlNode element) {
      XmlAttribute attributeNode = element.Attributes[name];

      if (attributeNode == null)
        return @default;
      else
        return attributeNode.Value;
    }

    public static string GetAttribute(string name, XmlNode element) {
      string value = GetAttribute(name, null, element);

      if (value == null)
        throw new Exception(string.Format("Attribute '{0}' not found.", name));

      return value;
    }
  }
}