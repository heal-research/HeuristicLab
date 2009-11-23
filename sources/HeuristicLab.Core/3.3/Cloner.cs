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
  public class Cloner : ICloner {
    private class ReferenceEqualityComparer : IEqualityComparer<IItem> {
      public bool Equals(IItem x, IItem y) {
        return object.ReferenceEquals(x, y);
      }

      public int GetHashCode(IItem item) {
        if (item == null) return 0;
        return item.GetHashCode();
      }
    }

    private Dictionary<IItem, IItem> mapping;

    /// <summary>
    /// Creates a new Cloner instance.
    /// </summary>
    public Cloner() {
      mapping = new Dictionary<IItem, IItem>(new ReferenceEqualityComparer());
    }

    /// <summary>
    /// Creates a deep clone of a given item.
    /// </summary>
    /// <param name="item">The item which should be cloned.</param>
    /// <returns>A clone of the given item.</returns>
    public IItem Clone(IItem item) {
      if (item == null) return null;
      IItem clone;
      if (mapping.TryGetValue(item, out clone))
        return clone;
      else
        return item.Clone(this);
    }
    /// <summary>
    /// Registers a new clone for a given item.
    /// </summary>
    /// <param name="item">The original item.</param>
    /// <param name="clone">The clone of the original item.</param>
    public void RegisterClonedObject(IItem item, IItem clone) {
      mapping.Add(item, clone);
    }
  }
}
