using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class DateTime2XmlFormatter : IFormatter {    

    public Type Type { get { return typeof(DateTime); } }
    public IFormat Format { get { return XmlFormat.Instance; } }  

    public object DoFormat(object o) {
      return ((DateTime)o).Ticks.ToString();      
    }

    public object Parse(object o) {
      return new DateTime(long.Parse((string)o));
    }

  }

  
}