using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Persistence.Default.Xml.Compact {
  public class ByteArray2XmlSerializer<T> : NumberArray2XmlSerializerBase<T> where T : class {
    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
      return byte.Parse(o);
    }
  }

  public class Byte1DArray2XmlSerializer : ByteArray2XmlSerializer<byte[]> { }

  public class Bytet2DArray2XmlSerializer : ByteArray2XmlSerializer<byte[,]> { }

  public class Byte3DArray2XmlSerializer : ByteArray2XmlSerializer<byte[, ,]> { }
}
