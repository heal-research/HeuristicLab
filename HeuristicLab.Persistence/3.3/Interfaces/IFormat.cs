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

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// Interface of a new serialization output format. Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>
  public interface IFormat {

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }

    /// <summary>
    /// Gets the type of the serial data.
    /// </summary>
    /// <value>The type of the serial data.</value>
    Type SerialDataType { get; }
  }

  /// <summary>
  /// Marker interface for new serialization output format.  Instead of implementing this
  /// interface, derive from FormatBase.
  /// </summary>
  /// <typeparam name="SerialDataFormat">The type of the serial data format.</typeparam>
  public interface IFormat<SerialDataFormat> : IFormat where SerialDataFormat : ISerialData {
  }

}