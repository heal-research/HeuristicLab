using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {  

  public abstract class SimpleNumber2XmlFormatterBase<T> : IFormatter {

    public Type Type { get { return typeof(T); } }

    public IFormat Format { get { return XmlFormat.Instance; } }

    private static MethodInfo ParseMethod = typeof(T)
      .GetMethod(
        "Parse",
        BindingFlags.Static | BindingFlags.Public,
        null,
        CallingConventions.Standard,
        new[] { typeof(string) },
        null);

    public object DoFormat(object o) {
      return ((T)o).ToString();
    }
    public object Parse(object o) {
      return ParseMethod.Invoke(null, new[] { o });      
    }
  }  
}