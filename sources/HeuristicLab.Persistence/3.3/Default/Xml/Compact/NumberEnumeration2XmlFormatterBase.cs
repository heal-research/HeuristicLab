using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;

namespace HeuristicLab.Persistence.Default.Xml.Compact {
  
  public abstract class NumberEnumeration2XmlFormatterBase : IFormatter {

    public abstract Type Type { get; }
    public IFormat Format { get { return XmlFormat.Instance; } }
    protected virtual string Separator { get { return ";"; } }
    protected abstract void Add(IEnumerable enumeration, object o);
    protected abstract object Instantiate();
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public object DoFormat(object o) {
      StringBuilder sb = new StringBuilder();
      foreach (var value in (IEnumerable)o) {
        sb.Append(FormatValue(value));
        sb.Append(Separator);
      }
      return sb.ToString();
    }

    public object Parse(object o) {
      IEnumerable enumeration = (IEnumerable)Instantiate();
      string[] values = ((string)o).Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var value in values) {
        Add(enumeration, ParseValue(value));
      }
      return enumeration;
    }
  }
  
}