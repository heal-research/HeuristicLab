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

using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Association of id, type name and serializer
  /// </summary>
  public class TypeMapping {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The type's full name.
    /// </summary>
    public readonly string TypeName;


    /// <summary>
    /// The full name of the serializes used to serialize the
    /// <see cref="TypeName"/>.
    /// </summary>
    public readonly string Serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapping"/> class.
    /// </summary>
    /// <param name="id">The type's id.</param>
    /// <param name="typeName">The type's full name.</param>
    /// <param name="serializer">The full name of the serializer use to
    /// serialize this type.</param>
    public TypeMapping(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }

    /// <summary>
    /// Creates a dictionary that conatins all properties
    /// and values of this instance.
    /// </summary>
    /// <returns>A dictionary containing all properties
    /// and values of this instance.</returns>
    public Dictionary<string, string> GetDict() {
      return new Dictionary<string, string> {
        {"id", Id.ToString()},
        {"typeName", TypeName},
        {"serializer", Serializer}};
    }

  }

}