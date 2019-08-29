#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Persistence.Default.Xml.Primitive;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  internal abstract class DoubleArray2XmlSerializerBase<T> : NumberArray2XmlSerializerBase<T> where T : class {

    protected override string FormatValue(object o) {
      return Double2XmlSerializer.FormatG17((double)o);
    }

    protected override object ParseValue(string o) {
      return Double2XmlSerializer.ParseG17(o);
    }
  }

  internal sealed class Double1DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[]> { }

  internal sealed class Double2DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[,]> { }

  internal sealed class Double3DArray2XmlSerializer : DoubleArray2XmlSerializerBase<double[, ,]> { }

}