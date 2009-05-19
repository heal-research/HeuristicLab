using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public abstract class NumberEnumeration2XmlSerializerBase<T> : CompactXmlSerializerBase<T> where T : IEnumerable {

    protected virtual string Separator { get { return ";"; } }
    protected abstract void Add(IEnumerable enumeration, object o);
    protected abstract IEnumerable Instantiate();
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public override XmlString Format(T t) {
      StringBuilder sb = new StringBuilder();
      foreach (var value in (IEnumerable)t) {
        sb.Append(FormatValue(value));
        sb.Append(Separator);
      }
      return new XmlString(sb.ToString());
    }

    public override T Parse(XmlString x) {
      try {
        IEnumerable enumeration = Instantiate();
        string[] values = x.Data.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var value in values) {
          Add(enumeration, ParseValue(value));
        }
        return (T)enumeration;
      } catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data during reconstruction of number enumerable.", e);
      } catch (OverflowException e) {
        throw new PersistenceException("Overflow during element parsing while trying to reconstruct number enumerable.", e);
      }
    }
  }

}