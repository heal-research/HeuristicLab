using System;
using HeuristicLab.Persistence.Core;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Compact {
  
  public abstract class DoubleArray2XmlFormatterBase<T> : NumberArray2XmlFormatterBase<T> {
    
    protected override string FormatValue(object o) {
      return ((double)o).ToString("r", CultureInfo.InvariantCulture);
    }

    protected override object ParseValue(string o) {
      return double.Parse(o);
    }
  }

  [EmptyStorableClass]
  public class Double1DArray2XmlFormatter : DoubleArray2XmlFormatterBase<double[]> { }


  [EmptyStorableClass]
  public class Double2DArray2XmlFormatter : DoubleArray2XmlFormatterBase<double[,]> { }

  [EmptyStorableClass]
  public class Double3DArray2XmlFormatter : DoubleArray2XmlFormatterBase<double[,,]> { }
  
}