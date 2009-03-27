using System;
using System.Text;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class String2XmlFormatter : IFormatter {

    public Type Type { get { return typeof(string); } }
    public IFormat Format { get { return XmlFormat.Instance;  } }

    public object DoFormat(object o) {      
      return "<![CDATA[" +
        ((string)o).Replace("]]>", "]]]]><![CDATA[>") +
        "]]>";
    }

    public object Parse(object o) {
      StringBuilder sb = new StringBuilder();
      foreach (string s in ((string)o).Split(
        new[] { "<![CDATA[", "]]>" },
        StringSplitOptions.RemoveEmptyEntries)) {
        sb.Append(s);
      }
      return sb.ToString();
    }
  }  
}