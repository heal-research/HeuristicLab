using System;
using System.Globalization;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class Double2XmlFormatter : IFormatter {    

    public Type Type { get { return typeof(double); } }
    public IFormat Format { get { return XmlFormat.Instance; } }

    public object DoFormat(object o) {
      return ((double)o).ToString("r", CultureInfo.InvariantCulture);      
    }

    public object Parse(object o) {
      return double.Parse((string)o, CultureInfo.InvariantCulture);
    }

  }
  
}