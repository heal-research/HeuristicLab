using System;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public class IntArray2XmlFormatter : NumberArray2XmlFormatterBase {

    public override Type Type {
      get {
        return typeof(int[]);
      }
    }

    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
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