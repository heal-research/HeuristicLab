#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using HeuristicLab.Persistence.Default.Xml.Compact;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {
  internal sealed class Bitmap2XmlSerializer : PrimitiveXmlSerializerBase<Bitmap> {

    public override XmlString Format(Bitmap o) {      
      MemoryStream stream = new MemoryStream();
      lock (o)
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
