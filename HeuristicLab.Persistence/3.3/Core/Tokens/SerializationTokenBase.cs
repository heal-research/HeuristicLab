#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Common base class for all serialization tokens.
  /// </summary>
  public abstract class SerializationTokenBase : ISerializationToken {

    /// <summary>
    /// The token's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializationTokenBase"/> class.
    /// </summary>
    /// <param name="name">The token name.</param>
    public SerializationTokenBase(string name) {
      Name = name;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.GetType().Name).Append('(');
      foreach (FieldInfo fi in this.GetType().GetFields()) {
        sb.Append(fi.Name).Append('=').Append(fi.GetValue(this)).Append(", ");
      }
      sb.Append(')');
      return sb.ToString();
    }
  }

}