using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {  
  
  public abstract class DecimalNumber2XmlFormatterBase<T> : IFormatter {

    public Type Type { get { return typeof(T); } }

    public IFormat Format { get { return XmlFormat.Instance; } }

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

    public object DoFormat(object o) {
      return ToStringMethod.Invoke(o, new object[] { "r", CultureInfo.InvariantCulture });
    }
    public object Parse(object o) {      
      return ParseMethod.Invoke(null, new[] { o, CultureInfo.InvariantCulture });
    }
  }  
}