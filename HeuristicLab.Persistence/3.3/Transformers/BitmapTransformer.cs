#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Google.Protobuf;
using HEAL.Attic;

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
