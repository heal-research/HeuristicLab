using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml.Compact;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {
  internal sealed class Bitmap2XmlSerializer : PrimitiveXmlSerializerBase<Bitmap> {

    public override XmlString Format(Bitmap o) {
      MemoryStream stream = new MemoryStream();
      o.Save(stream, ImageFormat.Png);
      byte[] array = stream.ToArray();
      Byte1DArray2XmlSerializer serializer = new Byte1DArray2XmlSerializer();
      return serializer.Format(array);
    }

    public override Bitmap Parse(XmlString t) {
      Byte1DArray2XmlSerializer serializer = new Byte1DArray2XmlSerializer();
      byte[] array = serializer.Parse(t);

      MemoryStream stream = new MemoryStream();
      stream.Write(array, 0, array.Length);
      stream.Seek(0, SeekOrigin.Begin);

      Bitmap bitmap = new Bitmap(stream);
      return bitmap;
    }
  }
}
