using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml.Primitive;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  internal abstract class DoubleArray2XmlSerializerBase<T> : NumberArray2XmlSerializerBase<T> where T : class {

    protected override string FormatValue(object o) {
      return Double2XmlSerializer.FormatG17((double)o);
    }

    protected override object ParseValue(string o) {
      return Double2XmlSerializer.ParseG17(o);
    }
  }

  internal sealed class Double1DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[]> { }

  internal sealed class Double2DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[,]> { }

  internal sealed class Double3DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[, ,]> { }

}