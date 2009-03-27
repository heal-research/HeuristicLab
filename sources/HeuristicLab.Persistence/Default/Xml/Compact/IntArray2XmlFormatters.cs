using System;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public class IntArray2XmlFormatter : NumberArray2XmlFormatter {

    public override Type Type {
      get {
        return typeof(int[]);
      }
    }

    protected override string formatValue(object o) {
      return o.ToString();
    }

    protected override object parseValue(string o) {
      return int.Parse(o);
    }
  }


  [EmptyStorableClass]
  public class Int2DArray2XmlFormatter : IntArray2XmlFormatter {
    public override Type Type {
      get {
        return typeof(int[,]);
      }
    }
  }


  [EmptyStorableClass]
  public class Int3DArray2XmlFormatter : IntArray2XmlFormatter {
    public override Type Type {
      get {
        return typeof(int[, ,]);
      }
    }
  }
  
}