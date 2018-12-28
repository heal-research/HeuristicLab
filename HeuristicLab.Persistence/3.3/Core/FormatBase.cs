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
using HEAL.Fossil;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Common base class for defining a new serialization format.
  /// </summary>
  /// <typeparam name="SerialDataFormat">The type of the serial data format.</typeparam>
  [StorableClass]
  public abstract class FormatBase<SerialDataFormat> : IFormat<SerialDataFormat> where SerialDataFormat : ISerialData {

    /// <summary>
    /// Gets the format's name.
    /// </summary>
    /// <value>The format's name.</value>
    public abstract string Name { get; }

    /// <summary>
    /// Datatype that describes the atoms used for serialization serialization.
    /// </summary>
    public Type SerialDataType { get { return typeof(SerialDataFormat); } }

    [StorableConstructor]
    protected FormatBase(StorableConstructorFlag _) { }
    protected FormatBase() { }

    /// <summary>
    /// Compares formats by name.
    /// </summary>
    /// <param name="f">The format.</param>
    /// <returns>wheter this object and f are equal by name.</returns>
    public bool Equals(FormatBase<SerialDataFormat> f) {
      if (f == null)
        return false;
      return f.Name == this.Name;
    }

    /// <summary>
    /// Compares foramts by name.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.NullReferenceException">
    /// The <paramref name="obj"/> parameter is null.
    /// </exception>
    public override bool Equals(object obj) {
      FormatBase<SerialDataFormat> f = obj as FormatBase<SerialDataFormat>;
      return Equals(f);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode() {
      return Name.GetHashCode();
    }

  }

}