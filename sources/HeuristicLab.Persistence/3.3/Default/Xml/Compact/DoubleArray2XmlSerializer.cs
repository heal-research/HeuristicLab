using System;
using HeuristicLab.Persistence.Core;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public abstract class DoubleArray2XmlSerializerBase<T> : NumberArray2XmlSerializerBase<T> where T : class {

    protected override string FormatValue(object o) {
      return ((double)o).ToString("r", CultureInfo.InvariantCulture);
    }

    protected override object ParseValue(string o) {
      return double.Parse(o, CultureInfo.InvariantCulture);
    }
  }

  public class Double1DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[]> { }

  public class Double2DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[,]> { }

  public class Double3DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[, ,]> { }

}