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

using System;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Base class for primitive serializers. These are serializers that map
  /// directly to a single datatype and directly produce a serializable object.
  /// </summary>
  /// <typeparam name="Source">The source type.</typeparam>
  /// <typeparam name="SerialData">The serialized type.</typeparam>
  [StorableClass]
  public abstract class PrimitiveSerializerBase<Source, SerialData> :
      IPrimitiveSerializer<Source, SerialData>
      where SerialData : ISerialData {

    /// <summary>
    /// Formats the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    public abstract SerialData Format(Source o);

    /// <summary>
    /// Parses the specified serialized data back into an object.
    /// </summary>
    /// <param name="data">The serial data.</param>
    /// <returns>
    /// A newly created object representing the serialized data.
    /// </returns>
    public abstract Source Parse(SerialData data);

    /// <summary>
    /// Gets the type of the serial data.
    /// </summary>
    /// <value>The type of the serial data.</value>
    public Type SerialDataType { get { return typeof(SerialData); } }

    /// <summary>
    /// Gets the type of the source.
    /// </summary>
    /// <value>The type of the source.</value>
    public Type SourceType { get { return typeof(Source); } }

    /// <summary>
    /// Formats the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    ISerialData IPrimitiveSerializer.Format(object o) {
      return Format((Source)o);
    }

    /// <summary>
    /// Parses the specified serialized data back into an object.
    /// </summary>
    /// <param name="data">The serial data.</param>
    /// <returns>A newly created object representing the serialized data.</returns>
    object IPrimitiveSerializer.Parse(ISerialData data) {
      return Parse((SerialData)data);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      return new StringBuilder()
        .Append(this.GetType().Name)
        .Append('(')
        .Append(SourceType.Name)
        .Append("->")
        .Append(SerialDataType.Name)
        .ToString();
    }

  }

}