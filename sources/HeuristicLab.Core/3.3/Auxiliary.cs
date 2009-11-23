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
  /// Static helper class.
  /// </summary>
  public static class Auxiliary {
    #region Cloning
    /// <summary>
    /// Clones the given <paramref name="obj"/> (deep clone).
    /// </summary>
    /// <remarks>Checks before clone if object has not already been cloned.</remarks>
    /// <param name="obj">The object to clone.</param>
    /// <param name="clonedObjects">A dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object.</returns>
    public static object Clone(IStorable obj, IDictionary<Guid, object> clonedObjects) {
      object clone;
      if (clonedObjects.TryGetValue(obj.Guid, out clone))
        return clone;
      else
        return obj.Clone(clonedObjects);
    }
    #endregion
  }
}
