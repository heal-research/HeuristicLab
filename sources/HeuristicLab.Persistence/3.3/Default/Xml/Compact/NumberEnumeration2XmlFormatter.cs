using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;

namespace HeuristicLab.Persistence.Default.Xml.Compact {
  
  public abstract class NumberEnumeration2XmlFormatter : IFormatter {

    public abstract Type Type { get; }
    public IFormat Format { get { return XmlFormat.Instance; } }
    protected virtual string Separator { get { return ";"; } }
    protected abstract void Add(IEnumerable enumeration, object o);
    protected abstract object Instantiate();
    protected abstract string formatValue(object o);
    protected abstract object parseValue(string o);

    public object DoFormat(object o) {
      StringBuilder sb = new StringBuilder();
      foreach (var value in (IEnumerable)o) {
        sb.Append(formatValue(value));
        sb.Append(Separator);
      }
      return sb.ToString();
    }

    public object Parse(object o) {
      IEnumerable enumeration = (IEnumerable)Instantiate();
      string[] values = ((string)o).Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var value in values) {
        Add(enumeration, parseValue(value));
      }
      return enumeration;
    }
  }
  
}