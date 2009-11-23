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
  /// An interface describing the signature of helper classes required for creating deep
  /// clones of object graphs.
  /// </summary>
  public interface ICloner {
    /// <summary>
    /// Creates a deep clone of a given item.
    /// </summary>
    /// <param name="item">The item which should be cloned.</param>
    /// <returns>A clone of the given item.</returns>
    IItem Clone(IItem item);
    /// <summary>
    /// Registers a new clone for a given item.
    /// </summary>
    /// <param name="item">The original item.</param>
    /// <param name="clone">The clone of the original item.</param>
    void RegisterClonedObject(IItem item, IItem clone);
  }
}
