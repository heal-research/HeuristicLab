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
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Interfaces {

  /// <summary>
  /// A composite serializer does not directly transform an object into its
  /// serialized form. It merely decomposes into other objects, that can
  /// later be used to recompose the same object.
  /// </summary>
  public interface ICompositeSerializer {

    /// <summary>
    /// Defines the Priorty of this composite serializer. Higher number means
    /// higher prioriy. Negative numbers are fallback serializers that are
    /// disabled by default.
    /// 
    /// All default generic composite serializers have priority 100. Specializations
    /// have priority 200 so they will  be tried first. Priorities are
    /// only considered for default configurations.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines for every type whether the composite serializer is applicable.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// <c>true</c> if this instance can serialize the specified type; otherwise, <c>false</c>.
    /// </returns>
    bool CanSerialize(Type type);

    /// <summary>
    /// Give a reason if possibly why the given type cannot be serialized by this
    /// ICompositeSerializer.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A string justifying why type cannot be serialized.</returns>
    string JustifyRejection(Type type);

    /// <summary>
    /// Generate MetaInfo necessary for instance creation. (e.g. dimensions
    /// necessary for array creation.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns>An enumerable of <see cref="Tag"/>s.</returns>
    IEnumerable<Tag> CreateMetaInfo(object obj);

    /// <summary>
    /// Decompose an object into <see cref="Tag"/>s, the tag name can be null,
    /// the order in which elements are generated is guaranteed to be
    /// the same as they will be supplied to the Populate method.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns>An enumerable of <see cref="Tag"/>s.</returns>
    IEnumerable<Tag> Decompose(object obj);

    /// <summary>
    /// Create an instance of the object using the provided meta information.
    /// </summary>
    /// <param name="type">A type.</param>
    /// <param name="metaInfo">The meta information.</param>
    /// <returns>A fresh instance of the provided type.</returns>
    object CreateInstance(Type type, IEnumerable<Tag> metaInfo);

    /// <summary>
    /// Fills an object with values from the previously generated <see cref="Tag"/>s
    /// in Decompose. The order in which the values are supplied is
    /// the same as they where generated. <see cref="Tag"/> names might be null.
    /// </summary>
    /// <param name="instance">An empty object instance.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="type">The type.</param>
    void Populate(object instance, IEnumerable<Tag> tags, Type type);
  }

}