using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Google.Protobuf;
using HEAL.Fossil;

namespace HeuristicLab.Persistence {
  [Transformer("D0ADB806-2DFD-459D-B5DA-14B5F1152534", 404)]
  [StorableType("B13D6153-E71D-4B76-9893-81D3570403E8")]
  internal sealed class BitmapTransformer : BoxTransformer<Bitmap> {
    protected override void Populate(Box box, Bitmap value, Mapper mapper) {
      lock (value)
        using (var ms = new MemoryStream()) {
          value.Save(ms, ImageFormat.Png);
          box.Bytes = ByteString.CopyFrom(ms.ToArray());
        }
    }

    protected override Bitmap Extract(Box box, Type type, Mapper mapper) {
      using (var ms = new MemoryStream()) {
        ms.Write(box.Bytes.ToArray(), 0, box.Bytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        return new Bitmap(ms);
      }
    }
  }

}
