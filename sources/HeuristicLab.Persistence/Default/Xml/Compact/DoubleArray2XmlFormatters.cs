using System;
using HeuristicLab.Persistence.Core;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public class DoubleArray2XmlFormatter : NumberArray2XmlFormatter {

    public override Type Type {
      get {
        return typeof(double[]);
      }
    }

    protected override string formatValue(object o) {
      return ((double)o).ToString("r", CultureInfo.InvariantCulture);
    }

    protected override object parseValue(string o) {
      return double.Parse(o);
    }
  }

  [EmptyStorableClass]
  public class Double2DArray2XmlFormatter : DoubleArray2XmlFormatter {
    public override Type Type {
      get {
        return typeof(double[,]);
      }
    }
  }

  [EmptyStorableClass]
  public class Double3DArray2XmlFormatter : DoubleArray2XmlFormatter {
    public override Type Type {
      get {
        return typeof(double[, ,]);
      }
    }
  }
  
}