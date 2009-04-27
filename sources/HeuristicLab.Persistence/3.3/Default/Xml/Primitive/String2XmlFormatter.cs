using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Default.Decomposers.Storable;
using System.Globalization;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class TimeSpan2XmlFormatter : PrimitiveXmlFormatterBase<TimeSpan> {

    public override XmlString Format(TimeSpan o) {
      return new XmlString(o.ToString());
    }

    public override TimeSpan Parse(XmlString t) {
      try {
        return TimeSpan.Parse(t.Data);
      } catch (FormatException x) {
        throw new PersistenceException("Cannot parse TimeSpan string representation.", x);
      } catch (OverflowException x) {
        throw new PersistenceException("Overflow during TimeSpan parsing.", x);
      }
    }
  }

  [EmptyStorableClass]
  public class Guid2XmlFormatter : PrimitiveXmlFormatterBase<Guid> {

    public override XmlString Format(Guid o) {
      return new XmlString(o.ToString("D", CultureInfo.InvariantCulture));
    }

    public override Guid Parse(XmlString t) {
      try {
        return new Guid(t.Data);
      } catch (FormatException x) {
        throw new PersistenceException("Cannot parse Guid string representation.", x);
      } catch (OverflowException x) {
        throw new PersistenceException("Overflow during Guid parsing.", x);
      }
    }
  }

  [EmptyStorableClass]
  public class String2XmlFormatter : PrimitiveXmlFormatterBase<string> {

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