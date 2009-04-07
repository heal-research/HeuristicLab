using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class Int2XmlFormatter : IFormatter {

    public Type Type { get { return typeof(int); } }
    public IFormat Format { get { return XmlFormat.Instance; } }

    public object DoFormat(object o) {
      return ((int)o).ToString();
    }

    public object Parse(object o) {
      return int.Parse((string)o);
    }

  }
  
}