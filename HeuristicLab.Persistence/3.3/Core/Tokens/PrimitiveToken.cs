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

using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Encapsulated the serialization of a single primitive value.
  /// </summary>
  public class PrimitiveToken : SerializationTokenBase {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int TypeId;

    /// <summary>
    /// The object's id.
    /// </summary>
    public readonly int? Id;

    /// <summary>
    /// The serialized data.
    /// </summary>
    public readonly ISerialData SerialData;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveToken"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="typeId">The type's id.</param>
    /// <param name="id">The onbject's id.</param>
    /// <param name="serialData">The serialized data.</param>
    public PrimitiveToken(string name, int typeId, int? id, ISerialData serialData)
      : base(name) {
      TypeId = typeId;
      Id = id;
      SerialData = serialData;
    }
  }

}