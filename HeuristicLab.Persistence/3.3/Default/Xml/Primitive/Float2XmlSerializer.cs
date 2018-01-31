#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class Float2XmlSerializer : PrimitiveXmlSerializerBase<float> {

    public static float ParseG8(string s) {
      float f;
      if (float.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out f))
        return f;
      throw new FormatException(
        String.Format("Invalid float G8 number format \"{0}\" could not be parsed", s));
    }

    public static string FormatG8(float f) {
      return f.ToString("g8", CultureInfo.InvariantCulture);
    }

    public override XmlString Format(float f) {
      return new XmlString(FormatG8(f));
    }

    public override float Parse(XmlString t) {
      return ParseG8(t.Data);
    }
  }
}