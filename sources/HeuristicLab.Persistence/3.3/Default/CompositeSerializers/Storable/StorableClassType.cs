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


namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  /// <summary>
  /// Specifies which memebrs are selected for serialization by the StorableSerializer
  /// </summary>
  public enum StorableClassType {

    /// <summary>
    /// Serialize only fields and properties that have been marked
    /// with the [Storable] attribute. This is the default value.
    /// </summary>
    MarkedOnly,

    /// <summary>
    /// Serialize all fields but ignore the 
    /// [Storable] attribute on properties.
    /// 
    /// This does not include generated backing fields
    /// for automatic properties.
    /// </summary>    
    AllFields,

    /// <summary>
    /// Serialize all properties but ignore the
    /// [Storable] attribute on fields.
    /// </summary>    
    AllProperties,

    /// <summary>
    /// Serialize all fields and all properties
    /// regardless of the [Storable] attribute.
    /// 
    /// This does not include generated backing fields
    /// for automatic properties, but uses the property
    /// accessors instead.
    /// </summary>    
    AllFieldsAndAllProperties
  };
}

