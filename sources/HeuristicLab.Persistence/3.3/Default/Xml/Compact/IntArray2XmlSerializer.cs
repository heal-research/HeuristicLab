using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public abstract class IntArray2XmlSerializerBase<T> : NumberArray2XmlSerializerBase<T> where T : class{

    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
      return int.Parse(o);
    }
  }

  [EmptyStorableClass]
  public class Int1DArray2XmlSerializer : IntArray2XmlSerializerBase<int[]> { }

  [EmptyStorableClass]
  public class Int2DArray2XmlSerializer : IntArray2XmlSerializerBase<int[,]> { }

  [EmptyStorableClass]
  public class Int3DArray2XmlSerializer : IntArray2XmlSerializerBase<int[, ,]> { }

}