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

using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// A token containing type information and mapping.
  /// </summary>
  public class TypeToken : ISerializationToken {

    /// <summary>
    /// The type id.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The type's full name.
    /// </summary>
    public readonly string TypeName;

    /// <summary>
    /// The full type name of the serialized used to
    /// serialize the type.
    /// </summary>
    public readonly string Serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeToken"/> class.
    /// </summary>
    /// <param name="id">The type id.</param>
    /// <param name="typeName">Full name of the type.</param>
    /// <param name="serializer">The full name of the serializer
    /// used to serialize the type.</param>
    public TypeToken(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }
  }
}
