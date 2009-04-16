using System;
using System.Text;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Xml;

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

    private static readonly string[] separators = new string[] { "<![CDATA[", "]]>" };

    public override string Parse(XmlString x) {
      StringBuilder sb = new StringBuilder();
      foreach (string s in x.Data.Split(separators,        
        StringSplitOptions.RemoveEmptyEntries)) {
        sb.Append(s);
      }
      return sb.ToString();
    }
  }  
}