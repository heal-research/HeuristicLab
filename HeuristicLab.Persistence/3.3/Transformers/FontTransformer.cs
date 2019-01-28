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
using HEAL.Attic;

namespace HeuristicLab.Persistence.Transformers {

  [Transformer("AFF27987-3301-4D70-9601-EFCA31BDA0DB", 405)]
  [StorableType("B9F9A371-4DCB-478B-B0D4-6F87A15C02B5")]
  internal sealed class FontTransformer : BoxTransformer<Font> {
    protected override void Populate(Box box, Font value, Mapper mapper) {
      var uints = box.UInts;
      uints.Add(mapper.GetStringId(GetFontFamilyName(value.FontFamily)));
      uints.Add(mapper.GetBoxId(value.Size));
      uints.Add(mapper.GetBoxId(value.Style));
      uints.Add(mapper.GetBoxId(value.Unit));
      uints.Add(mapper.GetBoxId(value.GdiCharSet));
      uints.Add(mapper.GetBoxId(value.GdiVerticalFont));
    }

    protected override Font Extract(Box box, Type type, Mapper mapper) {
      var fontData = box.UInts;
      return new Font(
        GetFontFamily(mapper.GetString(fontData[0])),
        (float)mapper.GetObject(fontData[1]),
        (FontStyle)mapper.GetObject(fontData[2]),
        (GraphicsUnit)mapper.GetObject(fontData[3]),
        (byte)mapper.GetObject(fontData[4]),
        (bool)mapper.GetObject(fontData[5])
      );
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
