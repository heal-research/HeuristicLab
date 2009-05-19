using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  public class String2XmlSerializer : PrimitiveXmlSerializerBase<string> {

    public override XmlString Format(string s) {
      StringBuilder sb = new StringBuilder();
      sb.Append("<![CDATA[");
      sb.Append(s.Replace("]]>", "]]]]><![CDATA[>"));
      sb.Append("]]>");
      return new XmlString(sb.ToString());
    }

    public override string Parse(XmlString x) {
      StringBuilder sb = new StringBuilder();
      Regex re = new Regex(@"<!\[CDATA\[((?:[^]]|\](?!\]>))*)\]\]>", RegexOptions.Singleline);
      foreach (Match m in re.Matches(x.Data)) {
        sb.Append(m.Groups[1]);
      }
      string result = sb.ToString();
      if (result.Length == 0 && x.Data.Length > 0 && !x.Data.Equals("<![CDATA[]]>"))
        throw new PersistenceException("Invalid CDATA section during string parsing.");
      return sb.ToString();
    }
  }
}