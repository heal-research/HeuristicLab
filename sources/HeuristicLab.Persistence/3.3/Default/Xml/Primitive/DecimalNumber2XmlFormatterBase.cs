using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  public abstract class DecimalNumber2XmlFormatterBase<T> : FormatterBase<T, XmlString> {

    private static MethodInfo ToStringMethod = typeof(T)
      .GetMethod(
        "ToString",
        BindingFlags.Instance | BindingFlags.Public,
        null,
        CallingConventions.Standard,
        new[] { typeof(string), typeof(IFormatProvider) },
        null);

    private static MethodInfo ParseMethod = typeof(T)
      .GetMethod(
        "Parse",
        BindingFlags.Static | BindingFlags.Public,
        null,
        CallingConventions.Standard,
        new[] { typeof(string), typeof(IFormatProvider) },
        null);

    public override XmlString Format(T t) {
      return new XmlString((string)ToStringMethod.Invoke(t, new object[] { "r", CultureInfo.InvariantCulture }));
    }
    public override T Parse(XmlString x) {
      return (T)ParseMethod.Invoke(null, new object[] { x.Data, CultureInfo.InvariantCulture });
    }
  }
}