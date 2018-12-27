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


namespace HeuristicLab.Persistence.Default.Xml.Compact {
  internal class ByteArray2XmlSerializer<T> : NumberArray2XmlSerializerBase<T> where T : class {
    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
      return byte.Parse(o);
    }
  }

  internal sealed class Byte1DArray2XmlSerializer : ByteArray2XmlSerializer<byte[]> { }

  internal sealed class Bytet2DArray2XmlSerializer : ByteArray2XmlSerializer<byte[,]> { }

  internal sealed class Byte3DArray2XmlSerializer : ByteArray2XmlSerializer<byte[, ,]> { }
}
