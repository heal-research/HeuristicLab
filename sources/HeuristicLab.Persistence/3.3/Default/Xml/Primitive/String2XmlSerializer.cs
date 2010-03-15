using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  /// <summary>
  /// Serializes a string to XML by embedding into a CDATA block.
  /// </summary>
  public sealed class String2XmlSerializer : PrimitiveXmlSerializerBase<string> {

    /// <summary>
    /// Formats the specified string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>An XmlString that embeds the string s in a CDATA section.</returns>
    public override XmlString Format(string s) {
      StringBuilder sb = new StringBuilder();
      sb.Append("<![CDATA[");
      sb.Append(s.Replace("]]>", "]]]]><![CDATA[>"));
      sb.Append("]]>");
      return new XmlString(sb.ToString());
    }

    private static Regex re = new Regex(@"<!\[CDATA\[((?:[^]]|\](?!\]>))*)\]\]>", RegexOptions.Singleline);

    /// <summary>
    /// Parses the specified XmlString into a string.
    /// </summary>
    /// <param name="x">The XMLString.</param>
    /// <returns>The plain string contained in the XML CDATA section.</returns>
    public override string Parse(XmlString x) {
      StringBuilder sb = new StringBuilder();
      foreach (Match m in re.Matches(x.Data)) {
        sb.Append(m.Groups[1]);
      }
      string result = sb.ToString();
      if (result.Length == 0 && x.Data.Length > 0 && !x.Data.Equals("<![CDATA[]]>"))
        throw new PersistenceException("Invalid CDATA section during string parsing.");
      return result;
    }
  }
}