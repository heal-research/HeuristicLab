#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Mark the member of a class to be considered by the <c>StorableSerializer</c>.
  /// The class must be marked as <c>[StorableClass]</c> and the
  /// <c>StorableClassType</c> should be set to <c>MarkedOnly</c> for
  /// this attribute to kick in.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
  public class StorableAttribute : Attribute {

    /// <summary>
    /// An optional name for this member that will be used during serialization.
    /// This allows to rename a field/property in code but still be able to read
    /// the old serialized format.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }


    /// <summary>
    /// A default value in case the field/property was not present or not serialized
    /// in a previous version of the class and could therefore be absent during
    /// deserialization.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue { get; set; }

    /// <summary>
    /// Allow storable attribute on properties with only a getter or a setter. These
    /// properties will then by either only serialized but not deserialized or only
    /// deserialized (if stored) but not serialized again.
    /// </summary>
    public bool AllowOneWay { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[Storable");
      if (Name != null || DefaultValue != null)
        sb.Append('(');
      if (Name != null) {
        sb.Append("Name = \"").Append(Name).Append("\"");
        if (DefaultValue != null)
          sb.Append(", ");
      }
      if (DefaultValue != null)
        sb.Append("DefaultValue = \"").Append(DefaultValue).Append("\"");
      if (Name != null || DefaultValue != null)
        sb.Append(')');
      sb.Append(']');
      return sb.ToString();
    }
  }
}
