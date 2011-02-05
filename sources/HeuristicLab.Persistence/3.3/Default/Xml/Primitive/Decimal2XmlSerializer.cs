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
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class Decimal2XmlSerializer : PrimitiveXmlSerializerBase<decimal> {

    public static decimal ParseG30(string s) {
      decimal d;
      if (decimal.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d))
        return d;
      throw new FormatException(
        String.Format("Invalid decimal G30 number format \"{0}\" could not be parsed", s));
    }

    public static string FormatG30(decimal d) {
      return d.ToString("g30", CultureInfo.InvariantCulture);
    }

    public override XmlString Format(decimal d) {
      return new XmlString(FormatG30(d));
    }

    public override decimal Parse(XmlString t) {
      return ParseG30(t.Data);
    }
  }
}