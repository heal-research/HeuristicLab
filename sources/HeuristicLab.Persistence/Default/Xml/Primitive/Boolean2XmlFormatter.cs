using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class Boolean2XmlFormatter : IFormatter {    

    public Type Type { get { return typeof(bool); } }
    public IFormat Format { get { return XmlFormat.Instance; } }

    public object DoFormat(object o) {
      return ((bool)o).ToString();
    }

    public object Parse(object o) {
      return bool.Parse((string)o);
    }

  }  

}