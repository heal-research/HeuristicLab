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

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Marker interface primitive serializers. Transform data of type SourceType
  /// into the serialization format SerialDataType. Derive from PrimitiveSerializerBase instead
  /// of implementing this interface.
  /// </summary>
  public interface IPrimitiveSerializer {

    /// <summary>
    /// Gets the type of the serial data.
    /// </summary>
    /// <value>The type of the serial data.</value>
    Type SerialDataType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    /// <value>The type of the source.</value>
    Type SourceType { get; }

    /// <summary>
    /// Creates a serialized representation of the provided object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    ISerialData Format(object o);


    /// <summary>
    /// Creates a fresh object instance using the serializes data..
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>A fresh object instance.</returns>
    object Parse(ISerialData data);
  }

  /// <summary>
  /// Marker interface primitive serializers. Transform data of type SourceType
  /// into the serialization format SerialDataType. Derive from PrimitiveSerializerBase instead
  /// of implementing this interface.
  /// </summary>
  /// <typeparam name="Source">The source type.</typeparam>
  /// <typeparam name="SerialData">The serialized data type.</typeparam>
  public interface IPrimitiveSerializer<Source, SerialData> : IPrimitiveSerializer where SerialData : ISerialData {

    /// <summary>
    /// Creates a serialized version of the provided object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A serialized version of the object.</returns>
    SerialData Format(Source o);


    /// <summary>
    /// Creates a fresh object instance from the serialized data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>A fresh object instance.</returns>
    Source Parse(SerialData data);
  }

}