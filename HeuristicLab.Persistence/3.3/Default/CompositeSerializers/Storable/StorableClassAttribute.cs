#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {  

  /// <summary>
  /// Mark a class to be considered by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class StorableClassAttribute : Attribute {


    /// <summary>
    /// Specify how members are selected for serialization.
    /// </summary>
    public StorableClassType Type { get; private set; }

    /// <summary>
    /// Mark a class to be serialize by the <c>StorableSerizlier</c>
    /// </summary>
    /// <param name="type">The storable class type.</param>
    public StorableClassAttribute(StorableClassType type) {
      Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorableClassAttribute"/> class.
    /// The default value for <see cref="StorableClassType"/> is
    /// <see cref="StorableClassType.MarkedOnly"/>.
    /// </summary>
    public StorableClassAttribute() { }

  }
}

