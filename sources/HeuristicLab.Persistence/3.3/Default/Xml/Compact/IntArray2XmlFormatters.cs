using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public abstract class IntArray2XmlFormatterBase<T> : NumberArray2XmlFormatterBase<T> where T : class{

    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
      return int.Parse(o);
    }
  }

  [EmptyStorableClass]
  public class Int1DArray2XmlFormatter : IntArray2XmlFormatterBase<int[]> { }

  [EmptyStorableClass]
  public class Int2DArray2XmlFormatter : IntArray2XmlFormatterBase<int[,]> { }

  [EmptyStorableClass]
  public class Int3DArray2XmlFormatter : IntArray2XmlFormatterBase<int[, ,]> { }

}