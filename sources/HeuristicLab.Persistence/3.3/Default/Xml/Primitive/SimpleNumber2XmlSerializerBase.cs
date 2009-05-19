using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  public abstract class SimpleNumber2XmlSerializerBase<T> : PrimitiveXmlSerializerBase<T> {

    private static MethodInfo ParseMethod = typeof(T)
      .GetMethod(
        "Parse",
        BindingFlags.Static | BindingFlags.Public,
        null,
        CallingConventions.Standard,
        new[] { typeof(string) },
        null);

    public override XmlString Format(T t) {
      return new XmlString(t.ToString());
    }

    public override T Parse(XmlString x) {
      try {
        return (T)ParseMethod.Invoke(null, new[] { x.Data });
      } catch (Exception e) {
        throw new PersistenceException("Could not parse simple number.", e);
      }
    }
  }
}