#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class Char2XmlSerializer : PrimitiveSerializerBase<char, XmlString> {

    private static readonly Regex base64Regex = new Regex("<Base64>(.+)</Base64>");

    private static bool IsSpecial(char c) {
      return c <= 0x1F && c != 0x9 && c != 0xA && c != 0xD;
    }

    private static string ToBase64String(char c) {
      return string.Format("<Base64>{0}</Base64>", Convert.ToBase64String(Encoding.ASCII.GetBytes(new[] {c})));
    }

    public override XmlString Format(char c) {
      return new XmlString(IsSpecial(c) ? ToBase64String(c) : new string(c, 1));
    }

    public override char Parse(XmlString x) {
      if (x.Data.Length <= 1) return x.Data[0];
      var m = base64Regex.Match(x.Data);
      if (m.Success)
        return Encoding.ASCII.GetString(Convert.FromBase64String(m.Groups[1].Value))[0];
      throw new PersistenceException("Invalid character format, XML string length != 1");
    }
  }
}