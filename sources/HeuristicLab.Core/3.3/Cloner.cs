#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.Core {
  /// <summary>
  /// A helper class which is used to create deep clones of object graphs.
  /// </summary>
  public sealed class Cloner {
    private class ReferenceEqualityComparer : IEqualityComparer<IDeepCloneable> {
      bool IEqualityComparer<IDeepCloneable>.Equals(IDeepCloneable x, IDeepCloneable y) {
        return object.ReferenceEquals(x, y);
      }

      int IEqualityComparer<IDeepCloneable>.GetHashCode(IDeepCloneable obj) {
        if (obj == null) return 0;
        return obj.GetHashCode();
      }
    }

    private Dictionary<IDeepCloneable, IDeepCloneable> mapping;

    /// <summary>
    /// Creates a new Cloner instance.
    /// </summary>
    public Cloner() {
      mapping = new Dictionary<IDeepCloneable, IDeepCloneable>(new ReferenceEqualityComparer());
    }

    /// <summary>
    /// Creates a deep clone of a given deeply cloneable object.
    /// </summary>
    /// <param name="item">The object which should be cloned.</param>
    /// <returns>A clone of the given object.</returns>
    public IDeepCloneable Clone(IDeepCloneable obj) {
      if (obj == null) return null;
      IDeepCloneable clone;
      if (mapping.TryGetValue(obj, out clone))
        return clone;
      else
        return obj.Clone(this);
    }
    /// <summary>
    /// Registers a new clone for a given deeply cloneable object.
    /// </summary>
    /// <param name="item">The original object.</param>
    /// <param name="clone">The clone of the original object.</param>
    public void RegisterClonedObject(IDeepCloneable item, IDeepCloneable clone) {
      mapping.Add(item, clone);
    }
  }
}
