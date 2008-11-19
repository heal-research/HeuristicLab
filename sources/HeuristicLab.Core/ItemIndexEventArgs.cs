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
  /// Event arguments to be able to specify the affected item at a specified index.
  /// </summary>
  public class ItemIndexEventArgs : ItemEventArgs {
    private int myIndex;
    /// <summary>
    /// Gets the affected index.
    /// </summary>
    public int Index {
      get { return myIndex; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemIndexEventArgs"/> with the given <paramref name="item"/>
    /// and the given <paramref name="index"/>.
    /// </summary>
    /// <remarks>Calls constructor of base class <see cref="ItemEventArgs"/>.</remarks>
    /// <param name="item">The affected item.</param>
    /// <param name="index">The affected index.</param>
    public ItemIndexEventArgs(IItem item, int index)
      : base(item) {
      myIndex = index;
    }
  }
}
