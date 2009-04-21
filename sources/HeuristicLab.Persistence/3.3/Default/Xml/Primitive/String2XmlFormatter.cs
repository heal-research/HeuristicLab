using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class String2XmlFormatter : FormatterBase<string, XmlString> {

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
      return sb.ToString();
    }
  }
}