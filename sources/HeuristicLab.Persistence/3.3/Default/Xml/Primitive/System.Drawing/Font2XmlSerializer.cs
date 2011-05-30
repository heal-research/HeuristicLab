#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Persistence.Default.Xml.Primitive {
  internal sealed class Font2XmlSerializer : PrimitiveXmlSerializerBase<Font> {

    public override XmlString Format(Font font) {
      return new XmlString(string.Format("{0};{1};{2};{3};{4};{5}",
        font.Name,
        Float2XmlSerializer.FormatG8(font.Size),
        font.Style,
        font.Unit,
        font.GdiCharSet,
        font.GdiVerticalFont));
    }

    public override Font Parse(XmlString fontData) {
      string[] tokens = fontData.Data.Split(';');
      return new Font(
        tokens[0],
        Float2XmlSerializer.ParseG8(tokens[1]),
        (FontStyle)Enum.Parse(typeof(FontStyle), tokens[2]),
        (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), tokens[3]),
        byte.Parse(tokens[4]),
        bool.Parse(tokens[5]));
    }
  }
}
