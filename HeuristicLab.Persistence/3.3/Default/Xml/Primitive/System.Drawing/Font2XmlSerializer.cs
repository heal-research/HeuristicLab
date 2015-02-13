#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
        GetFontFamilyName(font.FontFamily),
        Float2XmlSerializer.FormatG8(font.Size),
        font.Style,
        font.Unit,
        font.GdiCharSet,
        font.GdiVerticalFont));
    }

    public override Font Parse(XmlString fontData) {
      string[] tokens = fontData.Data.Split(';');
      return new Font(
        GetFontFamily(tokens[0]),
        Float2XmlSerializer.ParseG8(tokens[1]),
        (FontStyle)Enum.Parse(typeof(FontStyle), tokens[2]),
        (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), tokens[3]),
        byte.Parse(tokens[4]),
        bool.Parse(tokens[5]));
    }

    public const string GENERIC_MONOSPACE_NAME = "_GenericMonospace";
    public const string GENERIC_SANS_SERIF_NAME = "_GenericSansSerif";
    public const string GENERIC_SERIF_NAME = "_GenericSerif";

    public static FontFamily GetFontFamily(string name) {
      if (name == GENERIC_MONOSPACE_NAME) return FontFamily.GenericMonospace;
      if (name == GENERIC_SANS_SERIF_NAME) return FontFamily.GenericSansSerif;
      if (name == GENERIC_SERIF_NAME) return FontFamily.GenericSerif;
      return new FontFamily(name);
    }

    public static string GetFontFamilyName(FontFamily ff) {
      if (ff.Equals(FontFamily.GenericMonospace)) return GENERIC_MONOSPACE_NAME;
      if (ff.Equals(FontFamily.GenericSansSerif)) return GENERIC_SANS_SERIF_NAME;
      if (ff.Equals(FontFamily.GenericSerif)) return GENERIC_SERIF_NAME;
      return ff.Name;
    }

  }
}
